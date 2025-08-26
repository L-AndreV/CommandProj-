using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommandProj.Models;
using Newtonsoft.Json.Linq;

namespace CommandProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BankContext bContext;
        public MainWindow()
        {
            bContext = new BankContext();
            bContext.Database.EnsureCreated();
            //fillCounties();
            InitializeComponent();           
        }

        //private void fillCounties()
        //{
        //    JObject jsonObject = JObject.Parse(CountriesData.CoData);
        //    JArray tarr = (JArray)jsonObject["co"];

        //    int index = 0;
        //    foreach(var con in tarr)
        //    {
        //        bContext.Countries.Add(new Country {CountryId=index, Name = con["name"]["common"].ToString(), PhoneCode = con["idd"]["root"].ToString() });
        //        index++;
        //        bContext.SaveChanges();
        //    }
        //}
    }
}