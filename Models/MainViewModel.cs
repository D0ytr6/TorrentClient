using DropDownMenu;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MonoTorrent;
using MonoTorrent.Client;
using Prism.Commands;
using SampleClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfTestWork.Commands;
using WpfTestWork.Download;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WpfTestWork.Models
{
    class MainViewModel
    {
        public ObservableCollection<TorrentModel> TorrentsDataGrid { get; set; } = new ObservableCollection<TorrentModel>();
        public TorrentModel SelectedTorrent { get; set; }
        public List<string> Torrentfiles = new List<string>();

        public List<SubItem> AllTorrentListUI = new List<SubItem>();
        public List<SubItem> DownloadsListUI = new List<SubItem>();
        public List<SubItem> StopListUI = new List<SubItem>();
        public List<SubItem> FinishListUI = new List<SubItem>();

        public ObservableCollection<TorrentState> Torrentsdownloads { get; set; } = new ObservableCollection<TorrentState>();
        public ObservableCollection<TorrentState> TorrentStopped { get; set; } = new ObservableCollection<TorrentState>();

        public ObservableCollection<UserControlMenuItem> AllTorrentsMenuUI { get; set; } = new ObservableCollection<UserControlMenuItem>();
        public ObservableCollection<UserControlMenuItem> TorrentsdownloadsMenuUI { get; set; } = new ObservableCollection<UserControlMenuItem>();
        public ObservableCollection<UserControlMenuItem> TorrentsStopedMenuUI { get; set; } = new ObservableCollection<UserControlMenuItem>();
        public ObservableCollection<UserControlMenuItem> TorrentsDelMenuUI { get; set; } = new ObservableCollection<UserControlMenuItem>();

        public ICommand CommandOpenTorrent { get; set; }
        public ICommand CommandShutdown { get; set; }
        public ICommand CommandDownload { get; set; }
        public ICommand GetRowInfoCommandStop { get; set; }
        public ICommand GetRowInfoCommandDel { get; set; }

        public MainViewModel()
        {
            CommandOpenTorrent = new RelayCommand(OpentTorrentFile);
            CommandDownload = new RelayCommand(StartDwn);
            GetRowInfoCommandStop = new RelayCommand(GetRowInfoStop);
            GetRowInfoCommandDel = new RelayCommand(GetRowInfoDel);
            CommandShutdown = new RelayCommand(shutdown);

            var item6 = new ItemMenu("All Torrents", AllTorrentListUI, PackIconKind.Register);
            UserControlMenuItem itemreg = new UserControlMenuItem(item6);
            AllTorrentsMenuUI.Add(itemreg);

            var item5 = new ItemMenu("Downloading", DownloadsListUI, PackIconKind.Register);
            UserControlMenuItem itemreg2 = new UserControlMenuItem(item5);
            TorrentsdownloadsMenuUI.Add(itemreg2);

            var item4 = new ItemMenu("Stopped", StopListUI, PackIconKind.Register);
            UserControlMenuItem itemreg3 = new UserControlMenuItem(item4);
            TorrentsStopedMenuUI.Add(itemreg3);

            /*var item3 = new ItemMenu("Finished", FinishListUI, PackIconKind.Register);
            UserControlMenuItem itemreg4 = new UserControlMenuItem(item3);
            TorrentsDelMenuUI.Add(itemreg4);*/

        }

        private void shutdown() {
            Application.Current.Shutdown();
        }

        private void OpentTorrentFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".torrent"; // Default file extension
            dlg.Filter = "Torrent documents (.torrent)|*.torrent"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                foreach(string i in Torrentfiles) {
                    if (i == filename) {
                        MessageBox.Show("Файл уже добавлен");
                        return;
                    }
                }
                string folderPath = "";
                OpenFileDialog folderBrowser = new OpenFileDialog();
                // Set validate names and check file exists to false otherwise windows will
                // not let you select "Folder Selection."
                folderBrowser.ValidateNames = false;
                folderBrowser.CheckFileExists = false;
                folderBrowser.CheckPathExists = true;
                // Always default to Folder Selection.
                folderBrowser.FileName = "Folder Selection.";
                if (folderBrowser.ShowDialog() == true)
                {
                    folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                }

                Torrentfiles.Add(filename);
                TorrentAddEngine.AddTorrent(filename, TorrentsDataGrid, AllTorrentListUI, folderPath);
                //string fileText = System.IO.File.ReadAllText(filename);
                MessageBox.Show("Файл открыт");
            }

        }

        private async Task StartDownloadAsync(TorrentModel SelectedTorrent)
        {
            await Task.Run(() => TorrentStartEngine.TorrentRun(SelectedTorrent, TorrentAddEngine.arrayTorrentfiles[SelectedTorrent.MyName], SelectedTorrent.MyTorrentPath));

        }

        private async void StartDwn()
        {
            if (SelectedTorrent != null)
            {
   
                DeleteFromMenuUI(StopListUI, SelectedTorrent, TorrentsStopedMenuUI, "Stopped");

                DownloadsListUI.Add(new SubItem(SelectedTorrent.MyName));
                var item6 = new ItemMenu("Downloading", DownloadsListUI, PackIconKind.Register);
                UserControlMenuItem itemreg = new UserControlMenuItem(item6);
                TorrentsdownloadsMenuUI[0] = itemreg;

                if (TorrentAddEngine.arrayTorrenStopControl[SelectedTorrent.MyName] == true)
                {
                    TorrentAddEngine.arrayTorrenStopControl[SelectedTorrent.MyName] = false;
                }
                else
                {
                    TorrentAddEngine.arrayTorrenStopControl[SelectedTorrent.MyName] = false;
                    TorrentState current_torrent = new TorrentState(SelectedTorrent.MyName);
                    Torrentsdownloads.Add(current_torrent);
                    await StartDownloadAsync(SelectedTorrent);
                    if (SelectedTorrent.MyState != "Stopped")
                    {
                        DeleteTorrent(SelectedTorrent);

                    }
                }
            }
        }
        private void GetRowInfoStop()
        {
            if (SelectedTorrent != null)
            {
                foreach (TorrentState tr in Torrentsdownloads)
                {
                    if (tr.MyName == SelectedTorrent.MyName)
                    {
                        Torrentsdownloads.Remove(tr);
                        break;
                    }
                }

                DeleteFromMenuUI(DownloadsListUI, SelectedTorrent, TorrentsdownloadsMenuUI, "Downloading");

                StopListUI.Add(new SubItem(SelectedTorrent.MyName));
                var itemstop = new ItemMenu("Stopped", StopListUI, PackIconKind.Register);
                UserControlMenuItem contrItemStop = new UserControlMenuItem(itemstop);
                TorrentsStopedMenuUI[0] = contrItemStop;

                TorrentState current_torrent = new TorrentState(SelectedTorrent.MyName);
                TorrentStopped.Add(current_torrent);
                TorrentAddEngine.arrayTorrenStopControl[SelectedTorrent.MyName] = true;

            }
        }

        private void DeleteTorrent(TorrentModel SelectedTorrent)
        {
            TorrentAddEngine.arrayTorrentfiles.Remove(SelectedTorrent.MyName);
            TorrentAddEngine.arrayTorrenStopControl.Remove(SelectedTorrent.MyName);
            TorrentAddEngine.arrayTorrentDeleteControl.Remove(SelectedTorrent.MyName);
            TorrentAddEngine.arrayTorrentModels.Remove(SelectedTorrent.MyName);
            TorrentsDataGrid.Remove(SelectedTorrent);
            SelectedTorrent = null;

        }

        private void DeleteFromMenuUI(List<SubItem> ListUI, TorrentModel SelectedTorrent, ObservableCollection<UserControlMenuItem> MenuUI, string text) {
            List<SubItem> templist = new List<SubItem>();
            SubItem DelItem = new SubItem("item");
            foreach (SubItem i in ListUI)
            {
                if (i.Name != SelectedTorrent.MyName)
                {
                    //AllTorrentList.Remove(i);
                    templist.Add(i);
                }
                else
                    DelItem = i;
            }
            var item6 = new ItemMenu(text, templist, PackIconKind.Register);
            UserControlMenuItem itemreg = new UserControlMenuItem(item6);
            MenuUI[0] = itemreg;
            ListUI.Remove(DelItem);


        }


        private async void GetRowInfoDel()
        {
            if (SelectedTorrent != null)
            {
                DeleteFromMenuUI(AllTorrentListUI, SelectedTorrent, AllTorrentsMenuUI, "All Torrents");
                var item2 = new ItemMenu("All Torrents", AllTorrentListUI, PackIconKind.Register);
                UserControlMenuItem itemreg2 = new UserControlMenuItem(item2);
                AllTorrentsMenuUI[0] = itemreg2;

                DeleteFromMenuUI(DownloadsListUI, SelectedTorrent, TorrentsdownloadsMenuUI, "Downloading");
                DeleteFromMenuUI(StopListUI, SelectedTorrent, TorrentsStopedMenuUI, "Stopped");

                MessageBox.Show($"Имя: {SelectedTorrent.MyName}\nФамилия: {SelectedTorrent.MyPercentageDownloaded}");
                if (SelectedTorrent.MyState == null || SelectedTorrent.MyState == "Deleted")
                {
                    DeleteTorrent(SelectedTorrent);
                }
                else
                {
                    TorrentAddEngine.arrayTorrentDeleteControl[SelectedTorrent.MyName] = true;
                }

            }
        }

    }

}
