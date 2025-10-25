namespace RMS.Web.Core.Consts
{
    public static class RegexPatterns
    {
        public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
        public const string Username = "^[a-zA-Z0-9-._@+]*$";
        public const string CharactersOnly_Eng = "^[a-zA-Z-_ ]*$";
        public const string CharactersOnly_Ar = "^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FF ]*$";
        public const string NumbersAndChrOnly_ArEng = "^(?=.*[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z])[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z0-9 _-]+$";
        public const string DenySpecialCharacters = "^[^<>!#%$]*$";
        public const string MobileNumber = "^01[0,1,2,5]{1}[0-9]{8}$";
        public const string NationalId = "^[2,3]{1}[0-9]{13}$";

        // 🚨 ADDED PATTERN FOR EGYPTIAN PHONE NUMBERS (Mobile: 11-digit, Landline: 8 or 9-digit) 🚨

        public const string EgyptianPhoneNumber = @"^(01[0-9]{9}|0(2|3|4[0458]|5[056]|6[24589]|8[2369]|9[367]|13|82|86|88|93|96|97)[0-9]{6,7}|[0-9]{3,5})$";

    }
}