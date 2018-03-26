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

        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LocBamlHandler.ResolveAssembly); // register AssemblyResolver event for bamlLocalizer
        }

        private void loadDLLButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select resource DLL";
            ofd.DefaultExt = "dll";
            ofd.Filter = "DLL files|*.dll|All files|*.*";
            ofd.Multiselect = false;
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                this.dllPathField.Text = ofd.FileName;
                this.handler = new LocBamlHandler(ofd.FileName);
                this.handler.Load();
            }
        }
    }
}
