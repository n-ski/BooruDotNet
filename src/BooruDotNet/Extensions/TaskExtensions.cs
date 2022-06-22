using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

internal static class TaskExtensions
{
    /// <inheritdoc cref="Task.ConfigureAwait(bool)"/>
    public static ConfiguredTaskAwaitable CAF(this Task task) => task.ConfigureAwait(false);

    /// <inheritdoc cref="Task{TResult}.ConfigureAwait(bool)"/>
    public static ConfiguredTaskAwaitable<T> CAF<T>(this Task<T> task) => task.ConfigureAwait(false);

    /// <inheritdoc cref="ValueTask.ConfigureAwait(bool)"/>
    public static ConfiguredValueTaskAwaitable CAF(this ValueTask task) => task.ConfigureAwait(false);

    /// <inheritdoc cref="ValueTask{TResult}.ConfigureAwait(bool)"/>
    public static ConfiguredValueTaskAwaitable<T> CAF<T>(this ValueTask<T> task) => task.ConfigureAwait(false);
}
