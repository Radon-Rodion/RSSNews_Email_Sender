using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using SPP_5;

namespace UnitTests
{
    [TestClass]
    public class RssUnitTest
    {
        const string rssItemString1 = @"
                <item>
                  <title>Space Exploration</title>
                  <link>liftoff.msfc.nasa.gov</link>
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
        const string rssChannelStringStart = @"<?xml version='1.0'?>
                    <rss version='2.0'>
                        <channel>
                           <title>Liftoff News</title>
                           <link>liftoff.msfc.nasa.gov</link>
                           <description>Liftoff to Space Exploration.</description>";
        const string rssChannelStringEnd = @"
                        </channel>
                    </rss>";
        [TestMethod]
        public void TestItemParser()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rssItemString1);
            RSSItem item = new RSSItem(xmlDoc.FirstChild);
            Assert.AreEqual<string>("Space Exploration", item.Title, "Title parsed incorrectly!");
            Assert.AreEqual<string>("liftoff.msfc.nasa.gov", item.Link, "Link parsed incorrectly!");
            Assert.AreEqual<string>("Sky watchers in Europe, Asia, and parts of Alaska and Canada will experience a partial eclipse of the Sun on Saturday, May 31st."
                , item.Description, "Description parsed incorrectly!");
        }

        [TestMethod]
        public void TestItemsList()
        {
            XmlDocument xmlDoc1 = new XmlDocument();
            xmlDoc1.LoadXml(rssItemString1);
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(rssItemString2);

            RSSItem item1 = new RSSItem(xmlDoc1.FirstChild);
            RSSItem item2 = new RSSItem(xmlDoc2.FirstChild);

            RSSItemList list = new RSSItemList();
            list.Add(item1);
            Assert.IsFalse(list.Contains(item2), "Item containment information is incorrect (must be false)!");
            list.Add(item2);

            Assert.IsTrue(list.Contains(item1), "Item containment information is incorrect (must be true)!");
            Assert.IsTrue(list.Contains(item2), "Item containment information is incorrect (must be true)!");
            Assert.AreEqual<RSSItem>(item2, list.GetItem("Star City"), "Right item not found!");
        }

        [TestMethod]
        public void TestChannel()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rssChannelStringStart + rssItemString1 + rssItemString2 + rssChannelStringEnd);

            RSSChannel channel = new RSSChannel(xmlDoc);
            Assert.AreEqual<string>("Liftoff News", channel.Title, "Channel title parsing is incorrect!");
            Assert.AreEqual<string>("liftoff.msfc.nasa.gov", channel.Link, "Channel link parsing is incorrect!");
            Assert.AreEqual<string>("Liftoff to Space Exploration.", channel.Description, "Channel description parsing is incorrect!");

            Assert.AreEqual<int>(channel.ItemsList.Count, 2, "Items list parsing is incorrect!");
        }

        [TestMethod]
        public void TestDateTimeParser()
        {
            DateTime dtm1 = RSSDateTimeParser.Parse("Fri, 30 May 2003 21:06:42 GMT");
            Assert.AreEqual<DateTime>(new DateTime(2003, 5, 30, 21, 6, 42), dtm1, "Parsing first dateTime incorrect!");

            DateTime dtm2 = RSSDateTimeParser.Parse("Tue, 03 Jun 2019 09:39:21 GMT");
            Assert.AreEqual<DateTime>(new DateTime(2019, 6, 3, 9, 39, 21), dtm2, "Parsing second dateTime incorrect!");
        }
    }
}
