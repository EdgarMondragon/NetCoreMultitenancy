using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IAR.DispatcherAPI.Core
{
    public interface IApplicationContext
    {
        ApiUser CurrentUser { get; set; }
        bool Unrestricted { get; set; }
    }
}
