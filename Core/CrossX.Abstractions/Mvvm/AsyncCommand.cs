using CrossX.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossX.Abstractions.Mvvm
{
    public class AsyncCommand : SyncCommand
    {
        public AsyncCommand(IExceptionHandler exceptionHandler, Func<object, Task> executeTask, Func<object, bool> canExecute) : base(
            new Action<object>(async parameter =>
            {
                try
                {
                    await executeTask(parameter);
                }
                catch (AggregateException aggregate)
                {
                    PopulateExceptions(exceptionHandler, aggregate.InnerExceptions);
                    PopulateException(exceptionHandler, aggregate.InnerException);
                }
                catch (Exception ex)
                {
                    PopulateException(exceptionHandler, ex);
                }
            }),
            canExecute
            )
        {
        }

        public AsyncCommand(IExceptionHandler exceptionHandler, Func<Task> executeTask, Func<bool> canExecute)
            : this(exceptionHandler, p => executeTask(), o => canExecute?.Invoke() ?? true)
        {
        }

        public AsyncCommand(IExceptionHandler exceptionHandler, Func<object, Task> executeTask) : this(exceptionHandler, executeTask, null)
        {
        }


        public AsyncCommand(IExceptionHandler exceptionHandler, Func<Task> executeTask) : this(exceptionHandler, o => executeTask())
        {
        }

        private static void PopulateExceptions(IExceptionHandler exceptionHandler, IList<Exception> exceptions)
        {
            if (exceptions == null) return;

            for (var idx = 0; idx < exceptions.Count; ++idx)
            {
                PopulateException(exceptionHandler, exceptions[idx]);
            }
        }

        private static void PopulateException(IExceptionHandler exceptionHandler, Exception exception)
        {
            if (exception is AggregateException ae)
            {
                PopulateException(exceptionHandler, ae.InnerException);
                PopulateExceptions(exceptionHandler, ae.InnerExceptions);
            }
            else if (exception != null)
            {
                exceptionHandler.OnException(exception);
            }
        }
    }
}
