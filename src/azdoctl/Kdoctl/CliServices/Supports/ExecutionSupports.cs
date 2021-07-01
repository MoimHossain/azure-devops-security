

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Supports
{
    public static class ExecutionSupports
    {
        public async static Task Retry(
            Func<Task> action, 
            Action<Exception> onException,
            int exponentialBackoffFactor = 5000, // 5 secs
            int retryCount = 3)
        {
            var attempt = 0;
            var errorOccured = false;
            do
            {
                try
                {
                    errorOccured = false;
                    await Task.Delay(attempt * exponentialBackoffFactor);
                    await action();
                }
                catch (Exception exception)
                {
                    errorOccured = true;
                    onException(exception);
                }
            }
            while (errorOccured && ++attempt < retryCount);
        }
    }
}