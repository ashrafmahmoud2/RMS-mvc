using RMS.Web.Core.ViewModels.Branches;
using RMS.Web.Core.ViewModels.GovernateAreaBranch;
using RMS.Web.Services.Interfaces;

namespace RMS.Web.Services.Implementations;


public class BranchService : IBranchService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
   // private readonly ICloudflareR2Service _r2Service;
    private readonly ILogger<BranchService> _logger;

    public BranchService(
        ApplicationDbContext context,
        IMapper mapper,
     //   ICloudflareR2Service r2Service,
        ILogger<BranchService> logger)
    {
        _context = context;
        _mapper = mapper;
       // _r2Service = r2Service;
        _logger = logger;
    }



    public async Task<IEnumerable<BranchViewModel>> GetAllBranchesAsync()
    {
        var branches = await _context.Branches
            .AsNoTracking()
            .Include(b => b.Governorate)
            .Include(b => b.Area)
            .Include(b => b.BranchWorkingHours)
            .Include(b => b.WorkingHourExceptions)
            .Include(b => b.BranchImages)
            .OrderBy(b => b.Governorate!.NameAr)
            .ThenBy(b => b.NameAr)
            .ToListAsync();

        var branchViewModels = new List<BranchViewModel>();

        foreach (var branch in branches)
        {
            var vm = _mapper.Map<BranchViewModel>(branch);
            vm.IsCurrentlyOpen = IsBranchOpenNow(branch);
            vm.WorkingHoursStatus = GetWorkingHoursStatus(branch);
            vm.WorkingHoursText = GetWorkingHoursText(branch);
            branchViewModels.Add(vm);
        }

        return branchViewModels;
    }

    public async Task<Branch?> GetBranchByIdAsync(int id)
    {
        return await _context.Branches
            .Include(b => b.Governorate)
            .Include(b => b.Area)
            .Include(b => b.BranchWorkingHours)
            .Include(b => b.WorkingHourExceptions)
            .Include(b => b.BranchImages)
            .FirstOrDefaultAsync(b => b.Id == id);
    }


    public async Task<(bool Success, string Message, int? BranchId)> CreateBranchAsync(
        BranchFormViewModel viewModel,
        string userId)
    {
        try
        {
            // Validate uniqueness
            if (!await IsBranchNameUniqueAsync(viewModel.NameAr, viewModel.NameEn))
            {
                return (false, "Branch name already exists", null);
            }

            // Validate area belongs to governorate
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == viewModel.AreaId && a.GovernorateId == viewModel.GovernorateId);

            if (area == null)
            {
                return (false, "Selected area does not belong to the selected governorate", null);
            }

            // Validate working hours
            //var (validWorkingHours, workingHoursMessage) = ValidateWorkingHours(viewModel.WorkingHours);
            //if (!validWorkingHours)
            //{
            //    return (false, workingHoursMessage, null);
            //}

            // Validate exceptions
            //var (validExceptions, exceptionsMessage) = ValidateWorkingHourExceptions(viewModel.WorkingHourExceptions);
            //if (!validExceptions)
            //{
            //    return (false, exceptionsMessage, null);
            //}

            // Upload images
            //var imageUrls = new List<string>();
            //if (viewModel.NewImageFiles != null && viewModel.NewImageFiles.Any())
            //{
            //    try
            //    {
            //        imageUrls = await _r2Service.UploadMultipleFilesAsync(viewModel.NewImageFiles, "branch-images");
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Failed to upload branch images");
            //        return (false, $"Failed to upload images: {ex.Message}", null);
            //    }
            //}

            // Create branch entity
            var branch = _mapper.Map<Branch>(viewModel);
            branch.CreatedById = userId;
            branch.CreatedOn = DateTime.UtcNow;

            // Add images
            //foreach (var imageUrl in imageUrls)
            //{
            //    branch.BranchImages.Add(new BranchImage { ImageUrl = imageUrl });
            //}

            // Add working hours
            if (viewModel.WorkingHours != null)
            {
                foreach (var wh in viewModel.WorkingHours)
                {
                    branch.BranchWorkingHours.Add(new BranchWorkingHour
                    {
                        DayOfWeek = wh.DayOfWeek,
                        OpeningTime = wh.OpeningTime,
                        ClosingTime = wh.ClosingTime
                    });
                }
            }

            // Add exceptions
            if (viewModel.WorkingHourExceptions != null)
            {
                foreach (var ex in viewModel.WorkingHourExceptions)
                {
                    branch.WorkingHourExceptions.Add(new BranchWorkingHourException
                    {
                        BranchId = branch.Id,
                        ExceptionNameAr = ex.ExceptionNameAr,
                        ExceptionNameEn = ex.ExceptionNameEn,
                        StartDate = ex.StartDate,
                        EndDate = ex.EndDate,
                        IsClosedAllDay = ex.IsClosedAllDay,
                        IsOpen24Hours = ex.IsOpen24Hours,
                        OpeningTime = ex.OpeningTime ,
                        ClosingTime = ex.ClosingTime 
                    });
                }
            }

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Branch created successfully: {BranchId}", branch.Id);
            return (true, "Branch created successfully", branch.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating branch");
            return (false, "Database error occurred. Please check for duplicate names.", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating branch");
            return (false, $"An unexpected error occurred: {ex.Message}", null);
        }
    }

    public async Task<BranchFormViewModel?> GetBranchForEditAsync(int id)
    {
        var branch = await GetBranchByIdAsync(id);
        if (branch == null)
            return null;

        var viewModel = _mapper.Map<BranchFormViewModel>(branch);

        // Map existing images
        viewModel.ExistingBranchImagePaths = branch.BranchImages
            .Select(img => img.ImageUrl)
            .ToList();

        // Map working hours
        viewModel.WorkingHours = branch.BranchWorkingHours
            .Select(wh => _mapper.Map<BranchWorkingHoursFormViewModel>(wh))
            .ToList();

        // Map exceptions
        viewModel.WorkingHourExceptions = branch.WorkingHourExceptions
            .Select(ex => _mapper.Map<BranchExceptionHoursFormViewModel>(ex))
            .ToList();

        return viewModel;
    }


    public async Task<(bool Success, string Message)> UpdateBranchAsync(
        int id,
        BranchFormViewModel viewModel,
        string userId)
    {
        try
        {
            var branch = await _context.Branches
                .Include(b => b.BranchImages)
                .Include(b => b.BranchWorkingHours)
                .Include(b => b.WorkingHourExceptions)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                return (false, "Branch not found");
            }

            // Validate uniqueness (excluding current branch)
            if (!await IsBranchNameUniqueAsync(viewModel.NameAr, viewModel.NameEn, id))
            {
                return (false, "Branch name already exists");
            }

            // Validate area belongs to governorate
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == viewModel.AreaId && a.GovernorateId == viewModel.GovernorateId);

            if (area == null)
            {
                return (false, "Selected area does not belong to the selected governorate");
            }

            // Validate working hours
            //var (validWorkingHours, workingHoursMessage) = ValidateWorkingHours(viewModel.WorkingHours);
            //if (!validWorkingHours)
            //{
            //    return (false, workingHoursMessage);
            //}

            // Validate exceptions
            //var (validExceptions, exceptionsMessage) = ValidateWorkingHourExceptions(viewModel.WorkingHourExceptions);
            //if (!validExceptions)
            //{
            //    return (false, exceptionsMessage);
            //}

            // Handle image uploads
            //if (viewModel.NewImageFiles != null && viewModel.NewImageFiles.Any())
            //{
            //    try
            //    {
            //        var imageUrls = await _r2Service.UploadMultipleFilesAsync(viewModel.NewImageFiles, "branch-images");

            //        foreach (var imageUrl in imageUrls)
            //        {
            //            branch.BranchImages.Add(new BranchImage { ImageUrl = imageUrl });
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Failed to upload branch images");
            //        return (false, $"Failed to upload images: {ex.Message}");
            //    }
            //}

            // Update basic properties
            branch.NameEn = viewModel.NameEn;
            branch.NameAr = viewModel.NameAr;
            branch.AreaId = viewModel.AreaId;
            branch.GovernorateId = viewModel.GovernorateId;
            branch.AddressEn = viewModel.AddressEn;
            branch.AddressAr = viewModel.AddressAr;
            branch.Phone = viewModel.Phone;
            branch.MaxCashOnDeliveryAllowed = viewModel.MaxCashOnDeliveryAllowed;
            branch.DeliveryFee = viewModel.DeliveryFee;
            branch.DeliveryTimeInMinutes = viewModel.DeliveryTimeInMinutes;
            branch.MaxAllowedOrdersInDay = viewModel.MaxAllowedOrdersInDay;
            branch.IsBusy = viewModel.IsBusy;
            branch.IsOpen = viewModel.IsOpen;
            branch.LastUpdatedById = userId;
            branch.LastUpdatedOn = DateTime.UtcNow;

           

            branch.BranchWorkingHours.Clear();
            if (viewModel.WorkingHours != null)
            {
                foreach (var wh in viewModel.WorkingHours)
                {
                    branch.BranchWorkingHours.Add(new BranchWorkingHour
                    {
                        BranchId = branch.Id,
                        DayOfWeek = wh.DayOfWeek,
                        OpeningTime = wh.OpeningTime,
                        ClosingTime = wh.ClosingTime
                    });
                }
            }


            if (branch.WorkingHourExceptions.Any())
            {
                _context.BranchWorkingHourExceptions.RemoveRange(branch.WorkingHourExceptions);
            }
          //  branch.WorkingHourExceptions.Clear();

            if (viewModel.WorkingHourExceptions != null)
            {
                foreach (var ex in viewModel.WorkingHourExceptions)
                {
                    branch.WorkingHourExceptions.Add(new BranchWorkingHourException
                    {
                        BranchId = branch.Id,
                        ExceptionNameAr = ex.ExceptionNameAr,
                        ExceptionNameEn = ex.ExceptionNameEn,
                        StartDate = ex.StartDate,
                        EndDate = ex.EndDate,
                        IsClosedAllDay = ex.IsClosedAllDay,
                        IsOpen24Hours = ex.IsOpen24Hours,
                        OpeningTime = ex.OpeningTime,
                        ClosingTime = ex.ClosingTime
                    });
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Branch updated successfully: {BranchId}", branch.Id);
            return (true, "Branch updated successfully");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error while updating branch");
            return (false, "The branch was modified by another user. Please refresh and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating branch");
            return (false, $"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> DeleteBranchAsync(int id)
    {
        try
        {
            var branch = await _context.Branches
                .Include(b => b.BranchImages)
                 .Include(b => b.BranchWorkingHours) 
                .Include(b => b.WorkingHourExceptions)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                return (false, "Branch not found");
            }

            // Check if branch has any orders
            var hasOrders = await _context.Orders.AnyAsync(o => o.BranchId == id);
            if (hasOrders)
            {
                return (false, "Cannot delete branch with existing orders");
            }

            // Delete images from R2
            //if (branch.BranchImages.Any())
            //{
            //    var imageUrls = branch.BranchImages.Select(img => img.ImageUrl).ToList();
            //    await _r2Service.DeleteMultipleFilesAsync(imageUrls);
            //}

            if (branch.BranchWorkingHours.Any())
            {
                _context.BranchWorkingHours.RemoveRange(branch.BranchWorkingHours);
            }

            if (branch.WorkingHourExceptions.Any())
            {
                _context.BranchWorkingHourExceptions.RemoveRange(branch.WorkingHourExceptions);
            }
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Branch deleted successfully: {BranchId}", id);
            return (true, "Branch deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting branch");
            return (false, $"An error occurred: {ex.Message}");
        }
    }



    public async Task<(bool Success, bool IsOpen)> ToggleBranchStatusAsync(int branchId)
    {
        var branch = await _context.Branches.FindAsync(branchId);
        if (branch == null)
            return (false, false);

        branch.IsOpen = !branch.IsOpen;
        await _context.SaveChangesAsync();

        return (true, branch.IsOpen);
    }

    public async Task<(bool Success, bool IsBusy)> ToggleBranchBusyAsync(int branchId)
    {
        var branch = await _context.Branches.FindAsync(branchId);
        if (branch == null)
            return (false, false);

        branch.IsBusy = !branch.IsBusy;
        await _context.SaveChangesAsync();

        return (true, branch.IsBusy);
    }



    public async Task<IEnumerable<BranchWorkingHourViewModel>> GetWorkingHoursAsync(int branchId)
    {
        var workingHours = await _context.BranchWorkingHours
            .Where(wh => wh.BranchId == branchId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<BranchWorkingHourViewModel>>(workingHours);
    }

    public async Task<(bool Success, string Message)> SaveWorkingHoursAsync(
        int branchId,
        IEnumerable<BranchWorkingHourViewModel> workingHours)
    {
        var (valid, message) = ValidateWorkingHours(workingHours);
        if (!valid)
            return (false, message);

        var branch = await _context.Branches
            .Include(b => b.BranchWorkingHours)
            .FirstOrDefaultAsync(b => b.Id == branchId);

        if (branch == null)
            return (false, "Branch not found");

        branch.BranchWorkingHours.Clear();

        foreach (var wh in workingHours)
        {
            branch.BranchWorkingHours.Add(new BranchWorkingHour
            {
                BranchId = branchId,
                DayOfWeek = wh.DayOfWeek,
                OpeningTime = wh.OpeningTime,
                ClosingTime = wh.ClosingTime
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Working hours saved successfully");
    }

    public async Task<(bool Success, string Message)> DeleteWorkingHourAsync(int id)
    {
        var workingHour = await _context.BranchWorkingHours.FindAsync(id);
        if (workingHour == null)
            return (false, "Working hour not found");

        _context.BranchWorkingHours.Remove(workingHour);
        await _context.SaveChangesAsync();

        return (true, "Working hour deleted successfully");
    }

    public async Task<IEnumerable<BranchWorkingHourExceptionViewModel>> GetWorkingHourExceptionsAsync(int branchId)
    {
        var exceptions = await _context.BranchWorkingHourExceptions
            .Where(ex => ex.BranchId == branchId)
            .OrderBy(ex => ex.StartDate)
            .ThenBy(ex => ex.EndDate)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<BranchWorkingHourExceptionViewModel>>(exceptions);
    }


    public async Task<(bool Success, string Message)> AddWorkingHourExceptionAsync(BranchWorkingHourExceptionViewModel exceptionViewModel)
    {
        if (exceptionViewModel is null)
            return (false, "Invalid exception data");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Validate date range
        if (exceptionViewModel.StartDate < today)
            return (false, "Start date cannot be in the past");

        if (exceptionViewModel.EndDate < exceptionViewModel.StartDate)
            return (false, "End date must be after start date");

        // Validate time range (only if not closed/open 24h)
        if (!exceptionViewModel.IsClosedAllDay && !exceptionViewModel.IsOpen24Hours)
        {
            if (exceptionViewModel.OpeningTime is null || exceptionViewModel.ClosingTime is null)
                return (false, "Opening and closing times are required unless the branch is open 24 hours or closed all day");

            if (exceptionViewModel.OpeningTime >= exceptionViewModel.ClosingTime)
                return (false, "Opening time must be before closing time");
        }

        // Prevent duplicates for overlapping date ranges
        bool exists = await _context.BranchWorkingHourExceptions.AnyAsync(ex =>
            ex.BranchId == exceptionViewModel.BranchId &&
            ex.StartDate <= exceptionViewModel.EndDate &&
            ex.EndDate >= exceptionViewModel.StartDate);

        if (exists)
            return (false, "An exception already exists within this date range");

        // Map and save
        var exception = _mapper.Map<BranchWorkingHourException>(exceptionViewModel);

        await _context.BranchWorkingHourExceptions.AddAsync(exception);
        await _context.SaveChangesAsync();

        return (true, "Exception added successfully");
    }

    public async Task<(bool Success, string Message)> DeleteWorkingHourExceptionAsync(int id)
    {
        var exception = await _context.BranchWorkingHourExceptions.FindAsync(id);
        if (exception == null)
            return (false, "Exception not found");

        _context.BranchWorkingHourExceptions.Remove(exception);
        await _context.SaveChangesAsync();

        return (true, "Exception deleted successfully");
    }


    public bool IsBranchOpenNow(Branch branch)
    {
        if (branch.IsBusy || !branch.IsOpen)
            return false;

        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        // ✅ Check if there's an active exception for today
        var exception = branch.WorkingHourExceptions
            .FirstOrDefault(e => today >= e.StartDate && today <= e.EndDate);

        if (exception != null)
        {
            // Case 1: branch closed all day
            if (exception.IsClosedAllDay)
                return false;

            // Case 2: branch open 24h
            if (exception.IsOpen24Hours)
                return true;

            // Case 3: normal custom hours
            return IsTimeInRange(currentTime.ToTimeSpan(), exception.OpeningTime, exception.ClosingTime);
        }

        // ✅ Check regular working hours
        var todayWorkingHours = branch.BranchWorkingHours
            .FirstOrDefault(wh => wh.DayOfWeek == now.DayOfWeek);

        if (todayWorkingHours == null)
            return false;

        return IsTimeInRange(currentTime.ToTimeSpan(), todayWorkingHours.OpeningTime, todayWorkingHours.ClosingTime);
    }

    public string GetWorkingHoursStatus(Branch branch)
    {
        var isOpen = IsBranchOpenNow(branch);
        return isOpen ? "مفتوح" : "مغلق";
    }

    public string GetWorkingHoursText(Branch branch)
    {
        if (branch.IsBusy)
            return "مغلق مؤقتاً";

        if (!branch.IsOpen)
            return "مغلق";

        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        // ✅ Check exceptions first
        var exception = branch.WorkingHourExceptions
            .FirstOrDefault(e => today >= e.StartDate && today <= e.EndDate);

        if (exception != null)
        {
            if (exception.IsClosedAllDay)
                return $"مغلق - {exception.ExceptionNameAr}";

            if (exception.IsOpen24Hours)
                return $"مفتوح 24 ساعة - {exception.ExceptionNameAr}";

            if (IsTimeInRange(currentTime.ToTimeSpan(), exception.OpeningTime, exception.ClosingTime))
                return $"مفتوح حتى {FormatTime(exception.ClosingTime)} - {exception.ExceptionNameAr}";

            return $"مغلق - {exception.ExceptionNameAr}";
        }

        // ✅ Check regular working hours
        var todayWorkingHours = branch.BranchWorkingHours
            .FirstOrDefault(wh => wh.DayOfWeek == now.DayOfWeek);

        if (todayWorkingHours == null)
        {
            var nextWorkingDay = GetNextWorkingDay(branch.BranchWorkingHours, now.DayOfWeek);
            if (nextWorkingDay != null)
            {
                return $"مغلق - يفتح {GetDayName(nextWorkingDay.DayOfWeek)} {FormatTime(nextWorkingDay.OpeningTime)}";
            }
            return "مغلق";
        }

        if (IsTimeInRange(currentTime.ToTimeSpan(), todayWorkingHours.OpeningTime, todayWorkingHours.ClosingTime))
        {
            return $"مفتوح حتى {FormatTime(todayWorkingHours.ClosingTime)}";
        }
        else if (currentTime.ToTimeSpan() < todayWorkingHours.OpeningTime)
        {
            return $"مغلق - يفتح اليوم {FormatTime(todayWorkingHours.OpeningTime)}";
        }
        else
        {
            var nextWorkingDay = GetNextWorkingDay(branch.BranchWorkingHours, now.DayOfWeek);
            if (nextWorkingDay != null)
            {
                var dayName = nextWorkingDay.DayOfWeek == now.AddDays(1).DayOfWeek ? "غداً" : GetDayName(nextWorkingDay.DayOfWeek);
                return $"مغلق - يفتح {dayName} {FormatTime(nextWorkingDay.OpeningTime)}";
            }
            return "مغلق";
        }
    }


    public async Task<bool> IsBranchNameUniqueAsync(string nameAr, string nameEn, int? excludeId = null)
    {
        var query = _context.Branches.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        var exists = await query.AnyAsync(b => b.NameAr == nameAr || b.NameEn == nameEn);
        return !exists;
    }

    public async Task<bool> CanAcceptMoreOrdersAsync(int branchId)
    {
        var branch = await _context.Branches.FindAsync(branchId);
        if (branch == null || !branch.MaxAllowedOrdersInDay.HasValue)
            return true;

        var today = DateOnly.FromDateTime(DateTime.Now);
        var todayOrdersCount = await _context.Orders
            .Where(o => o.BranchId == branchId && o.CreatedOn.Date == DateTime.Today)
            .CountAsync();

        return todayOrdersCount < branch.MaxAllowedOrdersInDay.Value;
    }

    private (bool IsValid, string Message) ValidateWorkingHours(IEnumerable<BranchWorkingHourViewModel> workingHours)
    {
        if (workingHours == null || !workingHours.Any())
        {
            return (false, "At least one working hour must be specified");
        }

        // Check for duplicate days
        var duplicateDays = workingHours
            .GroupBy(wh => wh.DayOfWeek)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateDays.Any())
        {
            return (false, $"Duplicate working hours found for: {string.Join(", ", duplicateDays.Select(GetDayName))}");
        }

        // Validate time ranges
        foreach (var wh in workingHours)
        {
            if (wh.OpeningTime >= wh.ClosingTime)
            {
                return (false, $"Invalid time range for {GetDayName(wh.DayOfWeek)}: Opening time must be before closing time");
            }

            // Check if times are reasonable (e.g., not 00:00 to 00:00)
            if (wh.OpeningTime == wh.ClosingTime)
            {
                return (false, $"Invalid time range for {GetDayName(wh.DayOfWeek)}: Opening and closing times cannot be the same");
            }
        }

        return (true, string.Empty);
    }

    private (bool IsValid, string Message) ValidateWorkingHourExceptions(IEnumerable<BranchWorkingHourExceptionViewModel> exceptions)
    {
        if (exceptions == null || !exceptions.Any())
            return (true, string.Empty);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // ✅ Check for overlapping date ranges
        var overlappingRanges = exceptions
            .SelectMany(ex1 => exceptions.Where(ex2 =>
                ex1 != ex2 &&
                ex1.StartDate <= ex2.EndDate &&
                ex1.EndDate >= ex2.StartDate)
                .Select(ex2 => (ex1, ex2)))
            .FirstOrDefault();

        if (overlappingRanges != default)
        {
            return (false, $"Overlapping exception date ranges found between {overlappingRanges.ex1.StartDate} - {overlappingRanges.ex1.EndDate} and {overlappingRanges.ex2.StartDate} - {overlappingRanges.ex2.EndDate}");
        }

        // ✅ Validate each exception
        foreach (var ex in exceptions)
        {
            if (string.IsNullOrWhiteSpace(ex.ExceptionNameAr) || string.IsNullOrWhiteSpace(ex.ExceptionNameEn))
                return (false, "Exception name is required in both Arabic and English");

            if (ex.StartDate < today)
                return (false, $"Start date {ex.StartDate} cannot be in the past");

            if (ex.EndDate < ex.StartDate)
                return (false, $"End date {ex.EndDate} must be after start date {ex.StartDate}");

            // Skip time validation if closed/open 24h
            if (!ex.IsClosedAllDay && !ex.IsOpen24Hours)
            {
                if (ex.OpeningTime == null || ex.ClosingTime == null)
                    return (false, $"Opening and closing times are required for {ex.ExceptionNameEn}");

                if (ex.OpeningTime >= ex.ClosingTime)
                    return (false, $"Invalid time range for {ex.ExceptionNameEn}: opening time must be before closing time");
            }
        }

        return (true, string.Empty);
    }


    private static bool IsTimeInRange(TimeSpan currentTime, TimeSpan openTime, TimeSpan closeTime)
    {
        // Handle cases where close time is next day (e.g., 22:00 - 02:00)
        if (closeTime < openTime)
        {
            return currentTime >= openTime || currentTime <= closeTime;
        }
        return currentTime >= openTime && currentTime <= closeTime;
    }

    private static BranchWorkingHour? GetNextWorkingDay(ICollection<BranchWorkingHour> workingHours, DayOfWeek currentDay)
    {
        for (int i = 1; i <= 7; i++)
        {
            var nextDay = (DayOfWeek)(((int)currentDay + i) % 7);
            var workingHour = workingHours.FirstOrDefault(wh => wh.DayOfWeek == nextDay);
            if (workingHour != null)
                return workingHour;
        }
        return null;
    }

    private static string FormatTime(TimeSpan time)
    {
        var hours = time.Hours;
        var minutes = time.Minutes;
        var ampm = hours >= 12 ? "م" : "ص";

        if (hours > 12) hours -= 12;
        if (hours == 0) hours = 12;

        return minutes == 0 ? $"{hours} {ampm}" : $"{hours}:{minutes:D2} {ampm}";
    }

    private static string GetDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Sunday => "الأحد",
            DayOfWeek.Monday => "الإثنين",
            DayOfWeek.Tuesday => "الثلاثاء",
            DayOfWeek.Wednesday => "الأربعاء",
            DayOfWeek.Thursday => "الخميس",
            DayOfWeek.Friday => "الجمعة",
            DayOfWeek.Saturday => "السبت",
            _ => ""
        };
    }

    Task<IEnumerable<BranchWorkingHoursFormViewModel>> IBranchService.GetWorkingHourExceptionsAsync(int branchId)
    {
        throw new NotImplementedException();
    }

    public Task<(bool Success, string Message)> AddWorkingHourExceptionAsync(BranchExceptionHoursFormViewModel exception)
    {
        throw new NotImplementedException();
    }

}