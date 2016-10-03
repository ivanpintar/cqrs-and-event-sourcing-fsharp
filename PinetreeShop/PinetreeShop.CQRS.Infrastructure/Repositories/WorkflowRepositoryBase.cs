using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public abstract class WorkflowRepositoryBase : IWorkflowRepository
    {        
        protected int CalculateExpectedVersion<T>(IWorkflow workflow, List<T> events)
        {
            return workflow.Version - events.Count;
        }

        protected TResult BuildWorkflow<TResult>(IEnumerable<IEvent> events) where TResult : IWorkflow, new()
        {
            var result = new TResult();
            foreach (var evt in events)
            {
                result.Transition(evt);
            }

            result.ClearUncommittedEvents();
            result.ClearUndispatchedCommands();

            return result;
        }
        
        public abstract void SaveWorkflow<TWorkflow>(TWorkflow workflow) where TWorkflow : IWorkflow;
        public abstract TResult GetWorkflowById<TResult>(Guid id) where TResult : IWorkflow, new();
    }
}
