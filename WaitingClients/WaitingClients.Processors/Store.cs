using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Prism.Events;
using WaitingClients.Helpers;
using WaitingClients.Models;
using WaitingClients.Models.Events;

namespace WaitingClients.Processors
{
    public class Store : IStore
    {
        private readonly IEventAggregator _eventAggregator;
        public BlockingCollection<Client> Clients { get; set; }

        public List<IServiceProcessor> ServiceProcessors { get; } = new List<IServiceProcessor>();

        public Store(IEventAggregator eventAggregator)
        {
            Guard.ArgumentNotNull(eventAggregator, nameof(eventAggregator));

            _eventAggregator = eventAggregator;

            InitializeClientsQueue();
            GenerateClients(50);
            GenerateQueues();

            _eventAggregator.GetEvent<GenerateClientsEvent>().Subscribe(GenerateClients);
        }

        ~Store()
        {
            _eventAggregator.GetEvent<GenerateClientsEvent>().Unsubscribe(GenerateClients);
        }

        private void InitializeClientsQueue()
        {
            Clients = new BlockingCollection<Client>(new ConcurrentQueue<Client>());
        }

        private void GenerateQueues(int nr = 4)
        {
            ServiceProcessors.Clear();

            for (var i = 0; i < nr; i++)
            {
                var serviceProcessor = new ServiceProcessor(Clients, _eventAggregator);

                ServiceProcessors.Add(serviceProcessor);    
            }
        }

        public void Process()
        {
            foreach (var serviceProcessor in ServiceProcessors)
            {
                new Thread(() =>
                {
                    serviceProcessor.Process();
                }).Start();
            }
        }

        public void GenerateClients(int nr)
        {
            for (var i = 0; i < nr; i++)
            {
                var client = new Client
                {
                    ArrivalTime = DateTime.Now,
                    ProcessingTime = new Random().Next(2, 5)
                };

                Clients.Add(client);
            }
        }

        public void Stop()
        {
            foreach (var serviceProcessor in ServiceProcessors)
            {
                serviceProcessor.Stop();
            }
        }
    }
}
