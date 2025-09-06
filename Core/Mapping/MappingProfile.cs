using AutoMapper;
using RMS.Web.Core.ViewModels.Home;
using RMS.Web.Core.ViewModels.Item;
using RMS.Web.Core.ViewModels.Order;

namespace RMS.Web.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ===============================
        // Item & Toppings
        // ===============================
        CreateMap<Item, ItemDetailsViewModel>()
            .ForMember(dest => dest.BasePrice,
                opt => opt.MapFrom(src =>
                    src.BranchItems.FirstOrDefault() != null
                        ? src.BranchItems.First().BasePrice
                        : 0m))
            .ForMember(dest => dest.AllergyId,
                opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.Id : 0))
            .ForMember(dest => dest.AllergyNameAr,
                opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.NameAr : null))
            .ForMember(dest => dest.AllergyNameEn,
                opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.NameEn : null))
            .ForMember(dest => dest.AllergyImageUrl,
                opt => opt.MapFrom(src => src.Allergy != null ? src.Allergy.ImageUrl : null))
            .ForMember(dest => dest.ItemToppingGroups,
                opt => opt.MapFrom(src => src.ItemToppingGroups));

        CreateMap<ItemToppingGroup, ItemToppingGroupViewModel>()
            .ForMember(dest => dest.TitleAr,
                opt => opt.MapFrom(src => src.ToppingGroup.TitleAr))
            .ForMember(dest => dest.TitleEn,
                opt => opt.MapFrom(src => src.ToppingGroup.TitleEn))
            .ForMember(dest => dest.ToppingOptions,
                opt => opt.MapFrom(src => src.ToppingGroup.ToppingOptions))
            .ForMember(dest => dest.MaxAllowedOptions,
                opt => opt.MapFrom(src => src.ToppingGroup.MaxAllowedOptions))
            .ForMember(dest => dest.MinAllowedOptions,
                opt => opt.MapFrom(src => src.ToppingGroup.MinAllowedOptions));

        CreateMap<ToppingOption, ToppingOptionViewModel>();


        // ===============================
        // Order
        // ===============================
        CreateMap<Order, OrderDetailsViewModel>()
            .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.CustomerAddress))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payments.FirstOrDefault()))
            .ForMember(dest => dest.DeliveryTimeInMinutes, opt => opt.MapFrom(src => src.Branch.DeliveryTimeInMinutes))
            .ForMember(dest => dest.BranchPhone, opt => opt.MapFrom(src => src.Branch.Phone))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.NameAr))
                .ForMember(dest => dest.OrderStatusBox, opt => opt.MapFrom(src => src)); 

        CreateMap<OrderItem, OrderItemViewModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Item.NameAr))   // Arabic name
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Item.ThumbnailUrl))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item.DescriptionAr))
            //.ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Item.ImageUrl))
            .ForMember(dest => dest.SelectedToppingGroups, opt => opt.MapFrom(src => src.ToppingGroups));

        CreateMap<SelectedToppingGroup, SelectedToppingGroupViewModel>()
            //fix why Title by null in view modal 
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ToppingGroup.TitleAr))
            .ForMember(dest => dest.SelectedToppingOptions, opt => opt.MapFrom(src => src.ToppingOptions));

        CreateMap<SelectedToppingOption, SelectedToppingOptionViewModel>()
       .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ToppingOption.NameAr))
       .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ToppingOption.ImageUrl));

        CreateMap<Order, OrderStatusBoxViewModel>()
        .ForMember(dest => dest.LastStatus, opt => opt.MapFrom(src =>
        src.StatusHistory
           .OrderByDescending(s => s.Timestamp)
           .Select(s => s.Status)
           .FirstOrDefault()))
               .ForMember(dest => dest.DeliveryTimeInMinutes, opt => opt.MapFrom(src => src.Branch.DeliveryTimeInMinutes))
               .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate));

        // ===============================
        // Payment
        // ===============================
        CreateMap<Payment, PaymentSummaryViewModel>();


        // ===============================
        // Customer & Address
        // ===============================
        CreateMap<CustomerAddress, CustomerAddressViewModel>()
      .ForMember(dest => dest.GovernrateName, opt => opt.MapFrom(src => src.Governrate.NameAr))
      .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.NameAr))
      .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.NameAr));

    }
}
