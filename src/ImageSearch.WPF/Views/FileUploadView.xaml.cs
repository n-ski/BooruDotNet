#nullable disable
using System;
using System.IO;
using System.Reactive.Disposables;
using System.Windows;
using ImageSearch.ViewModels;
using ImageSearch.WPF.Helpers;
using Ookii.Dialogs.Wpf;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for FileUploadView.xaml
    /// </summary>
    public partial class FileUploadView : ReactiveUserControl<FileUploadViewModel>
    {
        private static readonly Lazy<string> _fileFilterLazy = new Lazy<string>(() => "Images|*" + string.Join(";*", FileHelper.ImageFileExtensions));

        public FileUploadView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.FileToUpload, v => v.FilePathTextBox.Text, file => file?.FullName)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.SelectFile, v => v.SelectFileButton)
                    .DisposeWith(d);

                ViewModel.ShowFileSelection.RegisterHandler(interaction =>
                {
                    var dialog = new VistaOpenFileDialog
                    {
                        Multiselect = false,
                        Filter = _fileFilterLazy.Value,
                    };

                    if (dialog.ShowDialog(Window.GetWindow(this)) is true)
                    {
                        interaction.SetOutput(new FileInfo(dialog.FileName));
                    }
                    else
                    {
                        interaction.SetOutput(null);
                    }
                }).DisposeWith(d);
            });
        }
    }
}
