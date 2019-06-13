﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DGP.Snap.Window.Wallpaper
{
    /// <summary>
    /// WallpaperWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WallpaperWindow : System.Windows.Window
    {
        public WallpaperWindow()
        {
            InitializeComponent();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();
            base.OnClosing(e);
        }
    }
}
