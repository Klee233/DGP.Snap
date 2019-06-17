﻿using DGP.Snap.Helper;
using FileDownloade;
using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DGP.Snap.Services.Update
{
    class UpdateService
    {
        /// <summary>
        /// 不要在调用 <see cref="CheckUpdateAvailability()"/> 前使用，默认为<see cref="null"/>
        /// </summary>
        public static Uri PackageUri { get; set; } = null;

        /// <summary>
        /// 不要在调用 <see cref="CheckUpdateAvailability()"/> 前使用，默认为<see cref="null"/>
        /// </summary>
        public static Version NewVersion { get; set; } = null;

        public static Version CurrentVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public static async Task<UpdateAvailability> CheckUpdateAvailability()
        {
            try
            {
                Release release = await Json.GetWebRequestJsonObjectAsync<Release>("https://api.github.com/repos/DGP-Studio/DGP.Snap/releases/latest");

                var newVersion = release.Tag_name;
                NewVersion = new Version(release.Tag_name);

                if (new Version(newVersion) > CurrentVersion)//有新版本
                {
                    //Debug.WriteLine(newVersion);
                    PackageUri = new Uri(release.Assets[0].Browser_download_url);
                    return UpdateAvailability.NeedUpdate;
                }
                else
                {
                    if(new Version(newVersion) == CurrentVersion)
                    {
                        //最新发行版
                        return UpdateAvailability.IsNewestRelease;
                    }
                    else
                    {
                        //测试版
                        return UpdateAvailability.IsInsiderVersion;
                    }

                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return UpdateAvailability.NotAvailable;
            }
        }

        public static void DownloadAndInstallPackage()
        {
            UpdateProgressWindow = new UpdateProgressWindow();//弹出下载更新窗口
            UpdateProgressWindow.Show();
            IFileDownloader fileDownloader = new FileDownloade.FileDownloader();
            fileDownloader.DownloadProgressChanged += OnDownloadProgressChanged;
            fileDownloader.DownloadFileCompleted += OnDownloadFileCompleted;

            string destinationPath = Environment.CurrentDirectory + @"\Package.zip";
            //DirectorySecurity directorySecurity = new DirectorySecurity();
            //directorySecurity.AddAccessRule(new FileSystemAccessRule(@"Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            //Directory.CreateDirectory(destinationPath);
            fileDownloader.DownloadFileAsync(PackageUri, destinationPath);
        }

        internal static void OnDownloadProgressChanged(object sender, DownloadFileProgressChangedArgs args)
        {
            double percent = Math.Round((double)args.BytesReceived/ args.TotalBytesToReceive*100,2);
            UpdateProgressWindow.ProgressBar.IsIndeterminate = false;
            UpdateProgressWindow.SetNewProgress(percent);
            UpdateProgressWindow.ProgressIndicatorText.Text = $@"{percent}% - {args.BytesReceived/1024}B / {args.TotalBytesToReceive/1024}B";
        }

        internal static void OnDownloadFileCompleted(object sender, DownloadFileCompletedArgs eventArgs)
        {
            if (eventArgs.State == CompletedState.Succeeded)
            {
                //download completed
                UpdateProgressWindow.SetNewProgress(100);
                UpdateProgressWindow.ProgressIndicatorText.Text = "下载已完成，请稍候...";
                Thread.Sleep(2000);
                //await Task.Run(() => { Thread.Sleep(2000); });

                UpdateProgressWindow.Close();
                StartUpdateInstall();

            }
            else if (eventArgs.State == CompletedState.Failed)
            {
                //download failed
            }
        }

        private static UpdateProgressWindow UpdateProgressWindow { get; set; } = null;

        public static void StartUpdateInstall()
        {
            Computer MyComputer = new Computer();
            MyComputer.FileSystem.RenameFile(Path.GetFullPath("DGP.Snap.Updater.exe"), "OldUpdater.exe");
            
            Process.Start("OldUpdater.exe");
            Application.Current.Shutdown();
        }
    }

}
