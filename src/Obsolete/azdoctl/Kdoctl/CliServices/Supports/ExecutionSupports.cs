
using System;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Supports
{
    public static class ExecutionSupports
    {
        public async static Task<bool> Retry(
            Func<Task> action, 
            Action<Exception> onException,
            int exponentialBackoffFactor = 5000, // 5 secs
            int retryCount = 3)
        {
            var attempt = 0;
            bool errorOccured;
            do
            {
                try
                {
                    errorOccured = false;
                    await Task.Delay(attempt * exponentialBackoffFactor);
                    await action();
                    return true;
                }
                catch (Exception exception)
                {
                    errorOccured = true;
                    onException(exception);
                }
            }
            while (errorOccured && ++attempt < retryCount);
            return false;
        }
    }
}