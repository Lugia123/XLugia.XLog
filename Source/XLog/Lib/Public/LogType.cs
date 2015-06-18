using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public class LogType : LogTypeObject
    {
        private static LogType instance = new LogType();
        public static LogType getIns() { return instance; }

        public LogType()
            : base("", "")
        {
            try
            {
                this._debug = new LogSubType("Debug", logType);
                this._info = new LogSubType("Info", logType);
                this._warn = new LogSubType("Warn", logType);
                this._error = new LogSubType("Error", logType);
                this._fatal = new LogSubType("Fatal", logType);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogType InitializeComponent");
            }
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        public LogSubType debug { get { return _debug; } }
        LogSubType _debug { get; set; }

        /// <summary>
        /// 信息日志
        /// </summary>
        public LogSubType info { get { return _info; } }
        LogSubType _info { get; set; }

        /// <summary>
        /// 警告日志
        /// </summary>
        public LogSubType warn { get { return _warn; } }
        LogSubType _warn { get; set; }

        /// <summary>
        /// 错误日志
        /// </summary>
        public LogSubType error { get { return _error; } }
        LogSubType _error { get; set; }

        /// <summary>
        /// 致命错误日志
        /// </summary>
        public LogSubType fatal { get { return _fatal; } }
        LogSubType _fatal { get; set; }
    }

    /// <summary>
    /// 日志子类型
    /// </summary>
    public class LogSubType : LogTypeObject
    {
        public LogSubType(string logID, string LogSubType)
            : base(logID, LogSubType)
        {
            try
            {
                this._net = new LogTypeObject("Net", logType);
                this._print = new LogTypeObject("Print", logType);
                this._communication = new LogTypeObject("Communication", logType);
                this._file = new LogTypeObject("File", logType);
                this._application = new LogTypeObject("Application", logType);
                this._security = new LogTypeObject("Security", logType);
                this._setup = new LogTypeObject("Setup", logType);
                this._system = new LogTypeObject("System", logType);
                this._database = new LogTypeObject("Database", logType);
                this._plc = new LogTypeObject("PLC", logType);
                this._hardware = new LogTypeObject("Hardware", logType);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogSubType InitializeComponent");
            }
        }

        /// <summary>
        /// 网络
        /// </summary>
        public LogTypeObject net { get { return _net; } }
        LogTypeObject _net { get; set; }

        /// <summary>
        /// 打印
        /// </summary>
        public LogTypeObject print { get { return _print; } }
        LogTypeObject _print { get; set; }

        /// <summary>
        /// 通讯
        /// </summary>
        public LogTypeObject communication { get { return _communication; } }
        LogTypeObject _communication { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public LogTypeObject file { get { return _file; } }
        LogTypeObject _file { get; set; }

        /// <summary>
        /// 应用程序
        /// </summary>
        public LogTypeObject application { get { return _application; } }
        LogTypeObject _application { get; set; }

        /// <summary>
        /// 安全
        /// </summary>
        public LogTypeObject security { get { return _security; } }
        LogTypeObject _security { get; set; }

        /// <summary>
        /// 设置
        /// </summary>
        public LogTypeObject setup { get { return _setup; } }
        LogTypeObject _setup { get; set; }

        /// <summary>
        /// 系统
        /// </summary>
        public LogTypeObject system { get { return _system; } }
        LogTypeObject _system { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public LogTypeObject database { get { return _database; } }
        LogTypeObject _database { get; set; }

        /// <summary>
        /// PLC
        /// </summary>
        public LogTypeObject plc { get { return _plc; } }
        LogTypeObject _plc { get; set; }

        /// <summary>
        /// 硬件
        /// </summary>
        public LogTypeObject hardware { get { return _hardware; } }
        LogTypeObject _hardware { get; set; }
    }

    /// <summary>
    /// 日志类型对象
    /// </summary>
    public class LogTypeObject
    {
        public LogTypeObject(string logID, string LogSubType)
        {
            try
            {
                this._logID = logID;
                this._LogSubType = LogSubType;
                this.isEnabled = true;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogTypeObject InitializeComponent");
            }
        }

        /// <summary>
        /// 日志父类型
        /// </summary>
        public string LogSubType { get { return _LogSubType; } }
        string _LogSubType { get; set; }

        /// <summary>
        /// 日志编号
        /// </summary>
        public string logID { get { return _logID; } }
        string _logID { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public string logType
        {
            get
            {

                string result = LogSubType;
                try
                {
                    result += string.IsNullOrEmpty(LogSubType) ? "" : "_";
                    result += logID;
                }
                catch (Exception ex)
                {
                    SimpleLogController.writeErrorLog(ex, "LogTypeObject logType get");
                }
                return result;
            }
        }

        /// <summary>
        /// 是否启用该日志
        /// </summary>
        public bool isEnabled { get; set; }
    }
}
