using System;
using System.Windows;
using Microsoft.Win32;

namespace locbamlUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private LocBamlHandler handler;
        private ViewModel CurrentModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.CurrentModel = new ViewModel();
            this.DataContext = CurrentModel;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LocBamlHandler.ResolveAssembly); // register AssemblyResolver event for bamlLocalizer
        }

        private void loadDLLButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select file with localization content...";
            ofd.DefaultExt = "dll";
            ofd.Filter = "Dynamic link libraries|*.dll|Executables|*.exe|Resource files|*.resources|BAML files|*.baml|All files|*.*";
            ofd.Multiselect = false;
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                this.dllPathField.Text = ofd.FileName;
                this.handler = new LocBamlHandler(ofd.FileName, CurrentModel);
                //this.DataContext = this.handler.CurrentViewModel;
                bool state = this.handler.Load();
                this.bamlDataGrid.IsEnabled = state;
            }
        }
    }
}
