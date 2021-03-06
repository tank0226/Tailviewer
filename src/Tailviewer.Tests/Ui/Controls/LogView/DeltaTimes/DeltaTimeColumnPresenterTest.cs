﻿using System;
using System.Threading;
using System.Windows;
using Moq;
using NUnit.Framework;
using Tailviewer.Api;
using Tailviewer.Settings;
using Tailviewer.Ui.LogView.DeltaTimes;

namespace Tailviewer.Tests.Ui.Controls.LogView.DeltaTimes
{
	[TestFixture]
	[RequiresThread(ApartmentState.STA)]
	public sealed class DeltaTimeColumnPresenterTest
	{
		private DeltaTimeColumnPresenter _column;
		private Mock<ILogSource> _logFile;

		[SetUp]
		public void Setup()
		{
			_column = new DeltaTimeColumnPresenter(TextSettings.Default);
			_logFile = new Mock<ILogSource>();
		}

		[Test]
		[Description("Verifies that values for the delta time column are not retrieved when the control is invisible")]
		public void TestUpdateLinesNotVisible([Values(Visibility.Collapsed, Visibility.Hidden)] Visibility visibility)
		{
			_column.Visibility = visibility;

			_column.FetchValues(_logFile.Object, new LogSourceSection(0, 1), 0);
			_logFile.Verify(x => x.GetColumn(It.IsAny<LogSourceSection>(),
			                                 It.IsAny<IColumnDescriptor<TimeSpan?>>(),
			                                 It.IsAny<TimeSpan?[]>(),
			                                 It.IsAny<int>(),
											 It.IsAny<LogSourceQueryOptions>()),
			                Times.Never, "because the control is invisible and thus no data should've been retrieved");
		}
	}
}