/*
Achievements - Brings achievements to Kerbal Space Program.
Copyright (C) 2013-2014 Maik Schreiber

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Achievements {
	internal enum LogLevel {
		TRACE = 0,
		DEBUG = 1,
		INFO = 2,
		WARN = 3,
		ERROR = 4
	}

	internal delegate void LogMethod(string message);
	
	internal static class Log {
		private const string CATEGORY = "Achievements";

		internal static LogLevel Level = LogLevel.INFO;

		internal static void trace(string message, params object[] @params) {
			log(LogLevel.TRACE, null, message, @params);
		}
		
		internal static void debug(string message, params object[] @params) {
			log(LogLevel.DEBUG, null, message, @params);
		}

		internal static void info(string message, params object[] @params) {
			log(LogLevel.INFO, null, message, @params);
		}

		internal static void warn(string message, params object[] @params) {
			log(LogLevel.WARN, null, message, @params);
		}

		internal static void warn(Exception e, string message, params object[] @params) {
			log(LogLevel.WARN, e, message, @params);
		}

		internal static void error(string message, params object[] @params) {
			log(LogLevel.ERROR, null, message, @params);
		}

		internal static void error(Exception e, string message, params object[] @params) {
			log(LogLevel.ERROR, e, message, @params);
		}

		private static void log(LogLevel level, Exception e, string message, params object[] @params) {
			if (doLog(level)) {
				LogMethod logMethod;
				switch (level) {
					case LogLevel.TRACE:
						goto case LogLevel.INFO;
					case LogLevel.DEBUG:
						goto case LogLevel.INFO;
					case LogLevel.INFO:
						logMethod = Debug.Log;
						break;

					case LogLevel.WARN:
						logMethod = Debug.LogWarning;
						break;

					case LogLevel.ERROR:
						logMethod = Debug.LogError;
						break;

					default:
						throw new ArgumentException("unknown log level: " + level);
				}

				logMethod(getLogMessage(level, message, @params));
				if (e != null) {
					Debug.LogException(e);
				}
			}
		}

		private static bool doLog(LogLevel level) {
			return level >= Level;
		}

		private static string getLogMessage(LogLevel level, string message, params object[] @params) {
			return string.Format("[{0}] [{1}] {2}", CATEGORY, level, formatMessage(message, @params));
		}

		private static string formatMessage(string message, params object[] @params) {
			return ((@params != null) && (@params.Length > 0)) ? string.Format(message, @params) : message;
		}
	}
}
