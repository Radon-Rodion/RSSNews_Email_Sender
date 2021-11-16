using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using SPP_5;
using System.Xml;

namespace UnitTests
{
    [TestClass]
    public class ControllerUnitTest
    {
        const string CONFIG_FILE_ADDRESS = "testConfig.xml";

        const string rssItemString1 = @"
                <item>
                  <title>Space Exploration</title>
                  <link>liftoff.msfc.nasa.gov</link>
                  <author>pavel</author>
                  <category>world</category>
                  <description>Sky watchers in Europe, Asia, and parts of Alaska and Canada will experience a partial eclipse of the Sun on Saturday, May 31st.</description>
                  <pubDate>Fri, 30 May 2003 11:06:42 GMT</pubDate>
               </item>";
        const string rssItemString2 = @"
                <item>
                  <title>Star City</title>
                  <link>liftoff.msfc.nasa.gov/news/2003/news-starcity.asp</link>
                  <description>How do Americans get ready to work with Russians aboard the International Space Station? They take a crash course in culture, language and protocol at Russia's Star City.</description>
                  <pubDate>Tue, 03 Jun 2003 09:39:21 GMT</pubDate>
               </item>";

        [TestMethod]
        public void TestMailer()
        {
            Mailer mailer = new Mailer("smtp.gmail.com", 587, "pavel02rafeev@gmail.com", "8334191gmail");
            mailer.SendMess("pavel02rafeev@gmail.com", "UnitTest message", "This is just a program test");
        }

        [TestMethod]
        public void TestConfigDocParsing()
        {
            Controller controller = new Controller(CONFIG_FILE_ADDRESS);
            Assert.AreEqual<int>(controller.Configurations.Count, 1, $"Incorrect number of categories parsed!");
            Assert.AreEqual<string>(controller.Configurations[0].AuthorFilter, "pavel", $"Incorrect author filter!");
            Assert.AreEqual<string>(controller.Configurations[0].CategoryFilter, "world", $"Incorrect category filter!");
            Assert.AreEqual<string>(controller.Configurations[0].RssChannelLink, "liftoff.msfc.nasa.gov", $"Incorrect channel link!");
            Assert.AreEqual<string>(controller.Configurations[0].RecieverEmail, "pavel02rafeev@gmail.com", $"Incorrect reciever email!");
        }

        [TestMethod]
        public void TestConfiguration()
        {
            Controller controller = new Controller(CONFIG_FILE_ADDRESS);
            XmlDocument xmlDoc1 = new XmlDocument();
            xmlDoc1.LoadXml(rssItemString1);
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(rssItemString2);

            RSSItem item1 = new RSSItem(xmlDoc1.FirstChild);
            RSSItem item2 = new RSSItem(xmlDoc2.FirstChild);

            Assert.IsTrue(controller.Configurations[0].CheckRSSItemFits(item1), "Invalid checking fittness result!");
            Assert.IsFalse(controller.Configurations[0].CheckRSSItemFits(item2), "Invalid checking fittness result!");
        }
    }
}
