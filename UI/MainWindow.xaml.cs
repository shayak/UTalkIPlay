using System;
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
using System.IO;
using WinForms = System.Windows.Forms;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel mainWinVM;
        public StreamWriter sw;
        
        public MainWindow()
        {
            sw = new StreamWriter("error.log");
            try
            {                
                InitializeComponent();
                sw.WriteLine("init success");
                mainWinVM = new MainWindowViewModel(this);
                sw.WriteLine("VM success");
                this.DataContext = mainWinVM;
            }
            catch (Exception ex)
            {
                sw.WriteLine("Exception details: \r\n" + ex.Message + ex.StackTrace);
            }
            sw.Close();
        }

        public void Add_Songs_Button_Click(Object sender, RoutedEventArgs e)
        {
            var dialog = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = dialog.ShowDialog();
            mainWinVM.AddSongsAsync(dialog.SelectedPath);
        }

        public void Start_Listening_Button_Click(Object sender, RoutedEventArgs e)
        {
            mainWinVM.ToggleListenMode();
        }     
        
    }
}
