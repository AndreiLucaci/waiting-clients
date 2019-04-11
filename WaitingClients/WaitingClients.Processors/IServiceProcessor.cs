using System;

namespace WaitingClients.Processors
{
    public interface IServiceProcessor
    {
        void Process();

        void Stop();

        Guid QueueGuid { get; }
    }
}