using System;
using System.IO;
using System.Reflection;
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

        public void Load()
        {
            LocBamlOptions options = new LocBamlOptions();
            options.Input = this.Path;
            options.ToParseAsStream = true;
            options.Output = "res.txt"; // Transient
            string errors = options.CheckAndSetDefault();

            Stream stream = LocBaml.ParseBamlResourcesAsSteram(options);

            SetTable(stream, '\t');
            // Test
            /*
            FileStream fs = new FileStream("test.csv", FileMode.Create);
            stream.CopyTo(fs);
            fs.Flush();
            fs.Close();
             * */
            

        }

        private void SetTable(Stream input, char cellDelimeiter)
        {
            this.CurrentViewModel.LocalizationList.Clear();
            LocalizationItem item;
            StreamReader sr = new StreamReader(input);
            string text = sr.ReadToEnd();
            sr.Close();
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
        }

    }
}
