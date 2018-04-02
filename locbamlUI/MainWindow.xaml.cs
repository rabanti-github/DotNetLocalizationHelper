
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;


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
           // FemtoXLSX.XlsxReader r = new XlsxReader(@"C:\temp\excelTest2.xlsx");
           // r.Read();

            InitializeComponent();
            this.CurrentModel = new ViewModel();
            this.DataContext = CurrentModel;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LocBamlHandler.ResolveAssembly); // register AssemblyResolver event for bamlLocalizer
            SetColumnSettings();
        }

        private void loadDLLButton_Click(object sender, RoutedEventArgs e)
        {
            LoadResources();
        }

        private void exportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportResources();
        }

        private void importMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImportResources();
        }

        private void showStreamNameColumnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.ShowStreamNameColumn == true) { CurrentModel.ShowStreamNameColumn = false; }
            else { CurrentModel.ShowStreamNameColumn = true; }
        }

        private void showResourceKeyColumnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.ShowResourceKeyColumn == true) { CurrentModel.ShowResourceKeyColumn = false; }
            else { CurrentModel.ShowResourceKeyColumn = true; }
        }

        private void showResourceCategoryColumnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.ShowResourceCategoryColumn == true) { CurrentModel.ShowResourceCategoryColumn = false; }
            else { CurrentModel.ShowResourceCategoryColumn = true; }
        }

        private void showReadableColumnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.ShowReadableColumn == true) { CurrentModel.ShowReadableColumn = false; }
            else { CurrentModel.ShowReadableColumn = true; }
        }

        private void showModifiableColumnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.ShowModifiableColumn == true) { CurrentModel.ShowModifiableColumn = false; }
            else { CurrentModel.ShowModifiableColumn = true; }
        }

        private void showCommentColumnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.ShowCommentColumn == true) { CurrentModel.ShowCommentColumn = false; }
            else { CurrentModel.ShowCommentColumn = true; }
        }

        private void showContentColumnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.ShowContentColumn == true) { CurrentModel.ShowContentColumn = false; }
            else { CurrentModel.ShowContentColumn = true; }
        }

        private void loadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadResources();
        }

        private void LoadResources()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select file with resource content...";
            ofd.DefaultExt = "dll";
            ofd.Filter = "Dynamic link libraries|*.dll|Executables|*.exe|Resource files|*.resources|BAML files|*.baml|All files|*.*";
            ofd.Multiselect = false;
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                this.handler = new LocBamlHandler(ofd.FileName, CurrentModel);
                //this.DataContext = this.handler.CurrentViewModel;
                bool state = this.handler.Load();
                this.bamlDataGrid.IsEnabled = state;
            }
        }

        private void ExportResources()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Export resources as...";
            sfd.DefaultExt = "xlsx";
            sfd.Filter = "Excel Worksheet|*.xlsx|CSV file|*.csv";
            bool? result = sfd.ShowDialog();
            if (result == true)
            {
                FileInfo fi = new FileInfo(sfd.FileName);
                string error = null;
                if (fi.Extension.ToLower() == ".xlsx")
                {
                    error = LocalizationItem.ExportAsXlsx(sfd.FileName, this.CurrentModel.LocalizationList);
                }
                else if (fi.Extension.ToLower() == ".csv")
                {
                    error = LocalizationItem.ExportAsCsv(sfd.FileName, this.CurrentModel.LocalizationList);
                }

                if (error != null)
                {
                    this.CurrentModel.Status = error;
                    MessageBox.Show("The resources could not be exported:\n" + error, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    this.CurrentModel.Status = "The resources were exported successfully";
                    MessageBox.Show("The resources were exported successfully", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ImportResources()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Import resources from...";
            ofd.DefaultExt = "xlsx";
            ofd.Multiselect = false;
            ofd.Filter = "Excel Worksheet|*.xlsx|CSV file|*.csv";
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                FileInfo fi = new FileInfo(ofd.FileName);
                string error = null;
                IList<LocalizationItem> list = null;
                if (fi.Extension.ToLower() == ".xlsx")
                {
                   list = LocalizationItem.ImportFromXlsx(ofd.FileName, out error);
                }
                else if (fi.Extension.ToLower() == ".csv")
                {
                    list = LocalizationItem.ImportFromCsv(ofd.FileName, out error);
                }

                if (error != null)
                {
                    CurrentModel.ClearLocalizationList();
                    this.CurrentModel.Status = error;
                    CurrentModel.Loaded = false;
                    MessageBox.Show("The resources could not be imported:\n" + error, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CurrentModel.SetLocalizationList(list);
                    CurrentModel.Loaded = true;
                    this.CurrentModel.Status = "The resources were imported successfully";
                    MessageBox.Show("The resources were imported successfully", "Import", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void optionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow ow = new OptionsWindow();
            ow.ShowDialog();
            SetColumnSettings();
            if (OptionsWindow.State == true)
            {
                CurrentModel.Status = "Program settings were updated";
            }
            else
            {
                CurrentModel.Status = "The update of the program settings was canceled";
            }
        }


        public void SetColumnSettings()
        {
            
            CurrentModel.ShowResourceKeyColumn = true;
            CurrentModel.ShowContentColumn = true;
            CurrentModel.ShowResourceCategoryColumn = false;
            CurrentModel.ShowCommentColumn = false;
            CurrentModel.ShowResourceCategoryColumn = false;
            CurrentModel.ShowCommentColumn = false;
            CurrentModel.ShowStreamNameColumn = false;
            CurrentModel.ShowReadableColumn = false;
            CurrentModel.ShowModifiableColumn = false;
            if (Properties.Settings.Default.ManyColumnsVisible == true)
            {
                CurrentModel.ShowResourceCategoryColumn = true;
                CurrentModel.ShowCommentColumn = true;
            }
            else if (Properties.Settings.Default.AllColumnsVisible == true)
            {
                CurrentModel.ShowResourceCategoryColumn = true;
                CurrentModel.ShowCommentColumn = true;
                CurrentModel.ShowStreamNameColumn = true;
                CurrentModel.ShowReadableColumn = true;
                CurrentModel.ShowModifiableColumn = true;
            }

            // Workaround since data binding is not possible directly
            CurrentModel.ColumnIsReadOnly = Properties.Settings.Default.SetColumnsReadOnly;
            this.streamNameColumn.IsReadOnly = CurrentModel.ColumnIsReadOnly;
            this.resourceKeyColumn.IsReadOnly = CurrentModel.ColumnIsReadOnly;
            this.resourceCategoryColumn.IsReadOnly = CurrentModel.ColumnIsReadOnly;
            this.readableColumn.IsReadOnly = CurrentModel.ColumnIsReadOnly;
            this.modifiableColumn.IsReadOnly = CurrentModel.ColumnIsReadOnly;
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select original resource library to translate";
            ofd.DefaultExt = "dll";
            ofd.Filter = "Dynamic link libraries|*.dll|Executables|*.exe|Resource files|*.resources|BAML files|*.baml|All files|*.*";
            ofd.Multiselect = false;
            bool? result = ofd.ShowDialog();
            if (result != true) { return; }

            FileInfo fi1 = new FileInfo(ofd.FileName);

            CommonOpenFileDialog ofd2 = new CommonOpenFileDialog();
            ofd2.Multiselect = false;
            ofd2.IsFolderPicker = true;
            ofd2.Title = "Save new resource library in folder...";
            CommonFileDialogResult result2 = ofd2.ShowDialog();
            if (result2 != CommonFileDialogResult.Ok) { return; }

            DirectoryInfo di1 = new DirectoryInfo(ofd2.FileName);

            if (fi1.Directory.FullName.Equals(di1.FullName))
            {
                this.CurrentModel.Status = "Save aborted";
                MessageBox.Show("The location of the existing resource library and the new one cannot be the same folder since both files have the same name.\nUse for instance a sub folder like \"" + cultureInfoBox.SelectedItem.ToString() + "\"", "Save aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
                this.handler = new LocBamlHandler(ofd.FileName, ofd2.FileName, CurrentModel, (CultureInfo)this.cultureInfoBox.SelectedItem);
                bool state = this.handler.Save();
                if (state == true)
                {
                    this.CurrentModel.Status = "The resources were saved successfully as " + this.handler.CurrentCultureInfo.ToString();
                    MessageBox.Show("The resources were imported saved with the culture info " + this.handler.CurrentCultureInfo.ToString(), "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
        }

        private void cultureInfoBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CurrentModel.HandleCultureInfo() == true)
            {
                bamlDataGrid.Items.Refresh();  // Force refresh
            }
           
        }

    }
}
