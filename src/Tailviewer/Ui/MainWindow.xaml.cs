﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using Metrolib;
using Metrolib.Controls;
using Tailviewer.Settings;
using Tailviewer.Ui.DataSourceTree;
using Tailviewer.Ui.Menu;

namespace Tailviewer.Ui
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
		: SingleApplicationHelper.IMessageListener
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static readonly DependencyProperty FocusLogFileSearchCommandProperty =
			DependencyProperty.Register("FocusLogFileSearchCommand", typeof(ICommand), typeof(MainWindow),
			                            new PropertyMetadata(default(ICommand)));

		public static readonly DependencyProperty FocusDataSourceSearchCommandProperty =
			DependencyProperty.Register("FocusDataSourceSearchCommand", typeof(ICommand), typeof(MainWindow),
			                            new PropertyMetadata(default(ICommand)));

		public static readonly DependencyProperty FocusLogFileSearchAllCommandProperty = DependencyProperty.Register(
			"FocusLogFileSearchAllCommand", typeof(ICommand), typeof(MainWindow), new PropertyMetadata(default(ICommand)));

		public static readonly DependencyProperty NewQuickFilterCommandProperty = DependencyProperty.Register(
		                                                                                                      "NewQuickFilterCommand",
		                                                                                                      typeof(ICommand
		                                                                                                      ),
		                                                                                                      typeof(
			                                                                                                      MainWindow),
		                                                                                                      new
			                                                                                                      PropertyMetadata(default
			                                                                                                                       (ICommand
			                                                                                                                       )));

		private readonly IApplicationSettings _settings;
		private readonly Dictionary<KeyBindingCommand, InputBinding> _inputBindingsByCommand;

		public MainWindow(IApplicationSettings settings, IMainWindowViewModel mainWindowViewModel)
		{
			_settings = settings ?? throw new ArgumentNullException(nameof(settings));
			FocusLogFileSearchCommand = new DelegateCommand(FocusLogFileSearch);
			FocusLogFileSearchAllCommand = new DelegateCommand(FocusLogFileSearchAll);
			FocusDataSourceSearchCommand = new DelegateCommand(FocusDataSourceSearch);
			NewQuickFilterCommand = new DelegateCommand(NewQuickFilter);
			_inputBindingsByCommand = new Dictionary<KeyBindingCommand, InputBinding>();

			InitializeComponent();
			SizeChanged += OnSizeChanged;
			LocationChanged += OnLocationChanged;
			Closing += OnClosing;
			DragEnter += OnDragEnter;
			DragOver += OnDragOver;
			Drop += OnDrop;
			MouseMove += OnMouseMove;

			DragLayer.AdornerLayer = PartDragDecorator.AdornerLayer;

			DataContext = mainWindowViewModel;
			SynchronizeInputBindings();
			mainWindowViewModel.KeyBindings.CollectionChanged += KeyBindingsOnCollectionChanged;
		}

		public ICommand FocusDataSourceSearchCommand
		{
			get { return (ICommand) GetValue(FocusDataSourceSearchCommandProperty); }
			set { SetValue(FocusDataSourceSearchCommandProperty, value); }
		}

		public ICommand FocusLogFileSearchCommand
		{
			get { return (ICommand) GetValue(FocusLogFileSearchCommandProperty); }
			set { SetValue(FocusLogFileSearchCommandProperty, value); }
		}

		public ICommand FocusLogFileSearchAllCommand
		{
			get { return (ICommand)GetValue(FocusLogFileSearchAllCommandProperty); }
			set { SetValue(FocusLogFileSearchAllCommandProperty, value); }
		}

		public ICommand NewQuickFilterCommand
		{
			get { return (ICommand) GetValue(NewQuickFilterCommandProperty); }
			set { SetValue(NewQuickFilterCommandProperty, value); }
		}

		public void OnShowMainwindow()
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				Log.InfoFormat("Ensuring main window is visible because another process asked us to...");

				if (!IsVisible)
				{
					Log.DebugFormat("Main window isn't visible anymore, showing it...");
					Show();
				}

				if (WindowState == WindowState.Minimized)
				{
					Log.DebugFormat("Main window has been minimized to taskbar, restoring it...");
					WindowState = WindowState.Normal;
				}

				Log.DebugFormat("Activating window...");
				Activate();

				// The following is a hack because sometimes Activate() doesn't bring the window
				// to the front (i.e. Tailviewer stays behind other windows, even though Activate()
				// is supposed to fix that). Therefore we shortly toggle the Topmost property to
				// force the window manager to bring Tailviewer on top.
				if (!Topmost) //< A user can configure tailviewer to naturally be on top => we don't want to mess with her settings!
				{
					Topmost = true;
					Topmost = false;
				}

				Focus();

				Log.DebugFormat("Bringing window to view (if necessary)");
				BringIntoView();
			}));
		}

		public void OnOpenDataSource(string dataSourceUri)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				Log.InfoFormat("Opening data source because another process asked us to...");
				var viewModel = DataContext as IMainWindowViewModel;
				viewModel?.AddFileOrDirectory(dataSourceUri);
			}));
		}

		private void OnLocationChanged(object sender, EventArgs eventArgs)
		{
			_settings.MainWindow.UpdateFrom(this);
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			_settings.MainWindow.UpdateFrom(this);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			DragLayer.OnMouseMove(e);
		}

		private void FocusLogFileSearch()
		{
			PartSearchBox.Focus();
		}

		private void FocusLogFileSearchAll()
		{
			PartFindAllBox.Focus();
		}

		private void FocusDataSourceSearch()
		{
			DataSourcesControl.Instance?.FocusSearch();
		}

		private void NewQuickFilter()
		{
			var viewModel = DataContext as IMainWindowViewModel;
			var logViewModel = viewModel?.LogViewPanel;
			if (logViewModel != null)
			{
				var model = logViewModel.AddQuickFilter();
				logViewModel.ShowQuickFilters();

				// Whelp, this is ugly: I just want to ensure that upon creating a new filter, the text-box is focused
				// so the user can start typing. I guess it would be prettier if we were to create a custom control
				// that takes care of it, but I'm feeling lazy...
				// (The operation is dispatched because the control is created later on and we chose a low priority to
				// ensure that this method is invoked *after* the control has been created).
				Dispatcher.BeginInvoke(new Action(() =>
				{
					var boxes = this.FindChildrenOfType<FilterTextBox>().ToList();
					var textBox = boxes.LastOrDefault(x => x.DataContext == model);
					textBox?.Focus();
				}), DispatcherPriority.Background);
			}
		}

		private void NewBookmark()
		{
			var viewModel = DataContext as IMainWindowViewModel;
			var logViewModel = viewModel?.LogViewPanel;
			if (logViewModel != null)
			{
				if (logViewModel.AddBookmark())
				{
					logViewModel.ShowBookmarks();
				}
			}
		}

		private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			_settings.MainWindow.UpdateFrom(this);
			_settings.Save();
		}

		private void OnDragOver(object sender, DragEventArgs e)
		{
			HandleDrag(e);
		}

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			HandleDrag(e);
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				// Note that you can have more than one file.
				var files = (string[]) e.Data.GetData(DataFormats.FileDrop);

				// Assuming you have one file that you care about, pass it off to whatever
				// handling code you have defined.
				((MainWindowViewModel) DataContext).AddFilesOrDirectories(files);
			}
			else
			{
				// Why the fuck is the main window even asked to handle
				// the drag when the mouse is clearly over the data source tree?!
				DataSourcesControl.Instance?.PartDataSourcesOnDrop(sender, e);
			}
		}

		private void HandleDrag(DragEventArgs e)
		{
			DragLayer.UpdateAdornerPosition(e);
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effects = DragDropEffects.Link;
			else
				DataSourcesControl.Instance?.HandleDrag(e);
			e.Handled = true;
		}

		private void OnClickOverlay(object sender, MouseButtonEventArgs e)
		{
			((IMainWindowViewModel) DataContext).CurrentFlyout = null;
		}

		private void OnCloseFlyout(object sender, RoutedEventArgs e)
		{
			((IMainWindowViewModel) DataContext).CurrentFlyout = null;
		}

		private void KeyBindingsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SynchronizeInputBindings();
		}

		private void SynchronizeInputBindings()
		{
			var items = ((IMainWindowViewModel) DataContext).KeyBindings;
			foreach (var command in items)
			{
				if (command != null)
				{
					if (!_inputBindingsByCommand.ContainsKey(command))
					{
						var binding = CreateInputBinding(command);
						_inputBindingsByCommand.Add(command, binding);
						InputBindings.Add(binding);
					}
				}
			}

			foreach (var pair in _inputBindingsByCommand.ToList())
			{
				if (!items.Contains(pair.Key))
				{
					_inputBindingsByCommand.Remove(pair.Key);
					InputBindings.Remove(pair.Value);
				}
			}
		}

		private static InputBinding CreateInputBinding(KeyBindingCommand item)
		{
			var gesture = new KeyGesture(item.GestureKey, item.GestureModifier);
			return new InputBinding(item, gesture);
		}
	}
}