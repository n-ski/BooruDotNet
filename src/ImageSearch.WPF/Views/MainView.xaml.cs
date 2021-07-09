﻿#nullable disable
using System.Reactive.Disposables;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveWindow<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.BestResults, v => v.BestSearchResultsControl.ItemsSource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.UploadMethods, v => v.UploadMethodsComboBox.ItemsSource)
                    .DisposeWith(d);

                this.WhenAnyValue(v => v.UploadMethodsComboBox.SelectedItem)
                    .BindTo(this, v => v.ViewModel.SelectedUploadMethod)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.UploadMethod, v => v.UploadMethodHost.ViewModel)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.SearchServices, v => v.SearchServicesComboBox.ItemsSource)
                    .DisposeWith(d);

                this.WhenAnyValue(v => v.SearchServicesComboBox.SelectedItem)
                    .BindTo(this, v => v.ViewModel.SelectedSearchService)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.Search, v => v.SearchButton)
                    .DisposeWith(d);
            });
        }
    }
}