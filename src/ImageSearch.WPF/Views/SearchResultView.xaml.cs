#nullable disable
using System;
using System.Reactive.Disposables;
using BooruDotNet.Helpers;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for SearchResultView.xaml
    /// </summary>
    public partial class SearchResultView : ReactiveUserControl<SearchResultViewModel>
    {
        public SearchResultView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.ImageUri, v => v.PreviewImage.Source, ImageHelper.CreateImageFromUri)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ImageSize, v => v.ImageSizeTextBlock.Text, size => $"{size.Width}x{size.Height}")
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Similarity, v => v.ImageSimilarityTextBlock.Text, similarity => $"{similarity:P0} similarity")
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.OpenSource, v => v.OpenSourceButton)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CopySource, v => v.CopySourceButton)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.SearchForSimilar, v => v.SearchForSimilarButton)
                    .DisposeWith(d);
            });
        }
    }
}
