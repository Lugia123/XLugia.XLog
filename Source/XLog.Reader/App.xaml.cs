using XLugia.Lib.XLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace XLugia.Lib.XLog.Reader
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                LogWriter.getIns().Dispose();
            }
            catch { }
        }
    }
}
