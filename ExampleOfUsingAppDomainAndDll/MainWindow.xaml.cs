using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
    [Serializable]
    public partial class MainWindow : Window
    {
        private static List<AppDomain> _domains = new List<AppDomain>();
        private ObservableCollection<String> _trueDomains = new ObservableCollection<string>();

        public ObservableCollection<string> TrueDomains
        {
            get => _trueDomains;
            set
            {
                _trueDomains = value;
            }
        }

        public MainWindow()
        {            
            InitializeComponent();
            ResultListView.ItemsSource = _trueDomains;
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
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog vistaFolderBrowserDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            vistaFolderBrowserDialog.ShowDialog();
            return vistaFolderBrowserDialog.SelectedPath;
        }

        private void LoadDllFromDirectory()
        {
            LoadDllFromDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Remove(0, 8)));
        }

        private void LoadDllFromDirectory(String path)
        {
            if (path.Length > 0)
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (System.IO.Path.GetExtension(file).Contains("dll"))
                    {
                        LoadDll(file);
                    }
                }
            }
        }

        private void LoadDll(String path)
        {
            var d = AppDomain.CreateDomain($"{_domains.Count.ToString()} domain");
            _domains.Add(d);
            d.DomainUnload += d_DomainUnload;
            d.Load(AssemblyName.GetAssemblyName(path));

            Assembly assembly = Assembly.Load(AssemblyName.GetAssemblyName(path));
            string namespaceDll = $"{System.IO.Path.GetFileNameWithoutExtension(path)}.Class1";
            Type currentClass = assembly.GetType(namespaceDll);
            var foundedInterface = currentClass?.GetInterface("IExtension");
            if (foundedInterface != null)
            {
                ConstructorInfo ci = currentClass.GetConstructor(new Type[] { });
                if (ci != null)
                {
                    IExtension newObj = (IExtension)ci.Invoke(new object[] { });
                    var res = newObj.GetExtensionName();
                    TrueDomains.Add(res);
                    MessageBox.Show(res);
                }
            }            
        }        

        static void d_DomainUnload(object sender, EventArgs e)
        {
            _domains.Remove(sender as AppDomain);
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
            foreach (var domain in _domains)
            {
                domain.DomainUnload -= d_DomainUnload;                
            }
            _domains.Clear();
            TrueDomains.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnloadAllDll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DllSateTextBlock.Text = "Unloaded";
            FirstInitialize();
            StateButton.IsEnabled = true;
            StateButton.Content = "Unload";
            DllSateTextBlock.Text = "Loaded";
        }
    }
}
