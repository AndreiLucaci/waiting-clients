using System;

namespace WaitingClients.Helpers
{
    public class Guard
    {
        public static void ArgumentNotNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName, "Provided parameter should not be null");
            }
        }
    }
}
