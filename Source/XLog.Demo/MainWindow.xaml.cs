﻿using XLugia.Lib.XLog.Demo.Lib;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLugia.Lib.XLog.Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void bodyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LogWriter.getIns().writeLog("Log Init", LogType.getIns().debug.application);

                bodyGrid.Children.Clear();
                bodyGrid.Children.Add(new Model.LogWriteDemo.LogWriteTest());
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "MainWindow bodyGrid_Loaded");
            }
        }
    }
}
