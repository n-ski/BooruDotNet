using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public class QueueItemStatusViewModel : ReactiveObject
    {
        public QueueItemStatusViewModel()
        {
            this.WhenAnyValue(x => x.Status)
                .Where(status => status != QueueItemStatus.Error)
                .Select(_ => default(Exception))
                .BindTo(this, x => x.Exception);

            this.WhenAnyValue(x => x.Exception)
                .WhereNotNull()
                .Select(_ => QueueItemStatus.Error)
                .BindTo(this, x => x.Status);

            this.WhenAnyValue(x => x.Exception)
                .WhereNotNull()
                .Select(_ => "Error. Hover over for details.")
                .BindTo(this, x => x.Text);
        }

        [Reactive]
        public QueueItemStatus Status { get; set; }

        [Reactive]
        public string? Text { get; set; }

        [Reactive]
        public Exception? Exception { get; set; }
    }
}
