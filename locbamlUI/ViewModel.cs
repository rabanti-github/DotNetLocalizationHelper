using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace locbamlUI
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<LocalizationItem> localizationList;
        private string status;

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
