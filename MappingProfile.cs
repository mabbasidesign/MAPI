using AutoMapper;
using Mapi.Dto;
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
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
}
