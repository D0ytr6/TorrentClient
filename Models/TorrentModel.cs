using MonoTorrent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WpfTestWork.Models
{
    class TorrentModel : INotifyPropertyChanged
    {

        private string Name;

        public string MyName
        {
            get { return Name; }
            set { Name = value; }
        }


        private string Speed;

        public string MySpeed
        {
            get { return Speed; }
            set { Speed = value;
                NotifyPropertyChanged("MySpeed");
            }
        }

        private string Size;

        public string MySize
        {
            get { return Size; }
            set
            {
                Size = value;
                NotifyPropertyChanged("MySize");
            }
        }

        private string SpeedUpload;

        public string MySpeedUpload
        {
            get { return SpeedUpload; }
            set
            {
                SpeedUpload = value;
                NotifyPropertyChanged("MySpeedUpload");
            }
        }

        private string State;

        public string MyState
        {
            get { return State; }
            set
            {
                State = value;
                NotifyPropertyChanged("MyState");
            }
        }

        private int PercentageDownloaded;

        public int MyPercentageDownloaded
        {
            get { return PercentageDownloaded; }
            set { PercentageDownloaded = value;
                NotifyPropertyChanged("MyPercentageDownloaded");
            }
        }


        private Torrent TorrentFile;

        public Torrent MyTorrentFile
        {
            get { return TorrentFile; }
            set
            {
                TorrentFile = value;
                NotifyPropertyChanged("MyTorrentFile");
            }
        }

        private string TorrentPath;

        public string MyTorrentPath
        {
            get { return TorrentPath; }
            set
            {
                TorrentPath = value;
                NotifyPropertyChanged("MyTorrentFile");
            }
        }


        public TorrentModel(string name, string size, int Percent, Torrent file, string path)
        {
            Name = name;
            MySize = size;
            PercentageDownloaded = Percent;
            MyTorrentFile = file;
            MyTorrentPath = path;
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
