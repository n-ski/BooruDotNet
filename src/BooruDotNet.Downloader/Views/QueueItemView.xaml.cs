﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BooruDotNet.Downloader.Helpers;
using BooruDotNet.Downloader.ViewModels;
using Humanizer;
using ReactiveUI;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for QueueItemView.xaml
    /// </summary>
    public partial class QueueItemView : ReactiveUserControl<QueueItemViewModel>
    {
        public QueueItemView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Post.PreviewImageUri, v => v.PreviewImage.Source, ImageHelper.CreateImageFromUri)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.Uri, v => v.SourceTextBlock.Text, StringifySource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.FileSize, v => v.FileSizeTextBlock.Text, StringifyFileSize)
                    .DisposeWith(d);

                PreviewImage
                    .Events().MouseLeftButtonDown
                    .Where(e => e.ClickCount == 2)
                    .Do(_ =>
                    {
                        var postView = new PostView
                        {
                            Owner = Window.GetWindow(this),
                            ViewModel = new PostViewModel(ViewModel.Post),
                        };

                        postView.ShowDialog();
                    })
                    .Subscribe(ReactiveHelper.DoNothing)
                    .DisposeWith(d);
            });
        }

        private static string StringifySource(Uri uri)
        {
            return $"Source: {uri.Host}";
        }

        private static string StringifyFileSize(long? size)
        {
            return $"File size: {(size.HasValue ? size.Value.Bytes().Humanize("0.00") : "unknown")}";
        }
    }
}
