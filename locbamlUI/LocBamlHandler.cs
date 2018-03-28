using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using BamlLocalization;

namespace locbamlUI
{
    public class LocBamlHandler
    {

        private static string CurrentAssembly;
        public static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/bc50dec8-3bb8-46e0-92b5-c1710429b263/locbaml-filenotfoundexception?forum=wpf
            return System.Reflection.Assembly.LoadFrom(CurrentAssembly);
        }

        public ViewModel CurrentViewModel { get; set; }
        public string Path { get; private set; }  


        public LocBamlHandler(ViewModel model)
        {
            this.CurrentViewModel = model;
        }

        public LocBamlHandler(string path, ViewModel model) : this(model)
        {
            this.Path = path;
            CurrentAssembly = path;
        }


        public void Load(string path)
        {
            this.Path = path;
            CurrentAssembly = path;
            Load();
        }

        public bool Load()
        {
            LocBamlOptions options = new LocBamlOptions();
            options.Input = this.Path;
            options.ToParseAsStream = true;
            options.Output = "res.txt"; // Transient
            string errors = options.CheckAndSetDefault();
            if (string.IsNullOrEmpty(errors) == false)
            {
                CurrentViewModel.Loaded = false;
                CurrentViewModel.Status = errors;
                MessageBox.Show(errors, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            Stream stream = LocBaml.ParseBamlResourcesAsSteram(options, out errors);
            if (string.IsNullOrEmpty(errors) == false)
            {
                CurrentViewModel.Loaded = false;
                CurrentViewModel.Status = errors;
                MessageBox.Show(errors, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            CurrentViewModel.Loaded = true;
            SetTable((MemoryStream)stream, '\t');
            return true;
        }

        private void SetTable(MemoryStream input, char cellDelimeiter)
        {
            this.CurrentViewModel.LocalizationList.Clear();
            LocalizationItem item;
            string text = "";
            using (StreamReader sr = new StreamReader(input))
            {
                text = sr.ReadToEnd();
            }
            input.Close();
            string[] cells;
            string[] lines = text.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            char[] delimiters = new char[]{ cellDelimeiter };
            bool bValue, check;
            foreach (string line in lines)
            {
                item = new LocalizationItem();
                cells = line.Split(delimiters, StringSplitOptions.None);
                for (int i = 0; i < cells.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            item.StreamName = cells[i];
                            break;
                        case 1:
                            item.ResourceKey = cells[i];
                            break;
                        case 2:
                            item.ResourceCategory = cells[i];
                            break;
                        case 3:
                            if (bool.TryParse(cells[i], out bValue) == true)
                            {
                                item.IsReadable = bValue;
                            }
                            else
                            {
                                item.IsReadable = false;
                            }
                            break;
                        case 4:
                            if (bool.TryParse(cells[i], out bValue) == true)
                            {
                                item.IsModifieable = bValue;
                            }
                            else
                            {
                                item.IsModifieable = false;
                            }
                            break;
                        case 5:
                            item.Comment = cells[i];
                            break;
                        case 6:
                            item.Content = cells[i];
                            break;
                    }
                }
                this.CurrentViewModel.LocalizationList.Add(item);
            }
            this.CurrentViewModel.UpdateList();
            this.CurrentViewModel.Status = "Resource was loaded";
        }

    }
}
