namespace DatingApp.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this string dob)
        {
            if(dob == null) return 0;

            DateTime date = DateTime.Parse(dob);
            var today = DateTime.UtcNow;

            var age = today.Year - date.Year;

            if (date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
