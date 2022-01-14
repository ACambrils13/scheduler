using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Creators;
using Scheduler.Resources;
using Scheduler.Validators;
using System.Globalization;

namespace Scheduler
{
    public class Scheduler
    {
        public static ScheduleEvent GetNextExecution(SchedulerConfigurator config)
        {
            AsignCulture(config.Language);
            LanguageManager.GenerateResources(config);
            ScheduleConfigValidator.ValidateBasicProperties(config);
            ScheduleEventCreator eventCreator = config.Type switch
            {
                ScheduleTypeEnum.Once => new ScheduleOnceCreator(),
                ScheduleTypeEnum.Recurring => new ScheduleRecurringCreator()
            };
            return eventCreator.GetNextExecution(config);
        }

        internal static void AsignCulture(LanguageEnum? language)
        {
            CultureInfo culture = language switch
            {
                LanguageEnum.EnglishUS => new CultureInfo("en-US"),
                LanguageEnum.Spanish => new CultureInfo("es"),
                _ => new CultureInfo("en-GB"),
            };
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}

