using MonoTorrent;
using MonoTorrent.Client;
using SampleClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfTestWork.Models;

namespace WpfTestWork.Download
{
    class TorrentAddEngine
    {

        public static Dictionary<String, Torrent> arrayTorrentfiles = new Dictionary<String, Torrent>();
        public static Dictionary<String, bool> arrayTorrenStopControl = new Dictionary<String, bool>();
        public static Dictionary<String, TorrentModel> arrayTorrentModels = new Dictionary<String, TorrentModel>();
        public static Dictionary<String, bool> arrayTorrentDeleteControl = new Dictionary<String, bool>();

        public static async void AddTorrent(string filename, ObservableCollection<TorrentModel> TorrentsDataGrid, List<SubItem> AllTorrentListUI, string path)
        {
            var torrentsPath = filename;
            if (filename.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase))
            {
                var torrent = await Torrent.LoadAsync(filename);

                //torrent.PieceLength
                var downloadsPath = Path.Combine(Environment.CurrentDirectory, "Downloads");
                var settingBuilder = new EngineSettingsBuilder
                {
                    AllowPortForwarding = true,
                    AutoSaveLoadDhtCache = true,
                    AutoSaveLoadFastResume = true,
                    AutoSaveLoadMagnetLinkMetadata = true,
                    ListenPort = 55123,
                    DhtPort = 55123,
                };

                arrayTorrentfiles.Add(torrent.Name, torrent);
                arrayTorrenStopControl.Add(torrent.Name, false);
                arrayTorrentDeleteControl.Add(torrent.Name, false);

                string size = "";
                double a = Math.Round(torrent.Size / 1048576.0, 2);

                if (a < 1000)
                {
                    size = a.ToString() + " Mb";
                }
                else if (a > 1000)
                {
                    a = Math.Round(a / 1024.0, 2);
                    size = a.ToString() + " Gb";
                }
                TorrentModel TrModel = new TorrentModel(torrent.Name, size, 0, torrent, path);
                TorrentsDataGrid.Add(TrModel);
                arrayTorrentModels.Add(torrent.Name, TrModel);
                AllTorrentListUI.Add(new SubItem(torrent.Name));
            }

        }
    }
}
