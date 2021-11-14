using System;
using DataSlice.Core.Utils;
using NLog;

namespace DataSlice
{
    public class AppLogger : IAppLogger
    {
        private readonly Logger _nLogLogger;

        public static bool IsLoggingOn = true; //for unit testing?

        public AppLogger()
        {
            _nLogLogger = LogManager.GetCurrentClassLogger();
        }

        //public AppLogger(string className)
        //{
        //    _nLogLogger = LogManager.GetLogger(className);
        //}

        public void Info(string message)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Info(message, args);
        }

        public void Debug(string message)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Debug(message, args);
        }

        public void Warn(string message)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Warn(message, args);
        }

        public void Error(string message)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Error(message, args);
        }


        public void Error(string message, Exception ex)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Error(ex, message);
      
        }

        public void Error(string message, Exception ex, params object[] args)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Error(message, args, ex);
       
        }

        public void Error(Exception ex)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Error(ex);
         
        }

        public void Fatal(string message)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Fatal(message);
        }

        public void Fatal(string message, params object[] args)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Fatal(message, args);
        }

        public void Fatal(string message, Exception ex)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Fatal(ex, message);
    
        }

        public void Fatal(string message, Exception ex, params object[] args)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Fatal(message, ex, args);

        }

        public void Fatal(Exception ex)
        {
            if (!IsLoggingOn)
            {
                return;
            }
            _nLogLogger.Fatal(ex);
          
        }


       
    }
}