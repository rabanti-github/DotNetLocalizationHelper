using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SevenZipExtractor;

namespace locbamlUI.FemtoXLSX
{
    public class XlsxReader
    {
        public string FilePath { get; private set; }

        private SharedStringsReader sharedStrings;
        private Dictionary<int,WorksheetReader> worksheets;

        public XlsxReader(string path)
        {
            this.FilePath = path;
            this.worksheets = new Dictionary<int,WorksheetReader>();
        }

        public void Read()
        {
            try
            {
                using (FileStream fs = new FileStream(this.FilePath, FileMode.Open))
                {
                    MemoryStream ms;
                    WorksheetReader wr;
                    string name;
                    SevenZipExtractor.ArchiveFile archive = new ArchiveFile(fs, SevenZipFormat.Zip);
                    sharedStrings = new SharedStringsReader();
                    ms = GetStreamByNamePattern(ref archive, "sharedstrings.xml", false);
                    if (ms != null)
                    {
                       sharedStrings.Read(ms); 
                    }

                    int worksheetIndex = 1;
                    while (true)
                    {
                        name = "sheet" + worksheetIndex.ToString() + ".xml";
                        ms = GetStreamByNamePattern(ref archive, name, false);
                        if (ms != null)
                        {
                            wr = new WorksheetReader(sharedStrings,name, worksheetIndex);
                            wr.Read(ms);
                            this.worksheets.Add(worksheetIndex - 1, wr);
                        }
                        else
                        {
                            break;
                        }

                        worksheetIndex++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        public WorksheetReader GetWorksheet(int worksheetNumber)
        {
            if (worksheetNumber < 0 || worksheetNumber > this.worksheets.Count - 1)
            {
                return null;
            }
            return this.worksheets[worksheetNumber];
        }

        private MemoryStream GetStreamByNamePattern(ref ArchiveFile archive, string pattern, bool caseSensitive)
        {
            Regex rx;
            if (caseSensitive == true)
            {
                rx = new Regex(pattern);
            }
            else
            {
                rx = new Regex(pattern, RegexOptions.IgnoreCase);
            }

            Match match;
            for (int i = 0; i < archive.Entries.Count; i++)
            {
                if (archive.Entries[i].IsFolder == true) { continue; }
                match = rx.Match(archive.Entries[i].FileName);
                if (match.Success)
                {
                    MemoryStream ms = new MemoryStream();
                    archive.Entries[i].Extract(ms);
                    ms.Flush();
                    ms.Position = 0;
                    return ms;
                }
            }
            return null;
        }


    }
}
