#nullable disable
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using BooruDotNet.Helpers;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using BooruDownloader.ViewModels;
using Humanizer;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Validation;

namespace BooruDownloader.WPF.Views
{
    /// <summary>
    /// Interaction logic for QueueItemView.xaml
    /// </summary>
    public partial class QueueItemView : ReactiveUserControl<QueueItemViewModel>
    {
        private readonly IBooruTagByName _tagExtractor;

        public QueueItemView(IBooruTagByName tagExtractor)
        {
            _tagExtractor = Requires.NotNull(tagExtractor, nameof(tagExtractor));

            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Post.PreviewImageUri, v => v.PreviewImage.Source, ImageHelper.CreateImageFromUri)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.ID, v => v.PostIdTextBlock.Text, StringifyId)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.Uri, v => v.SourceTextBlock.Text, StringifySource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.Uri, v => v.SourceTextBlock.ToolTip)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.FileSize, v => v.FileSizeTextBlock.Text, StringifyFileSize)
                    .DisposeWith(d);

                this.Events().MouseDoubleClick
                    .Where(e => e.ChangedButton == MouseButton.Left)
                    .Do(_ =>
                    {
                        var windows = Application.Current.Windows.OfType<PostView>();
                        var postUri = ViewModel.Post.Uri;

                        // Try to set focus to the existing window first.
                        if (windows.FirstOrDefault(w => Equals(w.Tag, postUri)) is PostView postView)
                        {
                            if (postView.WindowState == WindowState.Minimized)
                            {
                                postView.WindowState = WindowState.Normal;
                            }

                            postView.Focus();
                        }
                        else
                        {
                            var viewModel = ViewModel.Post is IPostExtendedTags postExtendedTags
                                ? new PostViewModel(postExtendedTags)
                                : new PostViewModel(ViewModel.Post, _tagExtractor);

                            postView = new PostView
                            {
                                Owner = Window.GetWindow(this),
                                Tag = postUri,
                                ViewModel = viewModel,
                            };

                            postView.Show();
                        }
                    })
                    .Subscribe()
                    .DisposeWith(d);
            });
        }

        private string StringifyId(int? id)
        {
            return $"ID: {(id.HasValue ? id.Value.ToString() : "unknown")}";
        }

        private static string StringifySource(Uri uri)
        {
            return $"Source: {uri.Host}";
        }

        private static string StringifyFileSize(long? size)
        {
            return $"Size: {(size.HasValue ? size.Value.Bytes().Humanize("0.00") : "unknown")}";
        }
    }
}
