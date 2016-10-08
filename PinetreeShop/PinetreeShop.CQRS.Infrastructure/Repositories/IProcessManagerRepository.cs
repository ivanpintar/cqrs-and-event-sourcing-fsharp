using System;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public interface IProcessManagerRepository
    {
        void SaveProcessManager<TProcessManager>(TProcessManager processManager) where TProcessManager : IProcessManager;
        TResult GetProcessManagerById<TResult>(Guid id) where TResult : IProcessManager, new();
    }
}
