using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Scheduler.Language
{
    public class Localize
    {
        public static string GetLocalizedText(string key)
        {
            return Resources.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);
        }

        public static List<string> GetLocalizedList(ICollection list)
        {
            List<string> result = new();
            foreach(var item in list)
            {
                string term = GetLocalizedText(item.ToString());
                result.Add(term);
            }
            return result;
        }
    }
}
