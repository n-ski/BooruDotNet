using System;
using System.Reactive.Disposables;
using System.Windows.Media;
using BooruDotNet;
using BooruDownloader.ViewModels;
using Humanizer;
using ReactiveUI;

namespace BooruDownloader.Views
{
    /// <summary>
    /// Interaction logic for TagView.xaml
    /// </summary>
    public partial class TagView : ReactiveUserControl<TagViewModel>
    {
        public TagView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TagNameTextBlock.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TagNameTextBlock.ToolTip)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Kind, v => v.TagNameTextBlock.Foreground, TagKindToBrush)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Count, v => v.TagCountTextBlock.Text, count => count.ToMetric(decimals: 2))
                    .DisposeWith(d);
            });
        }

        private Brush TagKindToBrush(TagKind tagKind)
        {
            string key = tagKind switch
            {
                TagKind.General => "TagGeneralBrush",
                TagKind.Artist => "TagArtistBrush",
                TagKind.Copyright => "TagCopyrightBrush",
                TagKind.Character => "TagCharacterBrush",
                TagKind.Metadata => "TagMetaBrush",
                _ => throw new ArgumentOutOfRangeException(nameof(tagKind)),
            };

            return (Brush)FindResource(key);
        }
    }
}
