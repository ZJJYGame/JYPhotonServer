using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using ExitGames.Logging;

namespace AscensionServer
{
    public class Log4NetDebugHelper : IDebugHelper
    {

        ILogger _Logger ;

        public Log4NetDebugHelper()
        {
            _Logger = LogManager.GetCurrentClassLogger();
        }
        public void LogError(object msg, object context=null)
        {
            StackTrace st = new StackTrace(new StackFrame(2, true));
            StackTrace str = new StackTrace(new StackFrame(3, true));
            StackTrace stra = new StackTrace(new StackFrame(4, true));
            context = context == null ? "null" : context;
            _Logger.Error($"Message :{msg}, Context:{context};\nStackTrace[ - ] > Error：\n{st}{str}{stra}");
        }
        public void LogInfo(object msg, object context=null)
        {
            StackTrace st = new StackTrace(new StackFrame(2, true));
            StackTrace str = new StackTrace(new StackFrame(3, true));
            StackTrace stra = new StackTrace(new StackFrame(4, true));
            context = context == null ? "null" : context;
            _Logger.Info($"Message :{msg}, Context:{context};\nStackTrace[ - ] > Info：\n{st}{str}{stra}");
        }
        public void LogInfo(object msg, string msgColor, object context=null)
        {
            StackTrace st = new StackTrace(new StackFrame(2, true));
            StackTrace str = new StackTrace(new StackFrame(3, true));
            StackTrace stra = new StackTrace(new StackFrame(4, true));
            context = context == null ? "null" : context;
            _Logger.Info($"Message :{msg}, Context:{context};\nStackTrace[ - ] > Info：\n{st}{str}{stra}");
        }
        public void LogWarning(object msg, object context)
        {
            StackTrace st = new StackTrace(new StackFrame(2, true));
            StackTrace str = new StackTrace(new StackFrame(3, true));
            StackTrace stra = new StackTrace(new StackFrame(4, true));
            context = context == null ? "null" : context;
            _Logger.Warn($"Message :{msg}, Context:{context};\nStackTrace[ - ] > Warn：\n{st}{str}{stra}");
        }
    }
}
