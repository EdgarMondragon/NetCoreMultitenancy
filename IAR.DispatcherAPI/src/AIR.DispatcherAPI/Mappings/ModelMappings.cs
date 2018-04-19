using AutoMapper;
using Entities;
using IAR.DispatcherAPI.Models;

namespace IAR.DispatcherAPI.Mappings
{
    public class ModelMappings : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Message, DispatchMessage>()
                .ForMember(m => m.Destinationemailaddress, map => map.MapFrom(dm => dm.Destinationemailaddress))
                .ForMember(m => m.Originationemailaddress, map => map.MapFrom(dm => dm.Originationemailaddress))
                .ForMember(m => m.Subscriberid, map => map.MapFrom(dm => dm.Subscriberid))
                .ForMember(m => m.Arrivedon, map => map.MapFrom(dm => dm.Arrivedon))
                .ForMember(m => m.Id, map => map.MapFrom(dm => dm.Id))
                .ForMember(m => m.Messagebody, map => map.MapFrom(dm => dm.Messagebody))
                .ForMember(m => m.Messageheader, map => map.MapFrom(dm => dm.Messageheader))
                .ForMember(m => m.Messagesubject, map => map.MapFrom(dm => dm.Messagesubject))
                .ForMember(m => m.XReceiver, map => map.MapFrom(dm => dm.XReceiver))
                .ForMember(m => m.XSender, map => map.MapFrom(dm => dm.XSender));
        }
    }
}
