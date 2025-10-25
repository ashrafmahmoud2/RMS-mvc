using AutoMapper;
using RMS.Web.Core.ViewModels.Branches;
using RMS.Web.Core.ViewModels.GovernateAreaBranch;
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

        CreateMap<Order, OrderListViewModel>();

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



        // ===============================
        //    Branch
        // ===============================

        CreateMap<Branch, BranchViewModel>()
           .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.NameAr))
           .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.NameEn))
           .ForMember(dest => dest.AddressAr, opt => opt.MapFrom(src => src.AddressAr))
           .ForMember(dest => dest.AddressEn, opt => opt.MapFrom(src => src.AddressEn))
           .ForMember(dest => dest.GovernorateNameAr, opt => opt.MapFrom(src => src.Governorate!.NameAr))
           .ForMember(dest => dest.GovernorateNameEn, opt => opt.MapFrom(src => src.Governorate!.NameEn))
           .ForMember(dest => dest.AreaNameAr, opt => opt.MapFrom(src => src.Area!.NameAr))
           .ForMember(dest => dest.AreaNameEn, opt => opt.MapFrom(src => src.Area!.NameEn))
           .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.BranchWorkingHours))
           .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
                src.BranchImages
                   .OrderBy(img => img.DisplayOrder) 
                   .Select(img => img.ImageUrl)
                   .ToList()
            ));

        CreateMap<Branch, BranchFormViewModel>()
                .ForMember(dest => dest.ExistingBranchImagePaths,
                    opt => opt.MapFrom(src => src.BranchImages.Select(img => img.ImageUrl).ToList()))
                .ForMember(dest => dest.WorkingHours,
                    opt => opt.MapFrom(src => src.BranchWorkingHours))
                .ForMember(dest => dest.WorkingHourExceptions,
                    opt => opt.MapFrom(src => src.WorkingHourExceptions))
                .ForMember(dest => dest.NewImageFiles, opt => opt.Ignore())
                .ForMember(dest => dest.GovernorateList, opt => opt.Ignore())
                .ForMember(dest => dest.AreaList, opt => opt.Ignore());

        CreateMap<BranchFormViewModel, Branch>()
             .ForMember(dest => dest.BranchImages, opt => opt.Ignore())
             .ForMember(dest => dest.BranchWorkingHours, opt => opt.Ignore())
             .ForMember(dest => dest.WorkingHourExceptions, opt => opt.Ignore())
             .ForMember(dest => dest.Area, opt => opt.Ignore())
             .ForMember(dest => dest.Governorate, opt => opt.Ignore())
             .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
             .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
             .ForMember(dest => dest.LastUpdatedById, opt => opt.Ignore())
             .ForMember(dest => dest.LastUpdatedOn, opt => opt.Ignore());



        CreateMap<BranchWorkingHour, BranchWorkingHoursFormViewModel>().ReverseMap();

        CreateMap<BranchWorkingHour, BranchWorkingHourViewModel>().ReverseMap();

        CreateMap<BranchWorkingHourException, BranchExceptionHoursFormViewModel>()
            .ReverseMap();
        CreateMap<BranchWorkingHourException, BranchWorkingHourExceptionViewModel>()
            
            .ReverseMap();

    }
}
