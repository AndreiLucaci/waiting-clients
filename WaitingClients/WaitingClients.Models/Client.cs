using System;
using System.Numerics;

namespace WaitingClients.Models
{
    public class Client
    {
        public Client()
        {
            var big = (new BigInteger(Guid.NewGuid().ToByteArray()) & 0xFFFFFFF);
            ProcessingTime = new Random((int)big).Next(1, 5);
        }

        /// <summary>
        /// Arrival time of the client
        /// </summary>
        public DateTime ArrivalTime { get; set; } = DateTime.Now;

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
