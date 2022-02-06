using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WpfTestWork.Models
{
    class TorrentState : INotifyPropertyChanged
    {
        private string Name;

        public string MyName
        {
            get { return Name; }
            set
            {
                Name = value;
                NotifyPropertyChanged("MyName");
            }
        }

        public TorrentState(string name) {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

    }
}
