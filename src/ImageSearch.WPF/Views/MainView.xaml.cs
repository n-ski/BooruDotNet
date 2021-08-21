﻿#nullable disable
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
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
                this.OneWayBind(ViewModel, vm => vm.BestSearchResultsViewModel, v => v.BestSearchResultsGroup.ViewModel)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.OtherSearchResultsViewModel, v => v.OtherSearchResultsGroup.ViewModel)
                    .DisposeWith(d);

                #region Status indicator

                this.OneWayBind(ViewModel, vm => vm.StatusViewModel, v => v.BusyIndicator.BusyContent)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StatusViewModel.IsActive, v => v.BusyIndicator.IsBusy)
                    .DisposeWith(d);

                #endregion

                #region Upload methods

                this.OneWayBind(ViewModel, vm => vm.UploadMethods, v => v.UploadMethodsComboBox.ItemsSource)
                    .DisposeWith(d);

                // This has to be a 2-way binding because view model can also change selected method (see SearchForSimilarImpl).
                this.Bind(ViewModel, vm => vm.SelectedUploadMethod, v => v.UploadMethodsComboBox.SelectedItem)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.UploadMethod, v => v.UploadMethodHost.ViewModel)
                    .DisposeWith(d);

                #endregion

                #region Search services

                this.OneWayBind(ViewModel, vm => vm.SearchServices, v => v.SearchServicesComboBox.ItemsSource)
                    .DisposeWith(d);

                this.WhenAnyValue(v => v.SearchServicesComboBox.SelectedItem)
                    .BindTo(this, v => v.ViewModel.SelectedSearchService)
                    .DisposeWith(d);

                #endregion

                this.BindCommand(ViewModel, vm => vm.Search, v => v.SearchButton)
                    .DisposeWith(d);

                // Because selection is bound from view to view model, initialize default selection after we're done binding here.
                UploadMethodsComboBox.SelectedIndex = 0;
                SearchServicesComboBox.SelectedIndex = 0;

                // Scroll to top after the search command is completed.
                this.WhenAnyObservable(x => x.ViewModel.Search)
                    .Subscribe(_ => SearchResultsScrollViewer.ScrollToTop())
                    .DisposeWith(d);

                #region Interactions

                ViewModel.OpenUriInteraction.RegisterHandler(interaction =>
                {
                    Uri uri = interaction.Input;
                    Debug.Assert(uri.IsAbsoluteUri);

                    using var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = uri.AbsoluteUri,
                        UseShellExecute = true,
                    });

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                ViewModel.CopyUriInteraction.RegisterHandler(interaction =>
                {
                    Uri uri = interaction.Input;
                    Debug.Assert(uri.IsAbsoluteUri);

                    Clipboard.SetText(uri.AbsoluteUri);

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                ViewModel.DisplaySearchError.RegisterHandler(interaction =>
                {
                    Exception exception = interaction.Input.InnerException ?? interaction.Input;

                    string message = string.Join(
                        Environment.NewLine,
                        $"Exception of type '{exception.GetType()}' has occurred with the following message:",
                        exception.Message);

                    MessageBox.Show(this, message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                #endregion
            });
        }
    }
}
