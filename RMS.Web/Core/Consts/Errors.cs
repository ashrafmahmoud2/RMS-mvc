
namespace RMS.Web.Core.Consts;

public static class Errors
{
    // 🔹 General
    public const string RequiredField = "Required field";
    public const string MaxLength = "Length cannot be more than {1} characters";
    public const string MaxMinLength = "The {0} must be at least {2} and at max {1} characters long.";
    public const string Duplicated = "Another record with the same {0} already exists!";
    public const string UnexpectedError = "An unexpected error occurred, please try again later.";
    public const string ConnectionFailed = "Failed to connect, please check your internet.";

    // 🔹 File Upload
    public const string DuplicatedBook = "Book with the same title already exists with the same author!";
    public const string NotAllowedExtension = "Only .png, .jpg, .jpeg files are allowed!";
    public const string MaxSize = "File cannot be more that 2 MB!";

    // 🔹 Dates
    public const string NotAllowFutureDates = "Date cannot be in the future!";
    public const string InvalidRange = "{0} should be between {1} and {2}!";
    public const string InvalidStartDate = "Invalid start date.";
    public const string InvalidEndDate = "Invalid end date.";

    // 🔹 Authentication / Users
    public const string ConfirmPasswordNotMatch = "The password and confirmation password do not match.";
    public const string WeakPassword = "Password must contain uppercase, lowercase, digit, and special character, and be at least 8 characters long.";
    public const string InvalidUsername = "Username can only contain letters or digits.";
    public const string OnlyEnglishLetters = "Only English letters are allowed.";
    public const string OnlyArabicLetters = "Only Arabic letters are allowed.";
    public const string OnlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";
    public const string DenySpecialCharacters = "Special characters are not allowed.";
    public const string InvalidMobileNumber = "Invalid mobile number.";
    public const string InvalidNationalId = "Invalid national ID.";
    public const string InvalidSerialNumber = "Invalid serial number.";



    // 🔹 Orders
    public const string OrderMustContainItems = "Order must contain at least one item.";
    public const string OrderInvalidTotal = "Invalid order total.";
    public const string OrderExceedsCODLimit = "Order exceeds branch COD limit.";
    public const string OrderFailedToCreate = "Failed to create order, please try again.";

    // 🔹 Branch
    public const string BranchNotFound = "Branch not found.";
    public const string BranchBusy = "Branch is currently busy, please try another branch.";
    public const string BranchClosed = "Branch is closed or invalid.";
    public const string BranchDailyLimitReached = "Branch has reached its daily order limit.";
    public const string BranchAreaGovernorateMismatch = "Branch does not belong to the selected area/governorate.";

    // 🔹 Address
    public const string InvalidAddress = "Delivery address is required.";
    public const string InvalidAreaGovernorate = "Invalid governorate/area combination.";
    public const string AddressNotFound = "Address not found.";

    // 🔹 Items & Toppings
    public const string ItemUnavailable = "Item '{0}' is not available in this branch.";
    public const string ItemOutOfStock = "Item '{0}' is out of stock or not enough quantity.";
    public const string InvalidToppingGroup = "Invalid topping group '{0}' for item '{1}'.";
    public const string InvalidToppingOption = "Topping option '{0}' is not available for '{1}'.";
    public const string ExceedsMaxToppingQuantity = "Topping option '{0}' exceeds max allowed quantity.";

    // 🔹 Payments
    public const string PaymentFailed = "Payment failed, please try again.";
    public const string InvalidCard = "Card is invalid or was declined.";
    public const string InsufficientFunds = "Insufficient funds to complete the payment.";
    public const string PaymentCanceled = "Payment was canceled.";
}

