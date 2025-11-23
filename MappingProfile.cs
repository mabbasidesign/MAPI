using AutoMapper;
using MAPI.Dto;
using MAPI.Model;

namespace MAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Products, ProductsDto>();
            CreateMap<ProductsDto, Products>();
        }
    }
}
