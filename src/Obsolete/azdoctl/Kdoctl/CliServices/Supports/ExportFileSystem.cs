

using Kdoctl.CliOptions;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports.Instrumentations;
using Kdoctl.Schema;
using Kdoctl.Schema.CliServices;
using System.IO;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Supports
{
    public class ExportFileSystem
    {
        private readonly ExportOptions exOpts;
        private readonly InstrumentationClient IcClient;

        public ExportFileSystem(ExportOptions exOpts, InstrumentationClient logger)
        {
            this.exOpts = exOpts;
            this.IcClient = logger;
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
            using var op = IcClient.BeginOperation($"Writing file [{filePath}] ...");            

            await EnsureProjectDirectory(project);
            if(File.Exists(filePath))
            {
                op.Message("Deleting existing file...");
                File.Delete(filePath);
            }
            await File.WriteAllTextAsync(filePath, content);
        }

        private async Task EnsureProjectDirectory(Project project)
        {   
            var rootPath = GetProjectRootDirectory(project.Name);
            using var op = IcClient.BeginOperation($"Ensure directory [{rootPath}] ...");
            var directory = new DirectoryInfo(rootPath);
            if(!directory.Exists)
            {
                op.Message("Creating Directory...");
                directory.Create();
            }
            await Task.CompletedTask;
        }
    }
}
