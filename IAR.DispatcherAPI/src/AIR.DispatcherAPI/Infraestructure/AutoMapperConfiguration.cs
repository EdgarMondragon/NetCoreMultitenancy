using AutoMapper;
using IAR.DispatcherAPI.Mappings;

namespace IAR.DispatcherAPI.Infraestructure
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x=> x.AddProfile<ModelMappings>());
        }
    }
}
