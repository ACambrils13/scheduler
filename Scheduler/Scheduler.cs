using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Creators;
using Scheduler.Validators;
using System.Globalization;

namespace Scheduler
{
    public class Scheduler
    {
        public static ScheduleEvent GetNextExecution(SchedulerConfigurator config)
        {
            ScheduleConfigValidator.ValidateBasicProperties(config);

            ScheduleEventCreator eventCreator = config.Type switch
            {
                ScheduleTypeEnum.Once => new ScheduleOnceCreator(),
                ScheduleTypeEnum.Recurring => new ScheduleRecurringCreator()
            };
            AsignCulture(config.Language.Value);
            return eventCreator.GetNextExecution(config);
        }

        internal static void AsignCulture(LanguageEnum language)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            switch (language)
            {
                case LanguageEnum.EnglishUK:
                    culture = new CultureInfo("en-GB");
                    break;
                case LanguageEnum.EnglishUS:
                    culture = new CultureInfo("en-US");
                    break;
                case LanguageEnum.Spanish:
                    culture = new CultureInfo("es");
                    break;
            }
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}

