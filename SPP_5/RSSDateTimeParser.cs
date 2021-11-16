using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_5
{
    public static class RSSDateTimeParser
    {
        public static DateTime Parse(string rssPubDate) //Example: "Fri, 30 May 2003 11:06:42 GMT"
        {
            string[] dateTimeElements = rssPubDate.Split(' ');
            string[] timeElements = dateTimeElements[4].Split(':');

            int year = System.Convert.ToInt32(dateTimeElements[3]);
            int month = MonthCodeToNumber(dateTimeElements[2]);
            int day = System.Convert.ToInt32(dateTimeElements[1]);
            int hour = System.Convert.ToInt32(timeElements[0]);
            int minute = System.Convert.ToInt32(timeElements[1]);
            int second = System.Convert.ToInt32(timeElements[2]);

            return new DateTime(year, month, day, hour, minute, second);
        }

        private static int MonthCodeToNumber(string monthCode)
        {
            switch (monthCode)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    return -1;
            }
        }
    }
}
