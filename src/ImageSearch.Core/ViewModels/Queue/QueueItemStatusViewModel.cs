using System;
using System.Reactive;
using System.Reactive.Linq;
using ImageSearch.Helpers;
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

            Retry = ReactiveCommand.Create(
                MethodHelper.DoNothing,
                this.WhenAnyValue(x => x.Status, status => status != QueueItemStatus.Processing));

            // Due to a bug in System.Reactive, when the search is processing after it has errored,
            // IsExecuting observable will never tick 'true', so override status value here as a hack.
            // See <https://github.com/reactiveui/ReactiveUI/issues/2894>.
            Retry
                .Select(_ => QueueItemStatus.Processing)
                .BindTo(this, x => x.Status);

            Remove = ReactiveCommand.Create(MethodHelper.DoNothing);
        }

        #region Properties

        [Reactive]
        public QueueItemStatus Status { get; set; }

        [Reactive]
        public string? Text { get; set; }

        [Reactive]
        public Exception? Exception { get; set; }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> Retry { get; }

        public ReactiveCommand<Unit, Unit> Remove { get; }

        #endregion
    }
}
