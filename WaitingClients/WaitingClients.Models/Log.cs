using System;
using WaitingClients.Helpers;

namespace WaitingClients.Models
{
    public class Log
    {
        public Client Client { get; }
        public string Message { get; }
        public Guid QueueGuid { get; }

        public Log(Client client, string message, Guid queueGuid)
        {
            Guard.ArgumentNotNull(client, nameof(client));

            Client = client;
            Message = message;
            QueueGuid = queueGuid;
        }
    }
}
