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

        public ObservableCollection<LocalizationItem> LocalizationList
        {
            get { return localizationList; }
            set
            {
                localizationList = value;
                NotifyPropertyChanged("LocalizationList");
            }
        }

        public void UpdateList()
        {
            NotifyPropertyChanged("LocalizationList");
        }


        public ViewModel()
        {
            LocalizationList = new ObservableCollection<LocalizationItem>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
