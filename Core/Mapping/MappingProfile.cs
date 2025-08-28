using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using RMS.Web.Core.ViewModels.Home;
using RMS.Web.Core.ViewModels.Item;
using RMS.Web.Core.ViewModels.Order;

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
            .ForMember(dest => dest.ToppingOptions, opt => opt.MapFrom(src => src.ToppingGroup.ToppingOptions))
            .ForMember(dest => dest.MaxAllowedOptions, opt => opt.MapFrom(src => src.ToppingGroup.MaxAllowedOptions))
            .ForMember(dest => dest.MinAllowedOptions, opt => opt.MapFrom(src => src.ToppingGroup.MinAllowedOptions));




        CreateMap<ToppingOption, ToppingOptionViewModel>();


        CreateMap<Order, OrderDetailsViewModel>()
           .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.LastStatus))
           .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.OrderDate))
           .ForMember(dest => dest.DeliveredAt, opt => opt.Ignore()) // You may calculate from StatusHistory if needed
           .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.User.FullName))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Customer.User.PhoneNumber))
           .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.CustomerAddress))
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
           .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
           .ForMember(dest => dest.DeliveryFees, opt => opt.MapFrom(src => src.DeliveryFees))
           .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
           .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.GrandTotal))
           .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payments.FirstOrDefault())); // Latest payment

        CreateMap<CustomerAddress, CustomerAddressViewModel>();

    }
}



