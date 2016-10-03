using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using System;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public interface IWorkflowRepository
    {
        void SaveWorkflow<TWorkflow>(TWorkflow workflow) where TWorkflow : IWorkflow;
        TResult GetWorkflowById<TResult>(Guid id) where TResult : IWorkflow, new();
    }
}
