using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Example.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string Dump(this HtmlHelper h,Object obj)
        {
            string dump = "<ul>";
            try
            {
                Dictionary<string, string> dictionary =
                    new Dictionary<string, string>();
                Type type = obj.GetType();
                var propInfo = type.GetProperties();
                for (int i = 0; i < propInfo.Length; i++)
                {
                    System.Reflection.PropertyInfo pi = (System.Reflection.PropertyInfo)propInfo.GetValue(i);
                    dictionary.Add(pi.Name,
                        (null == pi.GetValue(obj, new object[] { })) ?
                        "null" : pi.GetValue(obj, new object[] { }).ToString());
                }
                foreach (System.Collections.Generic.KeyValuePair<string, string> pair in dictionary)
                {
                    dump += string.Format("<li>{0}={1}</li>", pair.Key, pair.Value);
                }
            }
            catch (Exception ex)
            {
                // use a proper log instead of a text file..
                System.IO.File.AppendAllText(@"C:\myapplicationname.dump.exception.txt",
                    "--------------------------------------\n" +
                    "Exception:\n\tSource: " + ex.Source +
                    "\n\tMessage: \n" + ex.Message +
                    "\n\tStack: \n" + ex.StackTrace +
                    "\n------------------------------------");
            }
            return dump+"</ul>";
        }

    }
}