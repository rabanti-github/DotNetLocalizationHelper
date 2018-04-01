using System;
using System.Collections.Generic;
using System.IO;
using PicoXLSX;
using Csv;
using locbamlUI.FemtoXLSX;
using Cell = locbamlUI.FemtoXLSX.Cell;

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

        public static IList<LocalizationItem> ImportFromCsv(string fileName, out string errors)
        {
            errors = null;
            try
            {
                string rawText = "";
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    TextReader tr = new StreamReader(fs);
                    rawText = tr.ReadToEnd();
                    tr.Close();
                }
                CsvOptions options = new CsvOptions(){HeaderMode = HeaderMode.HeaderPresent, Separator = ','};
                IEnumerable<ICsvLine> lines = CsvReader.ReadFromText(rawText, options);
                List<LocalizationItem> items = new List<LocalizationItem>();
                LocalizationItem item;
                int i = 1;
                string temp;
                bool state, boolValue;
                foreach (var line in lines)
                {
                    if (line.ColumnCount < 7)
                    {
                        errors = "Row " + i.ToString() + " has too few values. 7 columns are expected.";
                        return null;
                    }
                    item = new LocalizationItem();
                    for (int j = 0; j < line.ColumnCount; j++)
                    {
                        if (j == 0)
                        {
                            item.StreamName = line[j];
                        }
                        else if (j == 1)
                        {
                            item.resourceKey = line[j];
                        }
                        else if (j == 2)
                        {
                            item.ResourceCategory = line[j];
                        }
                        else if (j == 3)
                        {
                            temp = line[j];
                            state = FemtoXLSX.WorksheetReader.GetBooleanValue(temp, out boolValue);
                            if (state == false)
                            {
                                errors = "The boolean value of column " + (j+1).ToString() + " in row " + i.ToString() + "could not be resolved";
                                return null;
                            }
                            else
                            {
                                item.IsReadable = boolValue;
                            }
                        }
                        else if (j == 4)
                        {
                            temp = line[j];
                            state = FemtoXLSX.WorksheetReader.GetBooleanValue(temp, out boolValue);
                            if (state == false)
                            {
                                errors = "The boolean value of column " + (j+1).ToString() + " in row " + i.ToString() + "could not be resolved";
                                return null;
                            }
                            else
                            {
                                item.IsModifieable = boolValue;
                            }
                        }
                        else if (j == 5)
                        {
                            item.comment = line[j];
                        }
                        else if (j == 6)
                        {
                            item.content = line[j];
                        }
                        
                    }
                    items.Add(item);
                    i++;
                }
                return items;
            }
            catch (Exception e)
            {
                errors = e.Message;
                return null;
            }

            
        }

        public static IList<LocalizationItem> ImportFromXlsx(string fileName, out string errors)
        {
            errors = null;
            FemtoXLSX.XlsxReader reader = new XlsxReader(fileName);
            try
            {
                reader.Read();
            }
            catch (Exception e)
            {
                errors = e.Message;
                return null;
            }
            
            WorksheetReader worksheet = reader.GetWorksheet(0);
            if (worksheet == null)
            {
                errors = "The worksheet could not be loaded";
                return null;
            }
            if (worksheet.HasColumn("G") == false)
            {
                errors = "Wrong number of columns. The worksheet must have at least 7 columns.";
                return null;
            }

            int rowCount = worksheet.GetRowCount();
            if (rowCount < 2)
            {
                errors = "No data found. The worksheet must have at least 2 rows (header + 1 data row).";
                return null;
            }
            Dictionary<string, Cell> data = worksheet.Data;
            List<LocalizationItem> items = new List<LocalizationItem>();
            LocalizationItem item;
            List<Cell> row;
            for (int i = 1; i < rowCount; i++) // Start with row 2( index 1)
            {
                row = worksheet.GetRow(i);
                if (row.Count < 7)
                {
                    errors = "Row " + (i + 1).ToString() + " has too few values. 7 columns are expected.";
                    return null;
                }
                item = new LocalizationItem();
                foreach (Cell cell in row)
                {
                    if (cell.ColumnNumber == 0)
                    {
                        item.StreamName = cell.GetStringValue();
                    }
                    else if (cell.ColumnNumber == 1)
                    {
                        item.ResourceKey = cell.GetStringValue();
                    }
                    else if (cell.ColumnNumber == 2)
                    {
                        item.ResourceCategory = cell.GetStringValue();
                    }
                    else if (cell.ColumnNumber == 3 && cell.Type == Cell.DataType.Boolean)
                    {
                        item.IsReadable = cell.GetBoolValue();
                    }
                    else if (cell.ColumnNumber == 4 && cell.Type == Cell.DataType.Boolean)
                    {
                        item.IsModifieable = cell.GetBoolValue();
                    }
                    else if (cell.ColumnNumber == 5)
                    {
                        item.Comment = cell.GetStringValue();
                    }
                    else if (cell.ColumnNumber == 6)
                    {
                        item.Content = cell.GetStringValue();
                    }
                    else if (cell.ColumnNumber < 7) // Error fall-back; higher numbers are ignored
                    {
                        errors = "Unexpected data occurred. Check the columns A to G in Row " + (i + 1).ToString();
                        return null;
                    }
                }
                items.Add(item);
            }
            return items;
        }
        

    }
}
