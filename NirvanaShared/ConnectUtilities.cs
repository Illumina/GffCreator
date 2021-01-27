using System;
using System.IO;

namespace IO
{
    public static class ConnectUtilities
    {
        public static Stream ConnectWithRetries(Func<long, Stream> connect, long position, int retryCount)
        {
            while (retryCount > 0)
            {
                retryCount--;
                try
                {
                    return connect(position);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"EXCEPTION: {e.Message}");
                    if (retryCount == 0) throw;
                }
                
            }

            return null;
        }
    }
}