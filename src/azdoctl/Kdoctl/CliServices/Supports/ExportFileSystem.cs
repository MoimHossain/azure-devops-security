using Kdoctl.CliOptions;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.Schema;
using Kdoctl.Schema.CliServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Supports
{
    public class ExportFileSystem
    {
        private readonly ExportOptions exOpts;
        private readonly ConsoleLogger Logger;

        public ExportFileSystem(ExportOptions exOpts, ConsoleLogger logger)
        {
            this.exOpts = exOpts;
            this.Logger = logger;
        }

        private string GetProjectRootDirectory(string projectName)
        {
            return Path.Combine(exOpts.Directory, projectName);
        }

        private string GetFilePath(string projectName, ManifestKind manifest)
        {
            return Path.Combine(GetProjectRootDirectory(projectName), $"{manifest.ToString()}.yaml");
        }

        public async Task WriteManifestAsync(AzDoServices.Dtos.Project project, ManifestKind manifest, string content)
        {
            var filePath = GetFilePath(project.Name, manifest);
            Logger.StatusBegin($"Writing file [{filePath}] ...");            

            await EnsureProjectDirectory(project);
            if(File.Exists(filePath))
            {
                Logger.Message("Deleting existing file...");
                File.Delete(filePath);
            }
            await File.WriteAllTextAsync(filePath, content);
            Logger.StatusEndSuccess("Success");
        }

        private async Task EnsureProjectDirectory(Project project)
        {
            var rootPath = GetProjectRootDirectory(project.Name);            
            var directory = new DirectoryInfo(rootPath);
            if(!directory.Exists)
            {
                Logger.Message("Creating Directory...");
                directory.Create();
            }
            await Task.CompletedTask;
        }
    }
}
