using Microsoft.Win32;
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

namespace ExampleOfUsingAppDomainAndDll
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DllSateTextBlock.Text = "Unloaded";
            InitializeComponent();
            FirstInitialize();
            StateButton.IsEnabled = true;
            StateButton.Content = "Unload";
            DllSateTextBlock.Text = "Loaded";
        }

        private void FirstInitialize()
        {
            var res = MessageBox.Show("Load .dll files from default path?", "Load DLL", MessageBoxButton.YesNo, MessageBoxImage.Question);
            DllSateTextBlock.Text = "Loading...";
            if (res == MessageBoxResult.Yes)
            {
                LoadDllFromDirectory();
            }
            else if (res == MessageBoxResult.No)
            {                
                LoadDllFromDirectory(ChooseDirectory());
            }
            else
            {
                throw new Exception("Some wents wrong... Pls,try restart the app, sorry");
            }
        }

        private string ChooseDirectory()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.ShowDialog();
            return openFileDialog.SafeFileName;
        }

        private void LoadDllFromDirectory()
        {
            LoadDllFromDirectory(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        private void LoadDllFromDirectory(String path)
        {
            
        }

        private void StateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!((string)(StateButton.Content)).Contains("Unload"))
            {
                StateButton.IsEnabled = false;
                StateButton.Content = "Unload";
                DllSateTextBlock.Text = "Loading...";
                FirstInitialize();
                StateButton.IsEnabled = true;
                DllSateTextBlock.Text = "Loaded";
            }
            else
            {
                StateButton.IsEnabled = false;
                StateButton.Content = "Load";
                DllSateTextBlock.Text = "Unloading...";
                UnloadAllDll();
                StateButton.IsEnabled = true;
                DllSateTextBlock.Text = "Unloaded";
            }
        }

        private void UnloadAllDll()
        {
            throw new NotImplementedException();
        }
    }
}
