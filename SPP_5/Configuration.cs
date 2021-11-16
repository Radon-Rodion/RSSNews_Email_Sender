using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SPP_5
{
    public class Configuration
    {
        public static string[] FilterCategories
        {
            get
            {
                return new string[] {/* "language", */"author", "category" };
            }
        }

        public string RecieverEmail { get; private set; }
        public string RssChannelLink { get; private set; }
        //public string LanguageFilter { get; private set; }
        public string AuthorFilter { get; private set; }
        public string CategoryFilter { get; private set; }

        public Configuration(XmlNode configNode)
        {
            foreach (XmlNode xmlTag in configNode.ChildNodes)
            {
                switch (xmlTag.Name)
                {
                    case "reciever":
                        RecieverEmail = xmlTag.InnerText;
                        break;
                    case "channel":
                        RssChannelLink = xmlTag.InnerText;
                        break;
                    /*case "language":
                        LanguageFilter = xmlTag.InnerText;
                        break;*/
                    case "author":
                        AuthorFilter = xmlTag.InnerText;
                        break;
                    case "category":
                        CategoryFilter = xmlTag.InnerText;
                        break;
                }
            }
        }

        public bool CheckRSSItemFits(RSSItem item)
        {
            foreach(string category in RSSItem.FilterCategories)
            {
                if (this[category]!= null && !this[category].Equals(item[category])) return false;
            }
            return true;
        }

        public string this [string categoryName]
        {
            get
            {
                switch (categoryName)
                {
                    /*case "language":
                        return LanguageFilter;*/
                    case "author":
                        return AuthorFilter;
                    case "category":
                        return CategoryFilter;
                    default:
                        return null;
                }
            }
        }

    }
}
