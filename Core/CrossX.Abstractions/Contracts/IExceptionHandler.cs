using System;

namespace CrossX.Abstractions.Contracts
{
    public interface IExceptionHandler
    {
        void OnException(Exception ex);
    }
}
