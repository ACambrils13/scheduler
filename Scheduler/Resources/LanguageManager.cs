using Scheduler.Auxiliary;
using Scheduler.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.Resources
{
    internal static class LanguageManager
    {
        private static LanguagesId languageId;
        private static ICollection<StringResource> stringResources;

        internal static string GetStringResource(string code)
        {
            StringResource resource = stringResources?.FirstOrDefault(
                sr => sr.Code.Trim().ToLower() == code.Trim().ToLower() && sr.IdLanguage == languageId);
            if (resource == null)
            {
                resource = stringResources?.FirstOrDefault(
                sr => sr.Code.Trim().ToLower() == code.Trim().ToLower() && sr.IdLanguage == LanguagesId.en);
            }
            return resource?.Value ?? string.Empty;
        }

        internal static List<string> GetStringResourcesList(ICollection codeList)
        {
            List<string> result = new();
            foreach (var item in codeList)
            {
                string term = GetStringResource(item.ToString());
                result.Add(term);
            }
            return result;
        }

        internal static void GenerateResources(SchedulerConfigurator config)
        {
            languageId = GetLanguage(config.Language);
            stringResources = new HashSet<StringResource>();
            switch (languageId)
            {
                case LanguagesId.en:
                default:
                    AddEnglishStrings();
                    break;
                case LanguagesId.es:
                    AddSpanishStrings();
                    break;
            }
        }

        private static LanguagesId GetLanguage(LanguageEnum? language)
        {
            return language switch
            {
                LanguageEnum.Spanish => LanguagesId.es,
                _ => LanguagesId.en,
            };
        }

        private static void AddStringResource(LanguagesId language, string code, string value)
        {
            stringResources.Add(new StringResource()
            {
                IdLanguage = language,
                Code = code,
                Value = value
            });
        }

        private static void AddEnglishStrings()
        {
            AddStringResource(LanguagesId.en, "And", "and");
            AddStringResource(LanguagesId.en, "ConfError", "Configuration Error: {0}");
            AddStringResource(LanguagesId.en, "Day", "day");
            AddStringResource(LanguagesId.en, "Days", "days");
            AddStringResource(LanguagesId.en, "EventDescDailyLimits", "between {0} and {1}");
            AddStringResource(LanguagesId.en, "EventDescDayOfMonth", "the day {0} of ");
            AddStringResource(LanguagesId.en, "EventDescLimitsEnd", "to {0}");
            AddStringResource(LanguagesId.en, "EventDescLimitsStart", "starting on {0}");
            AddStringResource(LanguagesId.en, "EventDescMonthFrecuency", "the {0} {1} of ");
            AddStringResource(LanguagesId.en, "EventDescOnce", "Occurs once.");
            AddStringResource(LanguagesId.en, "EventDescRecurringEvery", "every {0} {1}");
            AddStringResource(LanguagesId.en, "EventDescRecurringHour", "at {0}");
            AddStringResource(LanguagesId.en, "EventDescRecurringStart", "Occurs ");
            AddStringResource(LanguagesId.en, "EventDescRecurringWeekly", "on {0}");
            AddStringResource(LanguagesId.en, "EventDescSchedule", "Schedule will be used on {0} at {1}");
            AddStringResource(LanguagesId.en, "ExcDailyConfig", "Daily Frecuency configuration has errors.");
            AddStringResource(LanguagesId.en, "ExcDateMaxValue", "{0} can't be a Maximun DateTime value.");
            AddStringResource(LanguagesId.en, "ExcEnumError", "{0} hasn't be a valid option.");
            AddStringResource(LanguagesId.en, "ExcHoursValue", "{0} ins't a valid Day Hour value.");
            AddStringResource(LanguagesId.en, "ExcLimits", "Next schedule event isn't between date limits.");
            AddStringResource(LanguagesId.en, "ExcLimitsEndBeforeStart", "End Limit should be after Start Limit.");
            AddStringResource(LanguagesId.en, "ExcMonthlyDay", "The day selected has to be in 1-31");
            AddStringResource(LanguagesId.en, "ExcMonthlyTypeConfig", "Monthly Configuration selection isn't correct.");
            AddStringResource(LanguagesId.en, "ExcObjectNull", "{0} hasn't got any value.");
            AddStringResource(LanguagesId.en, "ExcPeriod", "{0} should be a positive number.");
            AddStringResource(LanguagesId.en, "First", "first");
            AddStringResource(LanguagesId.en, "Fourth", "fourth");
            AddStringResource(LanguagesId.en, "Friday", "Friday");
            AddStringResource(LanguagesId.en, "Hours", "hours");
            AddStringResource(LanguagesId.en, "Last", "last");
            AddStringResource(LanguagesId.en, "Minutes", "minutes");
            AddStringResource(LanguagesId.en, "Monday", "Monday");
            AddStringResource(LanguagesId.en, "Months", "months");
            AddStringResource(LanguagesId.en, "Saturday", "Saturday");
            AddStringResource(LanguagesId.en, "Second", "second");
            AddStringResource(LanguagesId.en, "Seconds", "seconds");
            AddStringResource(LanguagesId.en, "Sunday", "Sunday");
            AddStringResource(LanguagesId.en, "Third", "third");
            AddStringResource(LanguagesId.en, "Thursday", "Thursday");
            AddStringResource(LanguagesId.en, "Tuesday", "Tuesday");
            AddStringResource(LanguagesId.en, "Wednesday", "Wednesday");
            AddStringResource(LanguagesId.en, "Weekday", "weekday");
            AddStringResource(LanguagesId.en, "WeekendDay", "weekend day");
            AddStringResource(LanguagesId.en, "Weeks", "weeks");
            AddStringResource(LanguagesId.en, "Years", "years");
        }

        private static void AddSpanishStrings()
        {
            AddStringResource(LanguagesId.es, "And", "y");
            AddStringResource(LanguagesId.es, "ConfError", "Error de Configuración: {0}");
            AddStringResource(LanguagesId.es, "Day", "día");
            AddStringResource(LanguagesId.es, "Days", "días");
            AddStringResource(LanguagesId.es, "EventDescDailyLimits", "entre las {0} y las {1}");
            AddStringResource(LanguagesId.es, "EventDescDayOfMonth", "el día {0} de ");
            AddStringResource(LanguagesId.es, "EventDescLimitsEnd", "hasta el {0}");
            AddStringResource(LanguagesId.es, "EventDescLimitsStart", "comenzando el {0}");
            AddStringResource(LanguagesId.es, "EventDescMonthFrecuency", "el {0} {1} de ");
            AddStringResource(LanguagesId.es, "EventDescOnce", "Ocurre una única vez.");
            AddStringResource(LanguagesId.es, "EventDescRecurringEvery", "cada {0} {1}");
            AddStringResource(LanguagesId.es, "EventDescRecurringHour", "a las {0}");
            AddStringResource(LanguagesId.es, "EventDescRecurringStart", "Ocurre ");
            AddStringResource(LanguagesId.es, "EventDescRecurringWeekly", "el {0}");
            AddStringResource(LanguagesId.es, "EventDescSchedule", "Programado para el {0} a las {1}");
            AddStringResource(LanguagesId.es, "ExcDailyConfig", "La configuración diaria tiene errores.");
            AddStringResource(LanguagesId.es, "ExcDateMaxValue", "{0} no puede ser un valor máximo de DateTime.");
            AddStringResource(LanguagesId.es, "ExcEnumError", "{0} no es una opción válida.");
            AddStringResource(LanguagesId.es, "ExcHoursValue", "{0} no es un valor de hora válido.");
            AddStringResource(LanguagesId.es, "ExcLimits", "El siguiente evento no está incluido entre los limites fijados.");
            AddStringResource(LanguagesId.es, "ExcLimitsEndBeforeStart", "El limite final debe ser posterior al límite inicial.");
            AddStringResource(LanguagesId.es, "ExcMonthlyDay", "El día seleccionado debe estar entre 1-31");
            AddStringResource(LanguagesId.es, "ExcMonthlyTypeConfig", "La configuración mensual seleccionada es incorrecta.");
            AddStringResource(LanguagesId.es, "ExcObjectNull", "{0} no tiene valor.");
            AddStringResource(LanguagesId.es, "ExcPeriod", "{0} debe ser un número positivo.");
            AddStringResource(LanguagesId.es, "First", "primer");
            AddStringResource(LanguagesId.es, "Fourth", "cuarto");
            AddStringResource(LanguagesId.es, "Friday", "viernes");
            AddStringResource(LanguagesId.es, "Hours", "horas");
            AddStringResource(LanguagesId.es, "Last", "último");
            AddStringResource(LanguagesId.es, "Minutes", "minutos");
            AddStringResource(LanguagesId.es, "Monday", "lunes");
            AddStringResource(LanguagesId.es, "Months", "meses");
            AddStringResource(LanguagesId.es, "Saturday", "sábado");
            AddStringResource(LanguagesId.es, "Second", "segundo");
            AddStringResource(LanguagesId.es, "Seconds", "segundos");
            AddStringResource(LanguagesId.es, "Sunday", "domingo");
            AddStringResource(LanguagesId.es, "Third", "tercer");
            AddStringResource(LanguagesId.es, "Thursday", "jueves");
            AddStringResource(LanguagesId.es, "Tuesday", "martes");
            AddStringResource(LanguagesId.es, "Wednesday", "miércoles");
            AddStringResource(LanguagesId.es, "Weekday", "día entre semana");
            AddStringResource(LanguagesId.es, "WeekendDay", "día de fin de semana");
            AddStringResource(LanguagesId.es, "Weeks", "semanas");
            AddStringResource(LanguagesId.es, "Years", "años");
        }
    }

    internal enum LanguagesId
    {
        en,
        es
    }
}
