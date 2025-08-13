using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using RMS.Web.Core.ViewModels.Home;
using RMS.Web.Core.ViewModels.Item;

namespace RMS.Web.Core.Mapping;

public class MappingProfile : Profile
{

    public MappingProfile()
    {
        CreateMap<Item, ItemDetailsViewModel>()
       .ForMember(dest => dest.BasePrice,
           opt => opt.MapFrom(src =>
               src.BranchItems.FirstOrDefault() != null
                   ? src.BranchItems.First().BasePrice
                   : 0m))
       .ForMember(dest => dest.AllergyId, opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.Id : 0))
       .ForMember(dest => dest.AllergyNameAr, opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.NameAr : null))
       .ForMember(dest => dest.AllergyNameEn, opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.NameEn : null))
       .ForMember(dest => dest.AllergyImageUrl, opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.ImageUrl : null))
       .ForMember(dest => dest.ItemToppingGroups, opt => opt.MapFrom(src => src.ItemToppingGroups));

        CreateMap<ItemToppingGroup, ItemToppingGroupViewModel>()
            .ForMember(dest => dest.TitleAr, opt => opt.MapFrom(src => src.ToppingGroup.TitleAr))
            .ForMember(dest => dest.TitleEn, opt => opt.MapFrom(src => src.ToppingGroup.TitleEn))
            .ForMember(dest => dest.ToppingOptions, opt => opt.MapFrom(src => src.ToppingGroup.ToppingOptions));

        CreateMap<ToppingOption, ToppingOptionViewModel>();

    }
}



