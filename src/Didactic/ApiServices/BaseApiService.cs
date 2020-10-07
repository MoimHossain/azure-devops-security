using Didactic.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Didactic.ApiServices
{
    public abstract class BaseApiService
    {
        protected BaseApiService()
        {

        }

        public async Task ExecuteAsync(BaseSchema baseSchema, string manifest)
        {
            await ExecuteCoreAsync(baseSchema, manifest);
        }

        protected abstract Task ExecuteCoreAsync(BaseSchema baseSchema, string manifest);
    }
}
