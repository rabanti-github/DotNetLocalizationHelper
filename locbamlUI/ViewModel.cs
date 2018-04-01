using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace locbamlUI
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<LocalizationItem> localizationList;
        private string status;
        private bool loaded;
        private bool showStreamNameColumn = true; // Default
        private bool showResourceKeyColumn = true;
        private bool showResourceCategoryColumn = true;
        private bool showReadableColumn = true;
        private bool showModifiableColumn = true;
        private bool showCommentColumn = true;
        private bool showContentColumn = true;
        private bool columnIsReadOnly = true;
        private List<CultureInfo> cultureInfoList;
        private string assembly;
        private int cultureInfoIndex = 0;

        public int CultureInfoIndex
        {
            get { return cultureInfoIndex; }
            set
            {
                cultureInfoIndex = value;
                NotifyPropertyChanged("CultureInfoIndex");
            }
        }

        public string Assembly
        {
            get { return assembly; }
            set
            {
                assembly = value;
                NotifyPropertyChanged("Assembly");
            }
        }

        public List<CultureInfo> CultureInfoList
        {
            get { return cultureInfoList; }
            set
            {
                cultureInfoList = value;
                NotifyPropertyChanged("CultureInfoList");
            }
        }


        public bool ColumnIsReadOnly
        {
            get { return columnIsReadOnly; }
            set
            {
                columnIsReadOnly = value;
                NotifyPropertyChanged("ColumnIsReadOnly");
            }
        }


        public bool ShowStreamNameColumn
        {
            get { return showStreamNameColumn; }
            set
            {
                showStreamNameColumn = value;
                NotifyPropertyChanged("ShowStreamNameColumn");
            }
        }

        public bool ShowResourceKeyColumn
        {
            get { return showResourceKeyColumn; }
            set
            {
                showResourceKeyColumn = value;
                NotifyPropertyChanged("ShowResourceKeyColumn");
            }
        }

        public bool ShowResourceCategoryColumn
        {
            get { return showResourceCategoryColumn; }
            set
            {
                showResourceCategoryColumn = value;
                NotifyPropertyChanged("ShowResourceCategoryColumn");
            }
        }

        public bool ShowReadableColumn
        {
            get { return showReadableColumn; }
            set
            {
                showReadableColumn = value;
                NotifyPropertyChanged("ShowReadableColumn");
            }
        }

        public bool ShowModifiableColumn
        {
            get { return showModifiableColumn; }
            set
            {
                showModifiableColumn = value;
                NotifyPropertyChanged("ShowModifiableColumn");
            }
        }

        public bool ShowCommentColumn
        {
            get { return showCommentColumn; }
            set
            {
                showCommentColumn = value;
                NotifyPropertyChanged("ShowCommentColumn");
            }
        }

        public bool ShowContentColumn
        {
            get { return showContentColumn; }
            set
            {
                showContentColumn = value;
                NotifyPropertyChanged("ShowContentColumn");
            }
        }


        public bool Loaded
        {
            get { return loaded; }
            set
            {
                loaded = value;
                NotifyPropertyChanged("Loaded");
            }
        }

        public ObservableCollection<LocalizationItem> LocalizationList
        {
            get { return localizationList; }
            set
            {
                localizationList = value;
                NotifyPropertyChanged("LocalizationList");
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public void UpdateList()
        {
            NotifyPropertyChanged("LocalizationList");
        }

        public void ClearLocalizationList()
        {
            this.localizationList.Clear();
            UpdateList();
        }

        public void SetLocalizationList(IList<LocalizationItem> items)
        {
            this.Loaded = false; // Disable everything to prevent property change loops when auto-handling the culture info
            this.localizationList.Clear();
            foreach (LocalizationItem item in items)
            {
                this.localizationList.Add(item);
            }

            if (localizationList.Count == 0)
            {
                this.Loaded = false;
                this.Assembly = "";
                this.CultureInfoIndex = 0;
            }
            else
            {
                this.Loaded = true;
                Regex rx = new Regex(@"(.+)\.[\w]+\.([\w\-]{5,15})(\..*)");
                Match match = rx.Match(localizationList[0].StreamName);
                if (match.Groups.Count >= 4)
                {
                    Assembly = match.Groups[1].Value;
                    string ci = match.Groups[2].Value;
                    bool ciMatch = false;
                    if (Properties.Settings.Default.CultureInfoAutoHandling == true)
                    {
                        for (var i = 0; i < cultureInfoList.Count; i++)
                        {
                            if (cultureInfoList[i].ToString() == ci)
                            {
                                ciMatch = true;
                                CultureInfoIndex = i;
                                break;
                            }
                        }

                        if (ciMatch == false)
                        {
                            CultureInfoIndex = 0;
                        }
                    }
                }
                else
                {
                    Assembly = "";
                    CultureInfoIndex = 0;
                }

            }
            UpdateList();


        }

        public bool HandleCultureInfo()
        {
            if (Loaded == false) { return false; }
            if (Properties.Settings.Default.CultureInfoAutoHandling == false) { return false; }
            Regex rx = new Regex(@"(.+)(\.[\w]+\.)([\w\-]{5,15})(\..*)");
            Match match;
            string temp;
            string locale = cultureInfoList[cultureInfoIndex].ToString();
            for (int i = 0; i < localizationList.Count; i++)
            {
                match = rx.Match(localizationList[i].StreamName);
                if (match.Groups.Count >= 5)
                {
                    temp = match.Groups[1].Value + match.Groups[2].Value + locale + match.Groups[4].Value;
                    localizationList[i].StreamName = temp;
                }
            }
            UpdateList();
            return true;
        } 


        public ViewModel()
        {
            LocalizationList = new ObservableCollection<LocalizationItem>();
            cultureInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();
            cultureInfoList.Sort((c1,c2) => c1.Name.CompareTo(c2.Name));
            CultureInfo ci = CultureInfo.InvariantCulture;
            cultureInfoList.Insert(0,ci);
            this.Status = "ready...";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
