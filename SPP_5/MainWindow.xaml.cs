using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SPP_5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SentInfo> SentInfo { get; set; }
        const string CONFIG_FILE_ADDRESS = "config.xml";
        Controller controller;

        public MainWindow()
        {
            SentInfo = new ObservableCollection<SentInfo>();
            //SentInfo.Add(new SentInfo() { Id = 0, Title = "fg", Email = "dfg", Time = "12:23"});
            
            InitializeComponent();
            try
            {
                controller = new Controller(CONFIG_FILE_ADDRESS);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
            //controller.SentInfo = SentInfo;

            //sentList.DataContext = SentInfo;
            //sentList.ItemsSource = SentInfo;
            configsList.DataContext = controller.Configurations;
            configsList.ItemsSource = controller.Configurations;

            //SentInfo.Add(new SentInfo() { Id = 1, Title = "jmnb", Email = "zxcv", Time = "12:25" });
            /*Mailer mailer = new Mailer("smtp.gmail.com", 587, "pavel02rafeev@gmail.com", "8334191gmail");
            mailer.SendMess("pavel02rafeev@gmail.com", "Тест", "<h2>Письмо-тест работы smtp-клиента</h2>");*/
        }
    }
}
