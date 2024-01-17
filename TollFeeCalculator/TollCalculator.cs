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

        /// <summary>
        /// Calculates total toll fee for one vehicle for one day.
        /// </summary>
        /// <param name="vehicle">The vehicle that passes.</param>
        /// <param name="dates">The datetime(-s) the vehicle passes</param>
        /// <returns>An int representing the total toll fee for a vehicle that day.</returns>
        public static int GetTollFee(Vehicle vehicle, DateTime[] dates)
        {
            DateTime intervalStart = dates[0];
            int totalFee = 0;
            foreach (DateTime date in dates)
            {
                int nextFee = GetTollFee(date, vehicle);
                int tempFee = GetTollFee(intervalStart, vehicle);

                long diffInMillies = date.Millisecond - intervalStart.Millisecond;
                long minutes = diffInMillies / 1000 / 60;

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

        /// <summary>
        /// Evaluates if a vehicle is toll free.
        /// </summary>
        /// <param name="vehicle">The vehicle to be evaluated.</param>
        /// <returns>True or false depending on whether the vehicle is toll free.</returns>
        public static bool IsTollFreeVehicle(Vehicle vehicle)
        {
            if (vehicle.IsTollFree) return true;
            return false;
        }

        /// <summary>
        /// Calculates tariff depending on what time the vehicle passes.
        /// </summary>
        /// <param name="time">The time the vehicle passes.</param>
        /// <param name="vehicle">The vehicle that passes.</param>
        /// <returns>An int representing the tariff in SEK for that specific time.</returns>
        public static int GetTollFee(DateTime time, Vehicle vehicle)
        {
            if (IsTollFreeDate(time) || IsTollFreeVehicle(vehicle)) return (int)Tariffs.Free;

            int hour = time.Hour;
            int minute = time.Minute;

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


        public static bool CalculateIfDateIsFixedHoliday(DateTime date)
        {
            if (date.Month == 01 && date.Day == 01) return true;
            if (date.Month == 04 && date.Day == 30) return true;
            if (date.Month == 05 && date.Day == 01) return true;
            if (date.Month == 06 && date.Day == 06) return true;
            if (date.Month == 12 && date.Day == 24) return true;
            if (date.Month == 12 && date.Day == 25) return true;
            if (date.Month == 12 && date.Day == 26) return true;
            if (date.Month == 12 && date.Day == 31) return true;

            return false;
        }

        public static bool CalculateIfDateIsNonFixedHoliday(DateTime date)
        {
            bool isHoliday = new SwedenPublicHoliday().IsPublicHoliday(date);
            if (isHoliday) return true;
            return false;
        }

        /// <summary>
        /// Evaluates if a date is a tollfree date.
        /// </summary>
        /// <param name="date">The date to be evaluated.</param>
        /// <returns>True or false depending on whether the date is toll free.</returns>
        public static bool IsTollFreeDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;
            if (date.Month == 7) return true;
            if (CalculateIfDateIsFixedHoliday(date)) return true;
            if (CalculateIfDateIsNonFixedHoliday(date)) return true;
            return false;
        }
    }
}
