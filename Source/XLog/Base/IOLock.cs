using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XLog.Base
{
    internal class IOLock
    {
        private static IOLock _instance = new IOLock();
        public static IOLock getIns() { return _instance; }

        private System.Threading.ReaderWriterLock readerWriterLock;

        public IOLock()
        {
            try
            {
                readerWriterLock = new System.Threading.ReaderWriterLock();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "IOLock InitializeComponent");
            }
        }

        public void acquireReaderLock()
        {
            try
            {
                readerWriterLock.AcquireReaderLock(-1);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "IOLock acquireReaderLock");
            }
        }

        public void releaseReaderLock()
        {
            try
            {
                readerWriterLock.ReleaseReaderLock();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "IOLock releaseReaderLock");
            }
        }

        public void acquireWriterLock()
        {
            try
            {
                readerWriterLock.AcquireWriterLock(-1);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "IOLock acquireWriterLock");
            }
        }

        public void releaseWriterLock()
        {
            try
            {
                readerWriterLock.ReleaseWriterLock();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "IOLock releaseWriterLock");
            }
        }
    }
}
