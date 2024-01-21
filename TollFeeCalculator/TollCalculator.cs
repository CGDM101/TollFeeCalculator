using PublicHoliday;

namespace TollFeeCalculator
{
    public static class TollCalculator
    {
        /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total toll fee for that day
         */

        public static int CalculateTotalTollFee(Vehicle vehicle, DateTime[] dates)
        {
            var intervalStart = dates[0];
            var totalFee = 0;
            foreach (var date in dates)
            {
                var nextFee = GetTariff(date, vehicle!);
                var tempFee = GetTariff(intervalStart, vehicle!);

                var diffInMillies = date.Millisecond - intervalStart.Millisecond;
                var minutes = diffInMillies / 1000 / 60;

                if (minutes <= 60)
                {
                    if (totalFee > 0) totalFee -= tempFee;
                    if (nextFee >= tempFee) tempFee = nextFee;
                    totalFee += tempFee;
                }
                else
                {
                    totalFee += nextFee;
                }
            }
            if (totalFee > 60) totalFee = 60;
            return totalFee;
        }

        public static int GetTariff(DateTime time, Vehicle vehicle)
        {
            if (IsTollFreeDate(time) || vehicle.IsTollFree) return (int)Tariffs.Free;

            var hour = time.Hour;
            var minute = time.Minute;

            if (hour == 6 && minute >= 0 && minute <= 29) return (int)Tariffs.Low;
            else if (hour == 6 && minute >= 30 && minute <= 59) return (int)Tariffs.Medium;
            else if (hour == 7 && minute >= 0 && minute <= 59) return (int)Tariffs.High;
            else if (hour == 8 && minute >= 0 && minute <= 29) return (int)Tariffs.Medium;
            else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return (int)Tariffs.Low;
            else if (hour == 15 && minute >= 0 && minute <= 29) return (int)Tariffs.Medium;
            else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return (int)Tariffs.High;
            else if (hour == 17 && minute >= 0 && minute <= 59) return (int)Tariffs.Medium;
            else if (hour == 18 && minute >= 0 && minute <= 29) return (int)Tariffs.Low;
            else return (int)Tariffs.Free;
        }

        public static bool IsTollFreeDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;
            if (date.Month == 7) return true;
            if (new SwedenPublicHoliday().IsPublicHoliday(date)) return true;
            return false;
        }
    }
}
