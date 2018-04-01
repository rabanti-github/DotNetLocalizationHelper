using System;

namespace locbamlUI.FemtoXLSX
{
    public class Cell
    {
        public enum DataType
        {
           String,
           Number,
           Boolean,
           Date,
           Formula,
           None
        }

        public int ColumnNumber { get; private set; }
        public int RowNumber { get; private set; }
        public string Address { get; private set; }
        public DataType Type { get; private set; }

        public object RawValue { get; private set; }

        public Cell(object value, DataType type, string address)
        {
            this.RawValue = value;
            this.Type = type;
            this.Address = address;
            PicoXLSX.Cell.Address a = new PicoXLSX.Cell.Address(address);
            this.ColumnNumber = a.Column;
            this.RowNumber = a.Row;
        }

        public string GetStringValue()
        {
            return this.RawValue.ToString();
        }

        public double GetNumericValue()
        {
            if (Type == DataType.Number)
            {
                return (double)this.RawValue;
            }
            else
            {
                return 0;
            }
        }

        public bool GetBoolValue()
        {
            if (Type == DataType.Boolean)
            {
                return (bool)this.RawValue;
            }
            else
            {
                return false;
            }
        }

        public DateTime GetDateValue()
        {
            if (Type == DataType.Date)
            {
                try
                {
                    DateTime t = DateTime.FromOADate((double)this.RawValue);
                    return t;
                }
                catch
                {
                    return new DateTime(0, 0, 0, 0, 0, 0);
                }
            }
            else
            {
                return new DateTime(0,0,0,0,0,0);
            }
        }

        public string GetFormula()
        {
            if (Type == DataType.Formula)
            {
                return this.RawValue.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        

    }
}
