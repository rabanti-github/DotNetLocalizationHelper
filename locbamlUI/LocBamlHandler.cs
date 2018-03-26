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


        public string Path { get; private set; }  
        public LocBamlHandler(string path)
        {
            this.Path = path;
            CurrentAssembly = path;
        }



        public void Load()
        {
            LocBamlOptions options = new LocBamlOptions();
            options.Input = this.Path;
            options.ToParseAsStream = true;
            string errors = options.CheckAndSetDefault();

            Stream stream = LocBaml.ParseBamlResourcesAsSteram(options);

            // Test
            FileStream fs = new FileStream("test.csv", FileMode.Create);
            stream.CopyTo(fs);
            fs.Flush();
            fs.Close();
            

        }

    }
}
