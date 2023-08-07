namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly DOB)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var age = today.Year - DOB.Year;

            if (DOB > today.AddYears(-age)) age--; // birthday not happened yet so -1 age

            return age;
        }
    }
}