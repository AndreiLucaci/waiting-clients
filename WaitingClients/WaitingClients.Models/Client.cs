using System;

namespace WaitingClients.Models
{
    public class Client
    {
        /// <summary>
        /// Arrival time of the client
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Processing time in seconds
        /// </summary>
        public int ProcessingTime { get; set; }

        /// <summary>
        /// unique <see cref="Guid"/>
        /// </summary>
        public Guid Guid { get; } = Guid.NewGuid();

        public override string ToString()
        {
            return $"[{Guid}]: {ArrivalTime} - {ProcessingTime} seconds";
        }
    }
}
