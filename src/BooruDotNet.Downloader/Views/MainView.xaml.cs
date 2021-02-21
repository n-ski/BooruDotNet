﻿using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BooruDotNet.Downloader.ViewModels;
using Microsoft.Win32;
using ReactiveUI;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveFabricWindow<MainViewModel>
    {
        private string _initialDialogDirectory;

        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.QueuedItems, v => v.QueuedItemsListBox.ItemsSource)
                    .DisposeWith(d);

                // Since we can't observe SelectedItems directly, let's observe the event instead.
                QueuedItemsListBox
                    .Events().SelectionChanged
                    .Select(_ => QueuedItemsListBox.SelectedItems.Cast<QueueItemViewModel>())
                    .BindTo(this, v => v.ViewModel.SelectedItems)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsAddingPosts, v => v.AddButton.IsEnabled, isBusy => !isBusy)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.AddFromFile, v => v.AddFromFileMenuItem)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.RemoveSelection, v => v.RemoveSelectionButton)
                    .DisposeWith(d);

                Interactions.OpenFileBrowser.RegisterHandler(interaction =>
                {
                    if (!Directory.Exists(_initialDialogDirectory))
                    {
                        _initialDialogDirectory = Environment.CurrentDirectory;
                    }

                    var dialog = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Filter = interaction.Input,
                        InitialDirectory = _initialDialogDirectory,
                        Multiselect = false,
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        var fileInfo = new FileInfo(dialog.FileName);

                        _initialDialogDirectory = fileInfo.DirectoryName;
                        interaction.SetOutput(fileInfo);
                    }
                    else
                    {
                        interaction.SetOutput(null);
                    }
                });
            });
        }
    }
}
