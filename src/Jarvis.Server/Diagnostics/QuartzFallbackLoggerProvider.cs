using Quartz.Logging;
using Serilog;
using LogLevel = Quartz.Logging.LogLevel;

namespace Jarvis.Server.Diagnostics;

public class QuartzFallbackLoggerProvider : ILogProvider
{
	public Logger GetLogger(string name)
	{
		return (level, func, exception, parameters) =>
		{
			var serilogLevel = TranslateLevel(level);
			if(func != null)
			{
				var template = func.Invoke();
				Log.Logger.Write(serilogLevel, template, parameters);
			}

			//if (level >= LogLevel.Info && func != null)
			//{
			//	//Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
			//}
			return true;
		};
	}

	public IDisposable OpenNestedContext(string message)
	{
		throw new NotImplementedException();
	}

	public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
	{
		throw new NotImplementedException();
	}

	private static Serilog.Events.LogEventLevel TranslateLevel(LogLevel logLevel) =>
		logLevel switch
		{
			LogLevel.Fatal => Serilog.Events.LogEventLevel.Fatal,
			LogLevel.Error => Serilog.Events.LogEventLevel.Error,
			LogLevel.Warn => Serilog.Events.LogEventLevel.Warning,
			LogLevel.Info => Serilog.Events.LogEventLevel.Information,
			LogLevel.Trace => Serilog.Events.LogEventLevel.Verbose,
			_ => Serilog.Events.LogEventLevel.Debug
		};
}