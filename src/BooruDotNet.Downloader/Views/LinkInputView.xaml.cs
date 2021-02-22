using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using BooruDotNet.Downloader.ViewModels;
using ReactiveUI;
using System;
using BooruDotNet.Downloader.Helpers;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for LinkInputView.xaml
    /// </summary>
    public partial class LinkInputView : ReactiveFabricWindow<LinkInputViewModel>
    {
        public LinkInputView()
        {
            InitializeComponent();
            ViewModel = new LinkInputViewModel();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.InputText, v => v.InputTextBox.Text)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.Ok, v => v.OkButton)
                    .DisposeWith(d);

                OkButton
                    .Events().Click
                    .Do(_ => DialogResult = true)
                    .Subscribe()
                    .DisposeWith(d);

                CancelButton
                    .Events().Click
                    .Do(_ => DialogResult = false)
                    .Subscribe()
                    .DisposeWith(d);
            });
        }
    }
}
