using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        public void SetLocalizationList(IList<LocalizationItem> items)
        { 
            this.localizationList.Clear();
            foreach (LocalizationItem item in items)
            {
                this.localizationList.Add(item);
            }

            if (localizationList.Count == 0)
            {
                this.Loaded = false;
            }
            else
            {
                this.Loaded = true;
            }
            UpdateList();
        }


        public ViewModel()
        {
            LocalizationList = new ObservableCollection<LocalizationItem>();
            this.Status = "ready...";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
