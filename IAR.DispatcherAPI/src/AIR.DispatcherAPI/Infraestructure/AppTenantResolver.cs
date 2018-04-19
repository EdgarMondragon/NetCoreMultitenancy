using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Entities.Tenant;
using IAR.DispatcherAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;

namespace IAR.DispatcherAPI.Infraestructure
{
    public class CachingAppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> tenants;
        private readonly MultitenancyOptions multitenancy;
        public CachingAppTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory, IOptions<MultitenancyOptions> options)
            : base(cache, loggerFactory)
        {
            //this.multitenancy = new MultitenancyOptions {Tenants = new Collection<AppTenant>()};
            //multitenancy.Tenants.Add(new AppTenant
            //    {
            //        Name = options.Value.Name,
            //        ConnectionString = options.Value.ConnectionString,
            //        Hostnames = options.Value.Hostnames
            //});
            //multitenancy.Tenants.Add(new AppTenant
            //{
            //    Name = optcanada.Value.Name,
            //    ConnectionString = optcanada.Value.ConnectionString,
            //    Hostnames = optcanada.Value.Hostnames
            //});
            this.tenants = options.Value.Tenants;
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            var identifier = context.Request.Headers["ServerProvider"].ToString().ToUpper();
            if (string.IsNullOrEmpty(identifier)) return "USA";
            return identifier;

        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.Hostnames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;
            
            var provider = context.Request.Headers["Authorization"].ToString();
            if(!string.IsNullOrEmpty(provider))
                provider = provider.Substring(provider.Length - 2).Replace("ApiKey", "").Trim();

            var tenant = tenants.FirstOrDefault(t => t.Name.Contains(provider));

            tenantContext = tenant != null ? new TenantContext<AppTenant>(tenant) : new TenantContext<AppTenant>(tenants.FirstOrDefault(t => t.Name.ToUpper().Contains("USA")));

            return Task.FromResult(tenantContext);
        }

        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return base.CreateCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }
    }
}
