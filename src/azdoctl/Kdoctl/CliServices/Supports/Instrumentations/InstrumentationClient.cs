

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Supports.Instrumentations
{
    public class InstrumentationClient
    {
        private readonly TelemetryConfiguration configuration;
        private readonly TelemetryClient client;

        public InstrumentationClient(TelemetryConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.client = new TelemetryClient(configuration);            
        }

        public void InitializeSession(string taskName)
        {
            var context = client.Context;
            context.Session.Id = $"{taskName.ToUpperInvariant()}-{DateTime.UtcNow.ToString("yyyy-MMM-dd-HH-mm")}";
            context.User.Id = $"{nameof(Kdoctl)}-AutomationAccount";
            context.GlobalProperties.Add("SessionID", context.Session.Id);
            client.TrackEvent("ExecutionStarted");
        }

        public GenericOperation Begin(string message, string operationName = "NewOp")
        {
            return new GenericOperation(client, operationName).Begin(message);
        }

        public async Task<bool> FlushAsync()
        {
            var success = await client.FlushAsync(CancellationToken.None);
            // flush is not blocking when not using InMemoryChannel so wait a bit.
            // There is an active issue regarding the need for `Sleep`/`Delay`
            // which is tracked here: https://github.com/microsoft/ApplicationInsights-dotnet/issues/407
            await Task.Delay(5000);
            return success;
        }

        public class GenericOperation : IDisposable
        {
            private readonly TelemetryClient client;            
            private readonly string operationID;
            private readonly string operationName;
            private bool disposed = false;
            
            public GenericOperation(
                TelemetryClient client, string operationName)
            {
                this.client = client;
                this.operationID = Guid.NewGuid().ToString("N");
                this.operationName = operationName;
            }

            private void EndCore(string message, bool success)
            {
                if(!disposed)
                {
                    disposed = true;
                    CreateTraceWithOpContext(message);
                    var eventTelemetry = new EventTelemetry($"{operationName}-{(success ? "Succeded" : "Failed")}");
                    if(!string.IsNullOrWhiteSpace(message))
                    {
                        eventTelemetry.Properties.Add(success ? "AdditionalMessage" : "ErrorMessage", message);
                    }
                    client.TrackEvent(eventTelemetry);
                }
            }

            public void EndWithFailure(string message)
            {
                EndCore(message, false);
            }

            public void EndWithSuccess(string message)
            {
                EndCore(message, true);
            }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
            {
                EndCore("", true);                
            }

            public GenericOperation Begin(string message)
            {
                CreateTraceWithOpContext(message);
                return this;
            }

            private void CreateTraceWithOpContext(string message)
            {
                var telemetry = new TraceTelemetry { Message = message, };
                telemetry.Context.Operation.Id = operationID;
                telemetry.Context.Operation.Name = operationName;
                client.TrackTrace(telemetry);
            }
        }
    }
}
