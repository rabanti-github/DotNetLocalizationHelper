using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace locbamlUI.FemtoXLSX
{
    public class WorksheetReader
    {
        public string Name { get; private set; }
        public int WorksheetNumber { get; set; }
        public Dictionary<string, Cell> Data { get; private set; }
        private SharedStringsReader sharedStrings;

        public WorksheetReader(SharedStringsReader sharedStrings, string name, int number)
        {
            Data = new Dictionary<string, Cell>();
            this.Name = name;
            this.WorksheetNumber = number;
            this.sharedStrings = sharedStrings;
        }


        public bool HasColumn(string columnAddress)
        {
            if (string.IsNullOrEmpty(columnAddress)) { return false; }
            int columnNumber = PicoXLSX.Cell.ResolveColumn(columnAddress);
            foreach (KeyValuePair<string, Cell> cell in Data)
            {
                if (cell.Value.ColumnNumber == columnNumber)
                {
                    return true;
                }
            }
            return false;
        }

        public bool RowHasColumns(ref List<Cell> cells, int[] columnNumbers)
        {
            if (columnNumbers == null || cells == null) { return false; }
            int len = columnNumbers.Length;
            int len2 = cells.Count;
            int j;
            bool match;
            if (len < 1 || len2 < 1){ return false; }
            for (int i = 0; i < len; i++)
            {
                match = false;
                for (j = 0; j < len2; j++)
                {
                    if (cells[j].ColumnNumber == columnNumbers[i])
                    {
                        match = true;
                        break;
                    }
                }
                if (match == false) { return false; }
            }
            return true;
        }

        public int GetRowCount()
        {
            int count = -1;
            foreach (KeyValuePair<string, Cell> cell in Data)
            {
                if (cell.Value.RowNumber > count)
                {
                    count = cell.Value.RowNumber;
                }
            }
            return count + 1;
        }

        public List<Cell> GetRow(int rowNumber)
        {
            List<Cell> list = new List<Cell>();
            foreach (KeyValuePair<string, Cell> cell in Data)
            {
                if (cell.Value.RowNumber == rowNumber)
                {
                    list.Add(cell.Value);
                }
            }

            list = list.OrderBy(o => o.ColumnNumber).ToList(); // Sort by column
            return list;
        }

        public void Read(Stream stream)
        {
            Data.Clear();
            XmlTextReader xr = new XmlTextReader(stream);
            bool isSheetData = false, isCell = false, isCellValue = false, isFormula = false;
            string value = "", formula = null;
            string type = "s";
            string style = "";
            string address = "A1";
            string name;
            while (xr.Read())
            {
                name = xr.Name.ToLower();
                if (name == "sheetdata")
                {
                    isSheetData = true;
                    continue;
                }
                else if (name == "c" && xr.IsStartElement() == true && isSheetData == true)
                {
                    address = xr.GetAttribute("r"); // mandatory
                    type = xr.GetAttribute("t"); // can be null if not existing
                    style = xr.GetAttribute("s"); // can be null; if "1" then date
                    isCell = true;
                    continue;
                }
                else if (name == "f" && isCell == true)
                {
                    isFormula = true;
                    continue;
                }
                else if (name == "v" && isCell == true)
                {
                    isCellValue = true;
                    continue;
                }
                else if (string.IsNullOrEmpty(name) && isFormula == true)
                {
                    formula = xr.ReadString();
                }
                else if (string.IsNullOrEmpty(name) && isCellValue == true)
                {
                    value = xr.ReadString();
                    continue;
                }
                else if (name == "c" && isCell == true && isCellValue == true)
                {
                    isCell = false;
                    isCellValue = false;
                    isFormula = false;
                    ResolveCellData(address, type, value, style, formula);
                    formula = null;
                    value = "";
                    continue;
                }
                else if (name == "sheetdata" && isSheetData == true)
                {
                    isSheetData = false;
                    continue;
                }
            }
        }

        private void ResolveCellData(string address, string type, string value, string style, string formula)
        {
            address = address.ToUpper();
            double d;
            bool b;
            int i;
            string s;
            Cell cell = null;
            if (style == "1") // Date must come before numeric
            {
                if (GetNumericValue(value, out d) == true)
                {
                    cell = new Cell(d, Cell.DataType.Date, address);
                }
                else
                {
                    cell = new Cell(value, Cell.DataType.String, address);
                }
            }
            else if (type == null) // try numeric
            {
                if (GetNumericValue(value, out d) == true)
                {
                    cell = new Cell(d, Cell.DataType.Number, address);
                }
                else
                {
                    cell = new Cell(value, Cell.DataType.String, address);
                }
            }
            else if (type == "b")
            {
                if (GetBooleanValue(value, out b) == true)
                {
                    cell = new Cell(b, Cell.DataType.Boolean, address);
                }
                else
                {
                    cell = new Cell(value, Cell.DataType.String, address);
                }
            }
            else if (formula != null)
            {
                cell = new Cell(formula, Cell.DataType.Formula, address);
            }
            else if (type == "s")
            {
                if (GetIntValue(value, out i) == false)
                {
                    cell = new Cell(value, Cell.DataType.String, address);
                }
                else
                {
                    s = sharedStrings.GetString(i);
                    if (s != null)
                    {
                        cell = new Cell(s, Cell.DataType.String, address);
                    }
                    else
                    {
                        cell = new Cell(value, Cell.DataType.String, address);
                    }
                }
            }
            else
            {
                cell = new Cell(value, Cell.DataType.String, address);
            }

            if (Data.ContainsKey(address))
            {
                Data[address] = cell;
            }
            else
            {
                Data.Add(address, cell);
            }
        }

        public static bool GetNumericValue(string raw, out double value)
        {
           return double.TryParse(raw, out value);
        }

        public static bool GetIntValue(string raw, out int value)
        {
            return int.TryParse(raw, out value);
        }

        public static bool GetBooleanValue(string raw, out bool value)
        {
            if (raw == "0")
            {
                value = false;
                return true;
            }
            else if (raw == "1")
            {
                value = true;
                return true;
            }
            else
            {
                return bool.TryParse(raw, out value);
            } 
        }


    }
}
