#nullable disable
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using BooruDownloader.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace BooruDownloader.WPF.Views
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

            InputTextBox.Focus();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.InputText, v => v.InputTextBox.Text)
                    .DisposeWith(d);

                InputTextBox
                    .Events().KeyDown
                    .Where(e => ViewModel.IsValid && e.Key == Key.Enter && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    .Do(_ => DialogResult = true)
                    .Subscribe()
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsValid, v => v.OkButton.IsEnabled)
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
