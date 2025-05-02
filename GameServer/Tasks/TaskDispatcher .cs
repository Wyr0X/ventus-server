using System;
using System.Threading.Tasks;

public class TaskRunner<T>
{
    private readonly Func<Task<T>> _mainTask;
    private Func<T, Task>? _onComplete;

    public TaskRunner(Func<Task<T>> mainTask)
    {
        _mainTask = mainTask ?? throw new ArgumentNullException(nameof(mainTask));
    }

    public TaskRunner<T> OnComplete(Func<T, Task> onComplete)
    {
        _onComplete = onComplete;
        return this;
    }

    public async Task RunAsync()
    {
        T result = await _mainTask();

        if (_onComplete != null)
        {
            await _onComplete(result);
        }
    }
}
