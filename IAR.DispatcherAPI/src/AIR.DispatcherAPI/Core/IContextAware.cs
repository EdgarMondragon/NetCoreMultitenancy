using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IAR.DispatcherAPI.Core
{
    public interface IContextAware
    {
        IApplicationContext Context { get; }
    }
}
