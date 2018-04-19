using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Entities.Tenant;

namespace IAR.DispatcherAPI.Models
{
    public class MultitenancyOptions
    {
        public Collection<AppTenant> Tenants { get; set; }
    }
    public class ProviderUSA : AppTenant
    { }

    public class ProviderCANADA : AppTenant
    { }
}

