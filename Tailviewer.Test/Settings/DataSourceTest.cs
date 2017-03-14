﻿using FluentAssertions;
using NUnit.Framework;
using Tailviewer.BusinessLogic;
using Tailviewer.Settings;

namespace Tailviewer.Test.Settings
{
	[TestFixture]
	public sealed class DataSourceTest
	{
		[Test]
		public void TestCtor()
		{
			var dataSource = new DataSource();
			dataSource.ColorByLevel.Should().BeTrue();
			dataSource.HideEmptyLines.Should().BeFalse();
			dataSource.IsSingleLine.Should().BeFalse();

			dataSource.ActivatedQuickFilters.Should().NotBeNull();
			dataSource.LevelFilter.Should().Be(LevelFlags.All);
			dataSource.File.Should().BeNull();

			dataSource.VisibleLogLine.Should().Be(LogLineIndex.Invalid);
			dataSource.SelectedLogLines.Should().NotBeNull();
			dataSource.SelectedLogLines.Should().BeEmpty();

			dataSource.FollowTail.Should().BeFalse();

			dataSource.ShowLineNumbers.Should().BeTrue();
		}
	}
}