using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
