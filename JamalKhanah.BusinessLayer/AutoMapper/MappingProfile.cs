using AutoMapper;

namespace JamalKhanah.BusinessLayer.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        /* CreateMap<AdminModelView, ApplicationUser>()
            .ReverseMap();*/

        /* CreateMap<ProductGroup, ProductOffer>()
            .ReverseMap();

         CreateMap<Product, ProductDTO>()
             .ForMember(d => d.Name, opt => opt.MapFrom(s => s.lang == "ar" ? s.NameAr : s.NameEn))
             .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.lang == "ar" ? s.Country.NameAr : s.Country.NameEn))
             .ForMember(d => d.HaveSection, opt => opt.MapFrom(s => true))
             .ForMember(d => d.ByPiece, opt => opt.MapFrom(s => s.ProductUnit == Units.عدد ? true : false));

         CreateMap<ProductGroup, ProductDTO>()
             .ForMember(d => d.Name, opt => opt.MapFrom(s => s.lang == "ar" ? s.NameAr : s.NameEn))
             .ForMember(d => d.Description, opt => opt.MapFrom(s => s.lang == "ar" ? s.DescriptionAr : s.DescriptionEn))
             .ForMember(d => d.ByPiece, opt => opt.MapFrom(s => true));

         CreateMap<Section, SectionsDTO>()
             .ForMember(d => d.IsSection, opt => opt.MapFrom(s => true));

         CreateMap<ComplexSection, SectionsDTO>()
             .ForMember(d => d.IsSection, opt => opt.MapFrom(s => false));*/
    }
}