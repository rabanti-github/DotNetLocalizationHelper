using System.Windows;

namespace locbamlUI
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {

        public static bool State = false;

        public OptionsWindow()
        {
            InitializeComponent();
            State = false;
            this.Icon = Application.Current.MainWindow.Icon;
        }


        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
           Properties.Settings.Default.Save();
           State = true;
           this.Close();
        }
    }
}
