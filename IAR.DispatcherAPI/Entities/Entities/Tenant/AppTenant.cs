namespace Entities.Tenant
{
    public class AppTenant
    {
        public string Name { get; set; }
        public string[] Hostnames { get; set; }
        public string ConnectionString { get; set; }
    }
}
