using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SPP_5
{
    public class RSSItem
    {
        public static string[] FilterCategories { 
            get {
                return new string[]{ "author", "category"};
            }
        }

        public string Title { get; private set; } // заголовок записи
        public string Link { get; private set; } // ссылка на полный текст
        public string Description { get; private set; }// описание записи
        public string Author { get; private set; }
        public string Category { get; private set; }
        public DateTime PubDate { get; private set; }

        public RSSItem(XmlNode node)
        {
            foreach (XmlNode xmlTag in node.ChildNodes)
            {
                switch (xmlTag.Name)
                {
                    case "title":
                        Title = xmlTag.InnerText;
                        break;
                    case "description":
                        Description = xmlTag.InnerText;
                        break;
                    case "link":
                        Link = xmlTag.InnerText;
                        break;
                    case "author":
                        Author = xmlTag.InnerText;
                        break;
                    case "category":
                        Category = xmlTag.InnerText;
                        break;
                    case "pubDate":
                        PubDate = RSSDateTimeParser.Parse(xmlTag.InnerText);
                        break;
                }
            }
        }
        public string this[string categoryName]
        {
            get
            {
                switch (categoryName)
                {
                    case "author":
                        return Author;
                    case "category":
                        return Category;
                    default:
                        return null;
                }
            }
        }
    }
}
