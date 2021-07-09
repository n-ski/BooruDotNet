#nullable disable
using System.IO;
using System.Reactive.Disposables;
using System.Windows;
using BooruDotNet.Helpers;
using ImageSearch.ViewModels;
using Ookii.Dialogs.Wpf;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for FileUploadView.xaml
    /// </summary>
    public partial class FileUploadView : ReactiveUserControl<FileUploadViewModel>
    {
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
                        Filter = "Images|*.jpg;*.jpeg;*.png",
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
