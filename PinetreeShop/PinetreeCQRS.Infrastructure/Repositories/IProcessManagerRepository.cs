using System;

namespace PinetreeCQRS.Infrastructure.Repositories
{
    public interface IProcessManagerRepository
    {
        void SaveProcessManager<TProcessManager>(TProcessManager processManager) where TProcessManager : IProcessManager;
        TProcessManager GetProcessManagerById<TProcessManager>(Guid id) where TProcessManager : IProcessManager, new();
    }
}
