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
            InitializeComponent();
        }
    }
}