using Database.Model;
using Extended.Tool.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Extended.Log
{
    public class LogReadModel : IDisposable
    {
        public long rowID { get; set; }
        public string guid { get; set; }
        public string logType { get; set; }
        public string logCategory { get; set; }
        public string logContent { get; set; }
        public string operationLogID { get; set; }
        public string logDateTime { get; set; }

        public void Dispose()
        {
            this.rowID = -1;
            this.guid = null;
            this.logType = null;
            this.logCategory = null;
            this.logContent = null;
            this.operationLogID = null;
            this.logDateTime = null;
        }
    }
}
