using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace locbamlUI.FemtoXLSX
{
    public class SharedStringsReader
    {

        public List<string> SharedStrings { get; private set; }

        public bool HasElements
        {
            get
            {
                if (SharedStrings.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string GetString(int index)
        {
            if (HasElements == false || index > SharedStrings.Count - 1)
            {
                return null;
            }
            return SharedStrings[index];
        }


        public SharedStringsReader()
        {
            SharedStrings = new List<string>();
        }

        public void Read(Stream stream)
        {
            SharedStrings.Clear();
            XmlTextReader xr = new XmlTextReader(stream);
            bool isStringItem = false, isText = false;
            string item = "";
            string name;
            while (xr.Read())
            {
                name = xr.Name.ToLower();
                if (name == "si" && xr.IsStartElement() == true)
                {
                    isStringItem = true;
                    continue;
                }
                else if (name == "t" && isStringItem == true)
                {
                    isText = true;
                    continue;
                }
                else if (string.IsNullOrEmpty(name) && isText)
                {
                    item = xr.ReadString();
                    continue;
                }
                else if (name == "si" && isStringItem == true && isText == true)
                {
                    SharedStrings.Add(item);
                    item = "";
                    isText = false;
                    isStringItem = false;
                    continue;
                }
            }
            

        }


        
    }
}
