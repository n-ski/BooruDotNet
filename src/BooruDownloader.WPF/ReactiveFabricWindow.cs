using System.Windows;
using ReactiveUI;

namespace BooruDownloader.WPF
{
    public class ReactiveFabricWindow<TViewModel> : FluentUI.FabricWindow, IViewFor<TViewModel> where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactiveFabricWindow<TViewModel>),
            new PropertyMetadata(default(TViewModel)));

        public TViewModel? ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel?)value;
        }
    }
}
