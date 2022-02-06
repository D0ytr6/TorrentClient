using MonoTorrent;
using MonoTorrent.Client;
using SampleClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfTestWork.Models;

namespace WpfTestWork.Download
{
    class TorrentStartEngine
    {
        public static async Task TorrentRun(TorrentModel SelectedTorrent, Torrent torrentfile, string path)
        {
            Top10Listener Listener = new Top10Listener(10);
            CancellationTokenSource cancellation = new CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += delegate { cancellation.Cancel(); };
            var downloadsPath = path;
            var settingBuilder = new EngineSettingsBuilder
            {
                AllowPortForwarding = true,
                AutoSaveLoadDhtCache = true,
                AutoSaveLoadFastResume = true,
                AutoSaveLoadMagnetLinkMetadata = true,
                ListenPort = 55123,
                DhtPort = 55123,
            };
            var engine = new ClientEngine(settingBuilder.ToSettings());

            //torrentfile.PublisherUrl;
            var managerfortorrent = await engine.AddAsync(torrentfile, downloadsPath);
            foreach (TorrentManager manager in engine.Torrents)
            {
                await manager.StartAsync();
            }
            //managerfortorrent.

            while (engine.IsRunning)
            {
                foreach (TorrentManager manager in engine.Torrents)
                {

                    if (TorrentAddEngine.arrayTorrentDeleteControl[SelectedTorrent.MyName] == true)
                    {
                        SelectedTorrent.MyState = "Deleting";
                        //arrayTorrentDeleteControl[SelectedTorrent.MyName] = true;
                        var stoppingTask = manager.StopAsync();
                        await stoppingTask;
                        var remove = engine.RemoveAsync(manager, RemoveMode.CacheDataAndDownloadedData);
                        await remove;
                        SelectedTorrent.MyState = "Deleted";
                        return;
                    }

                    else if (TorrentAddEngine.arrayTorrenStopControl[SelectedTorrent.MyName] == true)
                    {
                        SelectedTorrent.MyState = "Stopping";
                        var stoppingTask = manager.PauseAsync();
                        await stoppingTask;
                        SelectedTorrent.MyState = manager.State.ToString();
                    }

                    else if (TorrentAddEngine.arrayTorrenStopControl[SelectedTorrent.MyName] == false && manager.State.ToString() == "Paused")
                    {
                        await manager.StartAsync();
                    }

                    int per;
                    double a = Math.Round(manager.Monitor.DownloadSpeed / 1048576.0, 2);
                    string sp = a.ToString() + " Mbits";
                    double c = Math.Round(manager.Monitor.UploadSpeed / 1048576.0, 4);
                    string spc = c.ToString() + " Mbits";

                    SelectedTorrent.MySpeed = sp;
                    SelectedTorrent.MySpeedUpload = spc;

                    var peers = manager.GetPeersAsync();
                    if (manager.Torrent != null)
                        foreach (var file in manager.Files)
                        {
                            per = (int)file.BitField.PercentComplete;
                            SelectedTorrent.MyState = manager.State.ToString();
                            if (a == 0 && per == 100)
                            {
                                SelectedTorrent.MyPercentageDownloaded = 0;
                            }
                            else if (per == 100)
                            {
                                SelectedTorrent.MyPercentageDownloaded = 100;
                            }
                            else
                            {
                                SelectedTorrent.MyPercentageDownloaded = per;
                            }

                        }
                }
                await Task.Delay(100, cancellation.Token);
            }
        }

    }
}
