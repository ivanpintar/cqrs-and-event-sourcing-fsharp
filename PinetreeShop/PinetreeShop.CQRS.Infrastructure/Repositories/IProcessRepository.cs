using System;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public interface IProcessRepository
    {
        void SaveProcess<TProcess>(TProcess process) where TProcess : IProcess;
        TResult GetProcessById<TResult>(Guid id) where TResult : IProcess, new();
    }
}
