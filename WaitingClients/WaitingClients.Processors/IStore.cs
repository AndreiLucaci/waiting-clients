using System.Collections.Concurrent;
using System.Collections.Generic;
using WaitingClients.Models;

namespace WaitingClients.Processors
{
    public interface IStore
    {
        void Process();

        void GenerateClients(int nr);

        void Stop();

        List<IServiceProcessor> ServiceProcessors { get; }
        BlockingCollection<Client> Clients { get; }
    }
}