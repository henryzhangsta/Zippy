using System;

namespace Zippy
{
	public class Logging
	{
		#if DEBUG
		public static LogLevel LEVEL = LogLevel.DEBUG;
		#else
		public static LogLevel LEVEL = LogLevel.WARNING;
		#endif

		public enum LogLevel
		{
			NONE = 0,
			TRACE = 1,
			DEBUG = 2,
			INFO = 3,
			WARNING = 4,
			ERROR = 5,
			CRITICAL = 6,
			FATAL = 7,
			ALL = 8
		}

		public static void Log(LogLevel Level, Object o)
		{
			if (Level >= LEVEL)
			{
				Console.WriteLine(String.Format("[{0}] [{1}] : {2}", DateTime.Now.ToString(), Enum.GetName(typeof(LogLevel), Level), o.ToString()));
			}
		}

		public static void None(Object o)
		{
			Log(LogLevel.NONE, o);
		}

		public static void Trace(Object o)
		{
			Log(LogLevel.TRACE, o);
		}

		public static void Debug(Object o)
		{
			Log(LogLevel.DEBUG, o);
		}

		public static void Info(Object o)
		{
			Log(LogLevel.INFO, o);
		}

		public static void Warning(Object o)
		{
			Log(LogLevel.WARNING, o);
		}

		public static void Error(Object o)
		{
			Log(LogLevel.ERROR, o);
		}

		public static void Critical(Object o)
		{
			Log(LogLevel.CRITICAL, o);
		}

		public static void Fatal(Object o)
		{
			Log(LogLevel.FATAL, o);
		}

		public static void All(Object o)
		{
			Log(LogLevel.ALL, o);
		}
	}
}

