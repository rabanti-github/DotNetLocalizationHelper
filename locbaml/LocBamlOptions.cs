using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Xml.Serialization;

namespace BamlLocalization
{
    public sealed class LocBamlOptions
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string Translations { get; set; }
        public bool ToParse { get; set; }
        public bool ToParseAsStream { get; set; }
        public bool ToGenerate { get; set; }
        public bool HasNoLogo { get; set; }
        public bool IsVerbose { get; set; }
        public FileType TranslationFileType { get; set; }
        public FileType InputType { get; set; }
        public ArrayList AssemblyPaths { get; set; }
        public Assembly[] Assemblies { get; set; }

        /// <summary>
        /// return true if the operation succeeded.
        /// otherwise, return false
        /// </summary>
        public  string  CheckAndSetDefault()
        {
            // we validate the options here and also set default
            // if we can

            // Rule #1: One and only one action at a time
            // i.e. Can't parse and generate at the same time
            //      Must do one of them
            if (((ToParse || ToParseAsStream) && ToGenerate) || (!ToParse && !ToParseAsStream && !ToGenerate))
                return StringLoader.Get("MustChooseOneAction");

            // Rule #2: Must have an input 
            if (string.IsNullOrEmpty(Input))
            {
                return StringLoader.Get("InputFileRequired");
            }
            else
            {                
                if (!File.Exists(Input))
                {
                    return StringLoader.Get("FileNotFound", Input);
                }

                string extension = Path.GetExtension(Input);
             
                // Get the input file type.
                if (string.Compare(extension, "." + FileType.BAML.ToString(), true, CultureInfo.InvariantCulture) == 0)
                {
                    InputType = FileType.BAML;
                }
                else if (string.Compare(extension, "." + FileType.RESOURCES.ToString(), true, CultureInfo.InvariantCulture) == 0)
                {
                    InputType = FileType.RESOURCES;                    
                }
                else if (string.Compare(extension, "." + FileType.DLL.ToString(), true, CultureInfo.InvariantCulture) == 0)
                {
                    InputType = FileType.DLL;
                }
                else if (string.Compare(extension, "." + FileType.EXE.ToString(), true, CultureInfo.InvariantCulture) == 0)
                {
                    InputType = FileType.EXE;
                }
                else
                {
                    return StringLoader.Get("FileTypeNotSupported", extension);
                }                                
            }
            
            if (ToGenerate)
            {
                // Rule #3: before generation, we must have Culture string
                if (CultureInfo == null &&  InputType != FileType.BAML)
                {
                    // if we are not generating baml, 
                    return StringLoader.Get("CultureNameNeeded", InputType.ToString());
                }
                
                // Rule #4: before generation, we must have translation file
                if (string.IsNullOrEmpty(Translations))
                {

                    return StringLoader.Get("TranslationNeeded");
                }
                else
                {
                    string extension = Path.GetExtension(Translations);

                    if (!File.Exists(Translations))
                    {
                        return StringLoader.Get("TranslationNotFound", Translations);
                    }
                    else
                    {
                        if (string.Compare(extension, "." + FileType.CSV.ToString(), true, CultureInfo.InvariantCulture) == 0)
                        {
                            TranslationFileType = FileType.CSV;
                        }
                        else 
                        {
                            TranslationFileType = FileType.TXT;
                        }
                    }
                }
            }

            

            // Rule #5: If the output file name is empty, we act accordingly
            if (string.IsNullOrEmpty(Output))
            {
                // Rule #5.1: If it is parse, we default to [input file name].csv
                if (ToParse || ToParseAsStream) // As stream will ignore output
                {
                    string fileName = Path.GetFileNameWithoutExtension(Input);                    
                    Output = fileName + "." + FileType.CSV.ToString();
                    TranslationFileType = FileType.CSV;
                }
                else  
                {
                    // Rule #5.2: If it is generating, and the output can't be empty
                    return StringLoader.Get("OutputDirectoryNeeded");
                }
                
            }else
            {                
                // output isn't null, we will determine the output file type                
                // Rule #6: if it is parsing. It will be .csv or .txt.
                if (ToParse || ToParseAsStream)  // As stream will ignore all output parameters
                {
                    string fileName;
                    string outputDir;

                    if (Directory.Exists(Output))
                    {
                        // the output is actually a directory name
                        fileName = string.Empty;
                        outputDir = Output;
                    }
                    else
                    {
                        // get the extension
                        fileName = Path.GetFileName(Output);
                        outputDir = Path.GetDirectoryName(Output);
                    }

                    // Rule #6.1: if it is just the output directory
                    // we append the input file name as the output + csv as default
                    if (string.IsNullOrEmpty(fileName))
                    {
                        TranslationFileType = FileType.CSV;
                        Output = outputDir  
                                 + Path.DirectorySeparatorChar 
                                 + Path.GetFileName(Input) 
                                 + "." 
                                 + TranslationFileType.ToString();
                    }
                    else
                    {
                        // Rule #6.2: if we have file name, check the extension.
                        string extension = Path.GetExtension(Output);

                        // ignore case and invariant culture
                        if (string.Compare(extension, "." + FileType.CSV.ToString(), true, CultureInfo.InvariantCulture) == 0)
                        {
                            TranslationFileType = FileType.CSV;
                        }
                        else
                        {
                            // just consider the output as txt format if it doesn't have .csv extension
                            TranslationFileType = FileType.TXT;
                        }
                    }                    
                }
                else
                {
                    // it is to generate. And output should point to the directory name.                    
                    if (!Directory.Exists(Output))
                        return StringLoader.Get("OutputDirectoryError", Output);
                }
            }

            // Rule #7: if the input assembly path is not null
            if (AssemblyPaths != null && AssemblyPaths.Count > 0)
            {
                Assemblies = new Assembly[AssemblyPaths.Count];
                for (int i = 0; i < Assemblies.Length; i++)
                {
                    string errorMsg = null;
                    try
                    {
                        // load the assembly
                        Assemblies[i] = Assembly.LoadFrom((string) AssemblyPaths[i]);
                    }
                    catch (ArgumentException argumentError)
                    {
                        errorMsg = argumentError.Message;   
                    }
                    catch (BadImageFormatException formatError)
                    {
                        errorMsg = formatError.Message;   
                    }
                    catch (FileNotFoundException fileError)
                    {
                        errorMsg = fileError.Message;
                    }
                    catch (PathTooLongException pathError)
                    {
                        errorMsg = pathError.Message;
                    }
                    catch (SecurityException securityError)
                    {

                        errorMsg = securityError.Message;
                    }

                    if (errorMsg != null)
                    {
                        return errorMsg; // return error message when loading this assembly
                    }
                }               
            }

            // if we come to this point, we are all fine, return null error message
            return null;
        }


        /// <summary>
        /// Write message line depending on isVerbose flag
        /// </summary>
        internal void WriteLine(string message)
        {
            if (IsVerbose)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Write the message depending on isVerbose flag
        /// </summary>        
        internal void Write(string message)
        {
            if (IsVerbose)
            {
                Console.Write(message);
            }
        }
    }
}