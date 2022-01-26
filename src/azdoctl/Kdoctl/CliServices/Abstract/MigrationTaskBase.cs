

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices
{
    public abstract class MigrationTaskBase : TaskBase
    {
        public MigrationTaskBase(IServiceProvider services) : base(services)
        {
                        
        }
    }
}
