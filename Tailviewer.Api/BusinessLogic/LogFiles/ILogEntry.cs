﻿using System;

namespace Tailviewer.BusinessLogic.LogFiles
{
	/// <summary>
	///     Provides read-only access to a log entry of a <see cref="ILogFile" />.
	/// </summary>
	/// <remarks>
	///     This interface is meant to replace <see cref="LogLine" />.
	///     With its introduction, <see cref="LogLineIndex" /> can be removed and be replaced
	///     with <see cref="LogEntryIndex" />.
	/// </remarks>
	public interface ILogEntry
	{
		/// <summary>
		///     The timestamp of this log entry, if available.
		/// </summary>
		DateTime? Timestamp { get; }

		/// <summary>
		///     The raw content of this log entry.
		/// </summary>
		/// <remarks>
		///     Might not be readable by a humand, depending on the data source.
		/// </remarks>
		string RawContent { get; }

		/// <summary>
		///     Returns the value of this log entry for the given column.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <returns></returns>
		T GetColumnValue<T>(ILogFileColumn<T> column);
	}
}