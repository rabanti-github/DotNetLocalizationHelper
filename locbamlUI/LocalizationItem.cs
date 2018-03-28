using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using PicoXLSX;
using Csv;

namespace locbamlUI
{
    public class LocalizationItem
    {
        private string streamName;
        private string resourceKey;
        private string resourceCategory;
        private bool isReadable;
        private bool isModifieable;
        private string comment;
        private string content;

        public string StreamName
        {
            get { return streamName; }
            set { streamName = value; }
        }

        public string ResourceKey
        {
            get { return resourceKey; }
            set { resourceKey = value; }
        }

        public string ResourceCategory
        {
            get { return resourceCategory; }
            set { resourceCategory = value; }
        }

        public bool IsReadable
        {
            get { return isReadable; }
            set { isReadable = value; }
        }

        public bool IsModifieable
        {
            get { return isModifieable; }
            set { isModifieable = value; }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public static string ExportAsCsv(string fileName, IList<LocalizationItem> items)
        {
            string[] line;
            List<string[]> lines = new List<string[]>();
            foreach (LocalizationItem item in items)
            {
                line = new string[7];
                line[0] = item.StreamName;
                line[1] = item.ResourceKey;
                line[2] = item.ResourceCategory;
                line[3] = item.IsReadable.ToString();
                line[4] = item.IsModifieable.ToString();
                line[5] = item.Comment;
                line[6] = item.Content;
                lines.Add(line);
            }
            string[] header = new string[7];
            header[0] = "Stream name";
            header[1] = "Resource key";
            header[2] = "Resource Category";
            header[3] = "Can be read";
            header[4] = "Can be modified";
            header[5] = "Comment";
            header[6] = "Value";
            try
            {
                string csv = CsvWriter.WriteToText(header, lines);
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write(csv);
                    sw.Flush();
                }

                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public static string ExportAsXlsx(string fileName, IList<LocalizationItem> items)
        {
            PicoXLSX.Workbook wb = new Workbook(fileName, "resources");
            wb.CurrentWorksheet.CurrentCellDirection = Worksheet.CellDirection.ColumnToColumn;
            Style s = Style.BasicStyles.Bold;
            wb.WS.Value("Stream name", s);
            wb.WS.Value("Resource key", s);
            wb.WS.Value("Resource Category", s);
            wb.WS.Value("Can be read", s);
            wb.WS.Value("Can be modified", s);
            wb.WS.Value("Comment", s);
            wb.WS.Value("Value", s);
            wb.WS.Down();
            foreach (LocalizationItem item in items)
            {
                wb.WS.Value(item.StreamName);
                wb.WS.Value(item.ResourceKey);
                wb.WS.Value(item.ResourceCategory);
                wb.WS.Value(item.IsReadable);
                wb.WS.Value(item.IsModifieable);
                wb.WS.Value(item.Comment);
                wb.WS.Value(item.Content);
                wb.WS.Down();
            }

            try
            {
                wb.Save();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }
        

    }
}
