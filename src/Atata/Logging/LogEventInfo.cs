﻿using System;

namespace Atata
{
    /// <summary>
    /// Represents the logging event information raised by Atata framework.
    /// </summary>
    public class LogEventInfo
    {
        internal LogEventInfo()
        {
            Timestamp = DateTime.Now;
            BuildStart = ATContext.BuildStart.Value;
            TestName = ATContext.Current?.TestName;
            TestStart = ATContext.Current?.TestStart ?? DateTime.MinValue;
        }

        /// <summary>
        /// Gets the timestamp of the logging event.
        /// </summary>
        public DateTime Timestamp { get; internal set; }

        /// <summary>
        /// Gets the level of the logging event.
        /// </summary>
        public LogLevel Level { get; internal set; }

        /// <summary>
        /// Gets the log message.
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// Gets the exception information.
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Gets the section start information.
        /// </summary>
        public LogSection SectionStart { get; internal set; }

        /// <summary>
        /// Gets the section end information.
        /// </summary>
        public LogSection SectionEnd { get; internal set; }

        /// <summary>
        /// Gets the build start date and time.
        /// </summary>
        /// <value>
        /// The build start. Contains the same value for all the tests being executed within one build.
        /// </value>
        public DateTime BuildStart { get; internal set; }

        /// <summary>
        /// Gets the name of the test.
        /// </summary>
        /// <value>
        /// The name of the test.
        /// </value>
        public string TestName { get; internal set; }

        /// <summary>
        /// Gets the test start date and time.
        /// </summary>
        /// <value>
        /// The test start.
        /// </value>
        public DateTime TestStart { get; internal set; }
    }
}
