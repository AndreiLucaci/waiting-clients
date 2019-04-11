using System;
using System.Collections.Concurrent;
using System.Threading;
using Prism.Events;
using WaitingClients.Helpers;
using WaitingClients.Models;
using WaitingClients.Models.Events;

namespace WaitingClients.Processors
{
    public class ServiceProcessor : IServiceProcessor
    {
        private readonly BlockingCollection<Client> _clients;
        private readonly IEventAggregator _eventAggregator;
        private bool _isRunning = true;

        public ServiceProcessor(BlockingCollection<Client> clients, IEventAggregator eventAggregator)
        {
            Guard.ArgumentNotNull(clients, nameof(clients));
            Guard.ArgumentNotNull(eventAggregator, nameof(eventAggregator));

            _clients = clients;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<StopEvent>().Subscribe(Stop);
        }

        ~ServiceProcessor()
        {
            _eventAggregator.GetEvent<StopEvent>().Unsubscribe(Stop);
        }

        public void Process()
        {
            while (_isRunning)
            {
                var client = _clients.Take();

                var startTime = DateTime.Now;

                Thread.Sleep(client.ProcessingTime * 1000);

                var endTime = DateTime.Now;

                var log = CreateLog(client, startTime, endTime);

                _eventAggregator.GetEvent<LogEvent>().Publish(log);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private Log CreateLog(Client client, DateTime startTime, DateTime endTime)
        {
            var message = $"[{client.Guid}] processed at {DateTime.Now}. [AT]: {client.ArrivalTime}. [ST]: {startTime}. [ET]: {endTime}. [Process Time]: {endTime-startTime}. [Wait time]: {endTime-client.ArrivalTime}";
            
            var log = new Log(client, message, QueueGuid);

            return log;
        }

        public Guid QueueGuid { get; } = Guid.NewGuid();
    }
}
