using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public abstract class UploadViewModelBase : ReactiveObject
    {
        protected UploadViewModelBase()
        {
            var subject = new Subject<string>();

            CurrentStatus = subject.AsObservable();
            CurrentStatusObserver = subject.AsObserver();
        }

        /// <summary>
        /// Gets the kind of the search input.
        /// </summary>
        public abstract UploadInputKind UploadInputKind { get; }

        /// <summary>
        /// Perform search with specified search service.
        /// </summary>
        public abstract ReactiveCommand<SearchServiceViewModel, IReadOnlyCollection<SearchResultViewModel>> Search { get; }

        /// <summary>
        /// Gets or sets an observable that will cancel the execution of <see cref="Search"/> when it's ticked.
        /// </summary>
        [Reactive]
        public IObservable<Unit>? CancelSearch { get; set; }

        /// <summary>
        /// Gets an observable whose value indicates the current status of the search operation.
        /// </summary>
        public IObservable<string> CurrentStatus { get; }

        /// <summary>
        /// Gets an observer that is used for notifying about the current status of the search operation.
        /// </summary>
        protected IObserver<string> CurrentStatusObserver { get; }
    }
}
