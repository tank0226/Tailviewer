﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tailviewer.BusinessLogic;
using Tailviewer.BusinessLogic.LogFiles;
using Tailviewer.Core.LogFiles;

namespace Tailviewer.Test.BusinessLogic.LogFiles
{
	[TestFixture]
	public sealed class LogEntriesViewTest
	{
		[Test]
		public void TestConstruction()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);
			view.Columns.Should().Equal(new object[] {LogFileColumns.LogLevel, LogFileColumns.Message});
			view.Contains(LogFileColumns.LogLevel).Should().BeTrue();
			view.Contains(LogFileColumns.Message).Should().BeTrue();
			view.Contains(LogFileColumns.Index).Should().BeFalse();
			view.Contains(LogFileColumns.LogEntryIndex).Should().BeFalse();

			inner.Setup(x => x.Count).Returns(42);
			view.Count.Should().Be(42);
		}

		[Test]
		public void TestCopyFromArray()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			var source = new LevelFlags[101];
			view.CopyFrom(LogFileColumns.LogLevel, 42, source, 2, 98);
			inner.Verify(x => x.CopyFrom(LogFileColumns.LogLevel, 42, source, 2, 98), Times.Once);
		}

		[Test]
		public void TestCopyFromArray_NoSuchColumn()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			var source = new DateTime?[101];
			new Action(() => view.CopyFrom(LogFileColumns.Timestamp, 42, source, 2, 98)).Should().Throw<NoSuchColumnException>();
			inner.Verify(x => x.CopyFrom(LogFileColumns.Timestamp, It.IsAny<int>(), It.IsAny<DateTime?[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public void TestCopyFromLogFile_Contiguous()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			var source = new Mock<ILogFile>();
			var queryOptions = new LogFileQueryOptions(LogFileQueryMode.FromCacheOnly);
			view.CopyFrom(LogFileColumns.LogLevel, 42, source.Object, new LogFileSection(2, 98), queryOptions);
			inner.Verify(x => x.CopyFrom(LogFileColumns.LogLevel, 42, source.Object, new LogFileSection(2, 98), queryOptions), Times.Once);
		}

		[Test]
		public void TestCopyFromLogFile_Contiguous_NoSuchColumn()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			var source = new Mock<ILogFile>();
			var queryOptions = new LogFileQueryOptions(LogFileQueryMode.FromCacheOnly);
			new Action(() => view.CopyFrom(LogFileColumns.Timestamp, 42, source.Object, new LogFileSection(2, 98), queryOptions)).Should().Throw<NoSuchColumnException>();
			inner.Verify(x => x.CopyFrom(LogFileColumns.Timestamp, It.IsAny<int>(), It.IsAny<ILogFile>(), It.IsAny<LogFileSection>(), It.IsAny<LogFileQueryOptions>()), Times.Never);
		}

		[Test]
		public void TestCopyFromLogFile_Noncontiguous()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			var source = new Mock<ILogFile>();
			var sourceIndices = new[] {new LogLineIndex(1), new LogLineIndex(42)};
			var queryOptions = new LogFileQueryOptions(LogFileQueryMode.FromCacheOnly);
			view.CopyFrom(LogFileColumns.LogLevel, 42, source.Object, sourceIndices, queryOptions);
			inner.Verify(x => x.CopyFrom(LogFileColumns.LogLevel, 42, source.Object, sourceIndices, queryOptions), Times.Once);
		}

		[Test]
		public void TestCopyFromLogFile_Noncontiguous_NoSuchColumn()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			var source = new Mock<ILogFile>();
			var sourceIndices = new[] {new LogLineIndex(1), new LogLineIndex(42)};
			var queryOptions = new LogFileQueryOptions(LogFileQueryMode.FromCacheOnly);
			new Action(() => view.CopyFrom(LogFileColumns.Timestamp, 42, source.Object, sourceIndices, queryOptions)).Should().Throw<NoSuchColumnException>();
			inner.Verify(x => x.CopyFrom(LogFileColumns.Timestamp, It.IsAny<int>(), It.IsAny<ILogFile>(), It.IsAny<IReadOnlyList<LogLineIndex>>(), It.IsAny<LogFileQueryOptions>()), Times.Never);
		}

		[Test]
		public void TestFillDefault()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			view.FillDefault(123, 456);
			inner.Verify(x => x.FillDefault(123, 456), Times.Once);
		}

		[Test]
		public void TestFillDefaultColumn()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			view.FillDefault(LogFileColumns.LogLevel, 123, 456);
			inner.Verify(x => x.FillDefault(LogFileColumns.LogLevel, 123, 456), Times.Once);
		}

		[Test]
		public void TestFillDefaultColumn_NoSuchColumn()
		{
			var inner = new Mock<ILogEntries>();
			var view = new LogEntriesView(inner.Object, LogFileColumns.LogLevel, LogFileColumns.Message);

			new Action(() => view.FillDefault(LogFileColumns.DeltaTime, 123, 456))
				.Should().Throw<NoSuchColumnException>();
			inner.Verify(x => x.FillDefault(It.IsAny<ILogFileColumnDescriptor>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public void TestEnumerable()
		{
			var inner = new LogEntryList(LogFileColumns.Minimum);
			inner.Add(new ReadOnlyLogEntry(new Dictionary<ILogFileColumnDescriptor, object>{{LogFileColumns.RawContent, "Hello!"}, {LogFileColumns.Timestamp, new DateTime(2021, 02, 11, 18, 49, 32)}}));
			var view = new LogEntriesView(inner, LogFileColumns.RawContent);

			var logEntries = view.ToList<ILogEntry>();
			logEntries.Should().HaveCount(1);
			var logEntryView = logEntries[0];
			logEntryView.Columns.Should().Equal(new object[] {LogFileColumns.RawContent});
			logEntryView.RawContent.Should().Be("Hello!");
			logEntryView.RawContent = "What's up?";

			inner[0].RawContent.Should().Be("What's up?");
		}

		[Test]
		public void TestReadOnlyEnumerable()
		{
			var inner = new LogEntryList(LogFileColumns.Minimum);
			inner.Add(new ReadOnlyLogEntry(new Dictionary<ILogFileColumnDescriptor, object>{{LogFileColumns.RawContent, "Hello!"}, {LogFileColumns.Timestamp, new DateTime(2021, 02, 11, 18, 49, 32)}}));
			var view = new LogEntriesView(inner, LogFileColumns.RawContent);

			var logEntries = view.ToList<IReadOnlyLogEntry>();
			logEntries.Should().HaveCount(1);
			var logEntryView = logEntries[0];
			logEntryView.Columns.Should().Equal(new object[] {LogFileColumns.RawContent});
			logEntryView.RawContent.Should().Be("Hello!");
		}
	}
}