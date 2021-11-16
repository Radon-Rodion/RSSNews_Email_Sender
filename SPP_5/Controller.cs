using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Threading;

namespace SPP_5
{
    public class Controller
    {
        public ObservableCollection<Configuration> Configurations { get; set; }
        //public ObservableCollection<SentInfo> SentInfo { get; set; }
        Mailer mailer;
        Queue<(Configuration config, RSSItem item)> queueForFiltration;
        Queue<(Configuration config, RSSItem item)> queueForSending;
        private int timerTime = 3600;

        public Controller(string configFileAddr)
        {
            ParseConfigsFromXmlDocument(LoadConfigurationDoc(configFileAddr));
            queueForFiltration = new Queue<(Configuration config, RSSItem item)>();
            queueForSending = new Queue<(Configuration config, RSSItem item)>();

            // создаем таймер
            if(timerTime > 0)
            {
                TimerCallback tm = new TimerCallback(ProcessConfigurations);
                Timer timer = new Timer(tm, null, 0, timerTime);
            }
            //SingleThreadParseConfigurations();
        }

        private XmlDocument LoadConfigurationDoc(string configFileAddr)
        {
            XmlTextReader xmlTextReader = new XmlTextReader(configFileAddr);
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlTextReader);
                xmlTextReader.Close();
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }

            return xmlDoc;
        }

        private void ParseConfigsFromXmlDocument(XmlDocument xmlDoc)
        {
            Configurations = new ObservableCollection<Configuration>();
            XmlNode configsXmlNode = xmlDoc.GetElementsByTagName("configurations")[0];

            if (configsXmlNode != null)
            {
                string mailerHostName = "";
                string mailerUserName = "";
                string mailerUserPassword = "";
                int mailerPort = 0;

                foreach (XmlNode configNode in configsXmlNode.ChildNodes)
                {
                    switch (configNode.Name)
                    {
                        case "timer":
                            timerTime = System.Convert.ToInt32(configNode.InnerText) * 1000;
                            break;
                        case "mailerHostName":
                            mailerHostName = configNode.InnerText;
                            break;
                        case "mailerUserName":
                            mailerUserName = configNode.InnerText;
                            break;
                        case "mailerUserPassword":
                            mailerUserPassword = configNode.InnerText;
                            break;
                        case "mailerPort":
                            mailerPort = System.Convert.ToInt32(configNode.InnerText);
                            break;
                        case "configuration":
                            Configurations.Add(new Configuration(configNode));
                            break;
                    }
                }
                mailer = new Mailer(mailerHostName, mailerPort, mailerUserName, mailerUserPassword);
            }
            else
            {
                throw new ArgumentException("Invalid Configurations: description not found!");
            }
        }
        public void SingleThreadParseConfigurations()
        {
            LoadNewsFromSources();
            FilterNews();
            SendNews();
        }

        public void ProcessConfigurations(object obj)
        {
            Thread loaderThread = new Thread(new ThreadStart(LoadNewsFromSources));
            loaderThread.Start();

            Thread filterThread = new Thread(new ThreadStart(FilterNews));
            filterThread.Start();

            Thread senderThread = new Thread(new ThreadStart(SendNews));
            senderThread.Start();
        }

        private void LoadNewsFromSources()
        {
            try
            {
                foreach (Configuration config in Configurations)
                {
                    RSSChannel channel = new RSSChannel(config.RssChannelLink);
                    foreach (RSSItem item in channel.ItemsList)
                    {
                        queueForFiltration.Enqueue((config, item));
                    }
                }
            } 
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            } 
            finally
            {
                queueForFiltration.Enqueue((null, null)); //Stop condition for next thread
            }
        }

        private void FilterNews()
        {
            try
            {
                while (true)
                {
                    if (queueForFiltration.Count > 0)
                    {
                        (Configuration config, RSSItem item) configAndItem = queueForFiltration.Dequeue();
                        if (configAndItem.config == null && configAndItem.item == null) //Stop condition
                        {
                            break;
                        }

                        if (configAndItem.item.PubDate.AddHours(100)>=DateTime.Now && configAndItem.config.CheckRSSItemFits(configAndItem.item))
                        {
                            queueForSending.Enqueue(configAndItem);
                        }
                    }
                    else Thread.Sleep(300);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                queueForSending.Enqueue((null, null)); //Stop condition for next thread
            }
        }

        private void SendNews()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        (Configuration config, RSSItem item) configAndItem = queueForSending.Dequeue();

                        if (configAndItem.config == null && configAndItem.item == null) //Stop condition
                        {
                            break;
                        }

                        MessageToSend mess = new MessageToSend();
                        mess.Address = configAndItem.config.RecieverEmail;
                        mess.Theme = configAndItem.item.Title;
                        mess.Body = configAndItem.item.Description;
                        mess.Author = configAndItem.item.Author;
                        mess.Category = configAndItem.item.Category;

                        mailer.SendMess(mess.Address, mess.Theme, mess.Body);

                        //AddSentInfo(mess);
                    } catch (InvalidOperationException)
                    {
                        Thread.Sleep(300);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /*private void AddSentInfo(MessageToSend mess)
        {
            if (SentInfo != null)
            {
                SentInfo info = new SentInfo();
                info.Email = mess.Address;
                info.Title = mess.Theme;
                info.Time = DateTime.Now.ToString();
                info.Id = SentInfo.Count;
                SentInfo.Add(info);
            }
        }*/

        private class MessageToSend
        {
            public string Address { get; set; }
            public string Theme { get; set; }
            public string Body { get; set; }

            public string Author { set
                {
                    if(value != null && value != "")
                    {
                        Body+=$"\nAuthor: {value}";
                    }
                } 
            }

            public string Category
            {
                set
                {
                    if (value != null && value != "")
                    {
                        Body = $"Category: {value}\n{Body}";
                    }
                }
            }
        }
    }
}
