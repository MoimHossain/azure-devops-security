using Cielo.Manifests.Common;
using Cielo.ResourceManagers.Abstract;
using Cielo.ResourceManagers.ResourceStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.CliSupports
{
    public class ReportManager
    {
        private readonly ResourceManagerBase resourceManager;
        private readonly ResourceState beforeState;
        private readonly ResourceState? afterState;

        public ReportManager(
            ResourceManagerBase resourceManager, 
            ResourceState beforeState, 
            ResourceState? afterState)
        {
            this.resourceManager = resourceManager;
            this.beforeState = beforeState;
            this.afterState = afterState;
        }

        public void Report()
        {
            var manifest = resourceManager.Manifest;
            ConsoleReport.BeginResource(manifest.Metadata.Name, manifest.Kind);
            ConsoleReport.ResourceState(beforeState.Exists);
            PrintProperties(beforeState.GetProperties());

            if (beforeState.HasErrors)
            {
                foreach (var error in beforeState.GetErrors())
                {
                    ConsoleReport.ReportError(error);
                }
            }

            if (afterState != null)
            {
                ConsoleReport.ChangeBegin(manifest.Metadata.Name, manifest.Kind);
                PrintProperties(afterState.GetProperties());

                if (afterState.HasErrors)
                {
                    foreach (var error in afterState.GetErrors())
                    {
                        ConsoleReport.ReportError(error);
                    }
                }

            }
        }

        private void PrintProperties(IEnumerable<(string, object, bool)> properties, bool beforeOrAfter = true, int indent = 1)
        {
            foreach (var property in properties)
            {
                var childProps = property.Item2 as IEnumerable<(string, object, bool)>;
                if (childProps != null)
                {
                    ConsoleReport.ReportPropertyHeader(property.Item1, beforeOrAfter, indent - 1); 
                    PrintProperties(childProps, beforeOrAfter, indent + 1);
                }
                else
                {
                    ConsoleReport.ReportProperty(property, beforeOrAfter, indent);
                }
            }
        }
    }
}
