

using Didactic.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Didactic.ApiServices
{
    public class ProjectApiService : BaseApiService
    {
        public ProjectApiService()
        {
        }

        protected async override Task ExecuteCoreAsync(BaseSchema baseSchema, string manifest)
        {
            await Task.CompletedTask;
        }
    }
}
