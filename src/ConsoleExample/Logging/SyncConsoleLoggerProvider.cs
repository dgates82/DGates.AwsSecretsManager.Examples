using System;
using Microsoft.Extensions.Logging;

namespace ConsoleExample.Logging
{
    /// <summary>
    /// Provides <see cref="SyncConsoleLogger"/> instances that write log entries to the console
    /// synchronously on the calling thread, rather than through the built-in console logger's
    /// background queue. Guarantees log output is flushed before a short-lived console app like
    /// ConsoleExample exits, at the cost of blocking the caller on each write.
    /// </summary>
    public class SyncConsoleLoggerProvider : ILoggerProvider
    {
        /// <summary>Creates a logger for the given category name.</summary>
        public ILogger CreateLogger(string categoryName) =>
            new SyncConsoleLogger(categoryName);

        /// <summary>No unmanaged resources to release; present only to satisfy <see cref="ILoggerProvider"/>.</summary>
        public void Dispose() { }

        /// <summary>
        /// Writes each log entry directly to <see cref="Console.WriteLine(string)"/> when it's
        /// logged, instead of queuing it for a background writer.
        /// </summary>
        private sealed class SyncConsoleLogger : ILogger
        {

            private  readonly string _categoryName;

            public SyncConsoleLogger(string categoryName)
            {
                _categoryName = categoryName;
            }

            /// <summary>Always enabled — this demo logger does not filter by level.</summary>
            public bool IsEnabled(LogLevel logLevel) => true;

            /// <summary>Scopes aren't supported; always returns <c>null</c>.</summary>
            public IDisposable BeginScope<TState>(TState state) => null;

            /// <summary>
            /// Formats the log entry as <c>[Level] Category: Message</c> (plus the exception
            /// details on a following line, if one was supplied) and writes it to the console
            /// immediately, blocking the calling thread until the write completes.
            /// </summary>
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                var message = formatter(state, exception);
                var logEntry = $"[{logLevel}] {_categoryName}: {message}";
                if (exception != null)
                {
                    logEntry += Environment.NewLine + exception;
                }

                Console.WriteLine(logEntry);
            }

        }
    }
}