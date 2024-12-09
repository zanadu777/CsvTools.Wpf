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
using CsvTools.Wpf.Wpf;
using Newtonsoft.Json;

namespace CsvTools.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var json = IsolatedStorageHelper.ReadTextFromIsolatedStorage("appState");
            var vm = JsonConvert.DeserializeObject<MainWindowVm>(json);
            DataContext = vm ?? new MainWindowVm();
        }
    }
}