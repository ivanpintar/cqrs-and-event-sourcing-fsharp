using System;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public interface IProcessManagerRepository
    {
        void SaveProcessManager<TProcessManager>(TProcessManager processManager) where TProcessManager : IProcessManager;
        TProcessManager GetProcessManagerById<TProcessManager>(Guid id) where TProcessManager : IProcessManager, new();
    }
}
