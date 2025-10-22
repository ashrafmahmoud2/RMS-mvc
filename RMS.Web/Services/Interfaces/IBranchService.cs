using RMS.Web.Core.ViewModels.Branches;
using RMS.Web.Core.ViewModels.GovernateAreaBranch;

//1.make form(razor pages) of add/edit branches wiht his working hour  and WorkingHourExceptions have  aribic and english
//2. in image using cloude flair r2 not s3
//3. split controller to have services IBranchService , BranchService with opmtize logic and add vldations and case i forget to handel
//this the code of model and viewmodel wiht controller

namespace RMS.Web.Services.Interfaces;

public interface IBranchService
{

    // Branch CRUD
    Task<IEnumerable<BranchViewModel>> GetAllBranchesAsync();
    Task<Branch?> GetBranchByIdAsync(int id);

    Task<(bool Success, string Message, int? BranchId)> CreateBranchAsync(BranchFormViewModel viewModel, string userId);
    Task<(bool Success, string Message)> UpdateBranchAsync(int id, BranchFormViewModel viewModel, string userId);
    Task<BranchFormViewModel?> GetBranchForEditAsync(int id);


    Task<(bool Success, string Message)> DeleteBranchAsync(int id);

    //// Status Management
    Task<(bool Success, bool IsOpen)> ToggleBranchStatusAsync(int branchId);
    Task<(bool Success, bool IsBusy)> ToggleBranchBusyAsync(int branchId);

    //// Working Hours
    Task<IEnumerable<BranchWorkingHourViewModel>> GetWorkingHoursAsync(int branchId);
    Task<(bool Success, string Message)> SaveWorkingHoursAsync(int branchId, IEnumerable<BranchWorkingHourViewModel> workingHours);
    Task<(bool Success, string Message)> DeleteWorkingHourAsync(int id);

    //// Working Hour Exceptions
    Task<IEnumerable<BranchWorkingHoursFormViewModel>> GetWorkingHourExceptionsAsync(int branchId);
    Task<(bool Success, string Message)> AddWorkingHourExceptionAsync(BranchExceptionHoursFormViewModel exception);
    Task<(bool Success, string Message)> DeleteWorkingHourExceptionAsync(int id);

    //// Business Logic
    bool IsBranchOpenNow(Branch branch);
    string GetWorkingHoursStatus(Branch branch);
    string GetWorkingHoursText(Branch branch);

    //// Validation
    Task<bool> IsBranchNameUniqueAsync(string nameAr, string nameEn, int? excludeId = null);
    Task<bool> CanAcceptMoreOrdersAsync(int branchId);
}
