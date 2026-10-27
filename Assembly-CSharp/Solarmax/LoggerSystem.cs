using System;

namespace Solarmax
{
	public class LoggerSystem : Solarmax.Singleton<LoggerSystem>, Lifecycle
	{
		public LoggerSystem()
		{
			this.mConsoleLogMode = true;
			this.mConsoleLogger = null;
			this.mConsoleLogLevel = LoggerSystem.LogLevel.DEBUG;
			this.mFileLogMode = true;
			this.mFileLogger = new FileLogger();
			this.mFileLogLevel = LoggerSystem.LogLevel.DEBUG;
		}

		public bool Init()
		{
			string empty = string.Empty;
			if (Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("consolelogmode", out empty))
			{
				this.SetConsoleLogMode(Converter.ConvertBool(empty));
			}
			if (Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("consoleloglevel", out empty))
			{
				this.SetConsoleLogLevel(Converter.ConvertNumber<int>(empty));
			}
			if (Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("filelogmode", out empty))
			{
				this.SetFileLogMode(Converter.ConvertBool(empty));
			}
			if (Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("fileloglevel", out empty))
			{
				this.SetFileLogLevel(Converter.ConvertNumber<int>(empty));
			}
			if (Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("filelogfrontname", out empty))
			{
				this.SetFileLogFrontName(empty);
			}
			if (Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("filelogextname", out empty))
			{
				this.SetFileLogExtName(empty);
			}
			if (this.mFileLogMode)
			{
				this.SetFileLogPath(Solarmax.Singleton<Framework>.Instance.GetWritableRootDir());
				this.mFileLogger.Init();
				this.ConsoleLog(LoggerSystem.LogLevel.ALWAYS, "FileLogger file path:" + (this.mFileLogger as FileLogger).GetFinalFilePath());
			}
			return true;
		}

		public void Tick(float interval)
		{
			if (this.mFileLogMode)
			{
				this.mFileLogger.Tick(interval);
			}
		}

		public void Destroy()
		{
			this.Debug("LoggerSystem    destroy  begin", new object[0]);
			if (this.mFileLogMode)
			{
				this.mFileLogger.Destroy();
			}
			this.Debug("LoggerSystem    destroy  begin", new object[0]);
		}

		public void Debug(string message, params object[] args)
		{
			if (args.Length > 0)
			{
				message = string.Format(message, args);
			}
			this.WriteLog(LoggerSystem.LogLevel.DEBUG, message);
		}

		public void Info(string message, params object[] args)
		{
			if (args.Length > 0)
			{
				message = string.Format(message, args);
			}
			this.WriteLog(LoggerSystem.LogLevel.INFO, message);
		}

		public void Warn(string message, params object[] args)
		{
			if (args.Length > 0)
			{
				message = string.Format(message, args);
			}
			this.WriteLog(LoggerSystem.LogLevel.WARN, message);
		}

		public void Error(string message, params object[] args)
		{
			if (args.Length > 0)
			{
				message = string.Format(message, args);
			}
			this.WriteLog(LoggerSystem.LogLevel.ERROR, message);
			this.WriteLog(LoggerSystem.LogLevel.ERROR, UtilTools.GetCallStack());
		}

		public void Fatal(string message, params object[] args)
		{
			if (args.Length > 0)
			{
				message = string.Format(message, args);
			}
			this.WriteLog(LoggerSystem.LogLevel.FATAL, message);
			this.WriteLog(LoggerSystem.LogLevel.FATAL, UtilTools.GetCallStack());
		}

		private void WriteLog(LoggerSystem.LogLevel level, string message)
		{
			message = string.Format("[{0}], [{1}],\t\t [frame:{2}]", LoggerSystem.LOGTITLE[(int)level], message, Solarmax.Singleton<TimeSystem>.Instance.GetFrame());
			this.ConsoleLog(level, message);
			this.FileLog(level, message);
		}

		private void ConsoleLog(LoggerSystem.LogLevel level, string message)
		{
			if (this.mConsoleLogMode && this.mConsoleLogLevel <= level && this.mConsoleLogger != null)
			{
				this.mConsoleLogger.Write(message);
			}
		}

		private void FileLog(LoggerSystem.LogLevel level, string message)
		{
			if (this.mFileLogMode && this.mFileLogLevel <= level && this.mFileLogger != null)
			{
				this.mFileLogger.Write(message);
			}
		}

		public void SetConsoleLogger(Logger logger)
		{
			this.mConsoleLogger = logger;
		}

		private void SetConsoleLogMode(bool status)
		{
			this.mConsoleLogMode = status;
		}

		private void SetConsoleLogLevel(int level)
		{
			this.mConsoleLogLevel = (LoggerSystem.LogLevel)level;
		}

		private void SetFileLogger(Logger logger)
		{
			this.mFileLogger = logger;
		}

		private void SetFileLogMode(bool status)
		{
			this.mFileLogMode = status;
		}

		private void SetFileLogLevel(int level)
		{
			this.mFileLogLevel = (LoggerSystem.LogLevel)level;
		}

		private void SetFileLogPath(string path)
		{
			if (this.mFileLogMode)
			{
				((FileLogger)this.mFileLogger).SetSavePath(path);
			}
		}

		private void SetFileLogFrontName(string name)
		{
			((FileLogger)this.mFileLogger).SetFileLogFrontName(name);
		}

		private void SetFileLogExtName(string name)
		{
			((FileLogger)this.mFileLogger).SetFileLogExtName(name);
		}

		public static void CodeComments(string str)
		{
		}

		private static string[] LOGTITLE = new string[]
		{
			"UNKNOW",
			"DEBUG",
			"INFO",
			"WARN",
			"ERROR",
			"FATAL",
			"ALWAYS"
		};

		private bool mConsoleLogMode;

		private Logger mConsoleLogger;

		private LoggerSystem.LogLevel mConsoleLogLevel;

		private bool mFileLogMode;

		private Logger mFileLogger;

		private LoggerSystem.LogLevel mFileLogLevel;

		public enum LogLevel
		{
			DEBUG = 1,
			INFO,
			WARN,
			ERROR,
			FATAL,
			ALWAYS
		}
	}
}
