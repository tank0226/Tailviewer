﻿using System.Text;
using Tailviewer.Api;

// ReSharper disable once CheckNamespace
namespace Tailviewer.Core
{
	/// <summary>
	///     Responsible for storing settings related to <see cref="ILogSource" /> and its implementations.
	/// </summary>
	[Service]
	public interface ILogFileSettings
	{
		/// <summary>
		///     The encoding used to interpret text based log files.
		/// </summary>
		/// <remarks>
		///     When no encoding is specified (=null), which is the default,
		///     then the encoding will be auto detected and/or specified by plugins.
		///     When a non-null value is specified, then that value will be used
		///     for all text based log files, no matter what.
		/// </remarks>
		Encoding DefaultEncoding { get; set; }
	}
}