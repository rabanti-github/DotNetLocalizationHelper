//---------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// Description: LocBaml command line tool. 
//
//---------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows.Threading;

namespace BamlLocalization
{
    /// <summary>
    /// LocBaml tool: A command line tool to localize baml
    /// </summary>
    public static class LocBaml
    {
        private const int ErrorCode = 100;        
        private const int SuccessCode = 0;
        private static Dispatcher _dispatcher;

        //----------------------------------
        // Main
        //----------------------------------
        [System.STAThread()]
        public static int Main(string[] args)
        {
			//Debugger.Break();
        	
            LocBamlOptions options;
            string errorMessage;
            GetCommandLineOptions(args, out options, out errorMessage);

            if (errorMessage != null)
            {
                // there are errors                
                PrintLogo(options);
                Console.WriteLine(StringLoader.Get("ErrorMessage", errorMessage));                
                Console.WriteLine();
                PrintUsage();
                return ErrorCode;    // error
            }          

             // at this point, we obtain good options.
            if (options == null)            
            {
                // no option to process. Noop.
                return SuccessCode;
            }
            
            _dispatcher = Dispatcher.CurrentDispatcher;

            PrintLogo(options);

            try{
                // it is to parse
                if (options.ToParse)
                {
                    ParseBamlResources(options);
                }
                else if (options.ToParseAsStream)
                {
                    Stream ms = ParseBamlResourcesAsSteram(options);
                }
                else
                {
                    GenerateBamlResources(options);
                }
            }
            catch(Exception e)                
            {
#if DEBUG
                throw e;
#else
                Console.WriteLine(e.Message);
                return ErrorCode;            
#endif
            }

            return SuccessCode;
        
        }        

         #region Private static methods
        //---------------------------------------------
        // Private static methods
        //---------------------------------------------

        /// <summary>
        /// Parse the baml resources given in the command line
        /// </summary>        
        private static void ParseBamlResources(LocBamlOptions options)
        {            
            TranslationDictionariesWriter.Write(options);         
        }

        public static Stream ParseBamlResourcesAsSteram(LocBamlOptions options)
        {
           MemoryStream ms = new MemoryStream();
           TranslationDictionariesWriter.Write(options, ms);
           return ms;
        }


        /// <summary>
        /// Genereate localized baml 
        /// </summary>        
        private static void GenerateBamlResources(LocBamlOptions options)
        {   
            Stream input = File.OpenRead(options.Translations);
            using (ResourceTextReader reader = new ResourceTextReader(options.TranslationFileType, input))
            {   
                TranslationDictionariesReader dictionaries = new TranslationDictionariesReader(reader);                                                               
                ResourceGenerator.Generate(options, dictionaries);
            }         
        }
            
        /// <summary>
        /// get CommandLineOptions, return error message
        /// </summary>
        private static void GetCommandLineOptions(string[] args, out LocBamlOptions options, out string errorMessage)
        {
            CommandLine commandLine; 
            try{
                // "*" means the option must have a value. no "*" means the option can't have a value 
                 commandLine = new CommandLine(args, 
                                    new string[]{
                                            "parse",        // /parse for update
                                            "generate",     // /generate     for generate
                                            "*out",         // /out          for output .csv|.txt when parsing, for output directory when generating
                                            "*culture",     // /culture      for culture name
                                            "*translation", // /translation  for translation file, .csv|.txt
                                            "*asmpath",     // /asmpath,     for assembly path to look for references   (TODO: add asmpath support)
                                            "nologo",       // /nologo       for not to print logo      
                                            "help",         // /help         for help
                                            "verbose"       // /verbose      for verbose output         
                                        }                
                                     );
           }            
           catch (ArgumentException e)
           {
               errorMessage = e.Message;
               options      = null;
               return;
           }

            if (commandLine.NumArgs + commandLine.NumOpts < 1)
            {
                PrintLogo(null);
                PrintUsage();
                errorMessage    = null;
                options         = null;
                return;
            }

            options = new LocBamlOptions();

            options.Input    = commandLine.GetNextArg();

            Option commandLineOption;

            while ( (commandLineOption = commandLine.GetNextOption()) != null)
            {
                if (commandLineOption.Name      == "parse")
                {
                    options.ToParse = true;
                }
                else if (commandLineOption.Name == "generate")
                {
                    options.ToGenerate = true;
                }
                else if (commandLineOption.Name == "nologo")
                {
                    options.HasNoLogo = true;                        
                }
                else if (commandLineOption.Name == "help")
                {
                    // we print usage and stop processing
                    PrintUsage();
                    errorMessage = null;
                    options = null;
                    return;
                }
                else if (commandLineOption.Name == "verbose")
                {
                    options.IsVerbose = true;
                }
                    // the following ones need value
                else if (commandLineOption.Name == "out")
                {
                    options.Output = commandLineOption.Value;
                }
                else if (commandLineOption.Name == "translation")
                {
                    options.Translations = commandLineOption.Value;
                }
                else if (commandLineOption.Name == "asmpath")
                {
                    if (options.AssemblyPaths == null)
                    {
                        options.AssemblyPaths = new ArrayList();
                    }

                    options.AssemblyPaths.Add(commandLineOption.Value);
                }
                else if (commandLineOption.Name == "culture")
                {
                    try
                    {
                        options.CultureInfo = new CultureInfo(commandLineOption.Value);
                    }
                    catch (ArgumentException e)
                    {
                        // Error
                        errorMessage = e.Message;
                        return;
                    }
                }
                else
                {
                    // something that we don't recognize
                    errorMessage = StringLoader.Get("Err_InvalidOption", commandLineOption.Name);
                    return;
                }
            }

            // we passed all the test till here. Now check the combinations of the options
            errorMessage = options.CheckAndSetDefault();       
        }

        private static void PrintLogo(LocBamlOptions option)
        {
            if (option == null || !option.HasNoLogo)
            {               
                Console.WriteLine(StringLoader.Get("Msg_Copyright", GetAssemblyVersion()));
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine(StringLoader.Get("Msg_Usage"));
        }         


        private static string GetAssemblyVersion()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();                                   
            return currentAssembly.GetName().Version.ToString(4);
        }
        
         #endregion
    }




    #region LocBamlOptions
    // the class that groups all the baml options together

    #endregion    
}


