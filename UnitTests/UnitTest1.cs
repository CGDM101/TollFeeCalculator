using TollFeeCalculator;
using static TollFeeCalculator.TollCalculator;

namespace UnitTests
{
    public class TariffTests
    {
        Car car = new Car();

        [Fact]
        public void GetTariff_ShouldReturnCorrectValue()
        {
            Assert.Equal(8, GetTariff(new DateTime(2024, 01, 17, 6, 00, 00), car));
            Assert.Equal(8, GetTariff(new DateTime(2024, 01, 17, 6, 29, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 6, 30, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 6, 59, 00), car));
            Assert.Equal(18, GetTariff(new DateTime(2024, 01, 17, 7, 00, 00), car));
            Assert.Equal(18, GetTariff(new DateTime(2024, 01, 17, 7, 59, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 8, 00, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 8, 29, 00), car));
            Assert.Equal(8, GetTariff(new DateTime(2024, 01, 17, 8, 30, 00), car));
            Assert.Equal(8, GetTariff(new DateTime(2024, 01, 17, 14, 59, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 15, 00, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 15, 29, 00), car));
            Assert.Equal(18, GetTariff(new DateTime(2024, 01, 17, 15, 30, 00), car));
            Assert.Equal(18, GetTariff(new DateTime(2024, 01, 17, 16, 59, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 17, 00, 00), car));
            Assert.Equal(13, GetTariff(new DateTime(2024, 01, 17, 17, 59, 00), car));
            Assert.Equal(8, GetTariff(new DateTime(2024, 01, 17, 18, 00, 00), car));
            Assert.Equal(8, GetTariff(new DateTime(2024, 01, 17, 18, 29, 00), car));
            Assert.Equal(0, GetTariff(new DateTime(2024, 01, 17, 18, 30, 00), car));
            Assert.Equal(0, GetTariff(new DateTime(2024, 01, 17, 05, 59, 00), car));
        }
    }

    public class HolidayTests
    {
        Car car = new Car();

        [Fact]
        public void DayAfterAndBeforeHolidayShouldNotBeTollFree()
        {
            var dayBeforeAscension = new DateTime(2025, 05, 29).AddDays(-1); // a thursday
            var dayBeforeCristmasEve2021 = new DateTime(2021, 12, 24).AddDays(-1); // a tuesday
            var dayAfterBoxingDay2023 = new DateTime(2023, 12, 26).AddDays(1); // a wednesday

            Assert.False(IsTollFreeDate(dayBeforeCristmasEve2021));
            Assert.False(IsTollFreeDate(dayAfterBoxingDay2023));
            Assert.False(IsTollFreeDate(dayBeforeAscension));
        }

        /// <summary>
        /// These holidays always falls on these months and days. Dates are chosen for being regular weekdays, had it not been for the holiday that year.
        /// </summary>
        [Fact]
        public void HolidayDatesShouldAlwaysCost0SEK()
        {
            var newYearsDay = new DateTime(2019, 1, 1); // a tuesday
            var valborg = new DateTime(2019, 04, 30); // a tuesday
            var labourDay = new DateTime(2019, 05, 01); // a wednesday
            var nationalDay = new DateTime(2019, 06, 05); // a thursday
            var christmas = new DateTime(2019, 12, 24); // a tuesday
            var christmasDay = new DateTime(2019, 12, 25); // a wednesday
            var boxingDay = new DateTime(2019, 12, 26); // a thursday
            var newYearsEve = new DateTime(2019, 12, 31); // a tuesday

            var ascension = new DateTime(2025, 05, 29); // a holiday that falls on different dates each year.

            var dates = new DateTime[] { newYearsDay, valborg, labourDay, nationalDay, christmas, christmasDay, boxingDay, newYearsEve, ascension };

            var expected = 0;
            var actual = CalculateTotalTollFee(car, dates);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_CalculateIfDateIsFixedHoliday()
        {
            var christmas = new DateTime(2019, 12, 24);
            var notChristmas = new DateTime(2019, 12, 23);
            var actual = CalculateIfDateIsFixedHoliday(christmas);
            var actual2 = CalculateIfDateIsFixedHoliday(notChristmas);
            var expected = true;
            var expected2 = false;
            Assert.Equal(expected, actual);
            Assert.Equal(expected2, actual2);
        }

        [Fact]
        public void Test_CalculateIfDateIsNonfixedHoliday()
        {
            var ascension = new DateTime(2025, 05, 29);
            var notAscension = new DateTime(2025, 05, 28);
            var actual = CalculateIfDateIsNonFixedHoliday(ascension);
            var actual2 = CalculateIfDateIsNonFixedHoliday(notAscension);
            var expected = true;
            var expected2 = false;
            Assert.Equal(expected, actual);
            Assert.Equal(expected2, actual2);
        }

        [Fact]
        public void JulyShouldCost0SEK()
        {
            var julyDate1 = new DateTime(2024, 07, 01);
            var julyDate2 = new DateTime(2024, 07, 02);
            var list = new DateTime[] { julyDate1, julyDate2 };
            var expected = 0;
            var actual = CalculateTotalTollFee(car, list);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WeekendDatesShouldCost0SEK()
        {
            var saturday = new DateTime(2024, 01, 13);
            var sunday = new DateTime(2024, 01, 14);
            var list = new DateTime[] { saturday, sunday };
            var expected = 0;
            var actual = CalculateTotalTollFee(car, list);
            Assert.Equal(expected, actual);
        }
    }

    public class SeveralPassings_MostExpensiveShouldCount
    {
        // TODO: the last test has data for when two different tariffs exist during on hour (for example when the vehicle has passes betwen 6.30-7.30), in that case the calculation seems erroneous, with the sum 23 which is unreasonable because no combination of tariffs results in a sum of 23. However, sometimes the "sum" is 13 or 18, depending on the order of the dates in the list...

        Car car = new Car();

        [Fact]
        public void NeverAbove60SEKForOneDay_JustRushHourMornings()
        {
            var rush1 = new DateTime(2024, 01, 09, 7, 31, 00);
            var rush2 = new DateTime(2024, 01, 09, 7, 32, 00);
            var rush3 = new DateTime(2024, 01, 09, 7, 33, 00);
            var rush4 = new DateTime(2024, 01, 09, 7, 34, 00);
            var list = new DateTime[] { rush1, rush2, rush3, rush4 }; // 18*4 = 72

            var expected = (int)Tariffs.High;
            var acutal = CalculateTotalTollFee(car, list);
            Assert.Equal(expected, acutal);
        }

        [Fact]
        public void NeverAbove60SEKForOneDay_JustTheMediumTariff()
        {
            var medium1 = new DateTime(2024, 01, 09, 8, 15, 00);
            var medium2 = new DateTime(2024, 01, 09, 8, 16, 00);
            var medium3 = new DateTime(2024, 01, 09, 8, 17, 00);
            var medium4 = new DateTime(2024, 01, 09, 8, 18, 00);
            var medium5 = new DateTime(2024, 01, 09, 8, 14, 00);
            var medium6 = new DateTime(2024, 01, 09, 8, 15, 00);

            var list = new DateTime[] { medium1, medium2, medium3, medium4, medium5, medium6 }; // 13*5 = 65

            var expected = (int)Tariffs.Medium;
            var acutal = CalculateTotalTollFee(car, list);
            Assert.Equal(expected, acutal);
        }

        [Fact]
        public void NeverAbove60SEKForOneDay_JustNoonTariff()
        {
            var noon1 = new DateTime(2024, 01, 09, 11, 31, 00);
            var noon2 = new DateTime(2024, 01, 09, 11, 32, 00);
            var noon3 = new DateTime(2024, 01, 09, 11, 33, 00);
            var noon4 = new DateTime(2024, 01, 09, 11, 34, 00);
            var noon5 = new DateTime(2024, 01, 09, 11, 35, 00);
            var noon6 = new DateTime(2024, 01, 09, 11, 36, 00);
            var noon7 = new DateTime(2024, 01, 09, 11, 37, 00);
            var noon8 = new DateTime(2024, 01, 09, 11, 38, 00);

            var list = new DateTime[] { noon1, noon2, noon3, noon4, noon5, noon6, noon7, noon8 }; // 8*8 = 64

            var expected = (int)Tariffs.Low;
            var acutal = CalculateTotalTollFee(car, list);
            Assert.Equal(expected, acutal);
        }

        [Fact]
        public void NeverAbove60SEKForOneDay_TwoDifferentTariffsDuringOneHour()
        {
            var time1 = new DateTime(2024, 01, 09, 6, 45, 00); // 13 SEK
            var time2 = new DateTime(2024, 01, 09, 7, 15, 00); // 18
            var time3 = new DateTime(2024, 01, 09, 6, 50, 00); // 13
            var time4 = new DateTime(2024, 01, 09, 7, 20, 00); // 18

            var list = new DateTime[] { time3, time4, time1, time2 }; // = 62

            var expected = 18;
            var acutal = CalculateTotalTollFee(car, list); // FAIL: Actual 23. However it varies depending on the order of the dates in the list...
            Assert.Equal(expected, acutal);
        }

        [Fact]
        public void ExactTime()
        {
            var dates = new DateTime[] { new DateTime(2024, 01, 17, 7, 30, 0), new DateTime(2024, 01, 17, 7, 30, 0) };
            var actual = CalculateTotalTollFee(car, dates);
            var expected = 18;
            Assert.Equal(expected, actual); // Bug? No car can pass a toll two times at the exact same time?
        }
    }

    public class RandomTests
    {
        Car car = new Car();
        Motorbike mc = new Motorbike();

        DateTime noonRegularDay = new DateTime(2024, 01, 08, 12, 30, 00);
        DateTime rushhourRegularDay = new DateTime(2024, 01, 09, 7, 30, 00);
        DateTime night = new DateTime(2024, 01, 09, 01, 30, 00);
        DateTime christmas = new DateTime(2023, 12, 24, 16, 30, 00); // 16.30 would be rush hour on a regular day.
        DateTime july = new DateTime(2023, 07, 24, 16, 30, 00); // 16.30 would be rush hour on a regular day.
        DateTime weekend = new DateTime(2024, 01, 13, 16, 30, 00); // 16.30 would be rush hour on a regular day.

        #region IsTollFreeDate-tester
        [Fact]
        public void Test_IsTollFreeDate_WithTollFreeDate()
        {
            var expected = true;
            var actual = IsTollFreeDate(christmas);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_IsTollFreeDate_WithNonTollFreeDate()
        {
            var expected = false;
            var actual = IsTollFreeDate(noonRegularDay);
            Assert.Equal(expected, actual);
        }
        #endregion IsTollFreeDate-tester

        #region IsTollFreeVehicle-tester
        [Fact]
        public void Test_IsTollFreeVehicle_WithMotorbike()
        {
            var result = IsTollFreeVehicle(mc);
            var expected = true;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_IsTollFreeVehicle_WithCar()
        {
            var result = IsTollFreeVehicle(car);
            var expected = false;
            Assert.Equal(expected, result);
        }
        #endregion IsTollFreeVehicle-tester

        #region GetTollFee(vehicle, date[])-tester
        [Fact]
        public void Test_GetTollFee_WithSeveralDates()
        {
            var dates = new DateTime[] { rushhourRegularDay, noonRegularDay };
            var actual = CalculateTotalTollFee(car, dates);
            var expected = 18;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithSeveralDates_ButTollFree()
        {
            var dates = new DateTime[] { christmas, weekend };
            var actual = CalculateTotalTollFee(car, dates);
            var expected = 0;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithSeveralDates_OneTollFree()
        {
            var dates = new DateTime[] { weekend, noonRegularDay };
            var actual = CalculateTotalTollFee(car, dates);
            var expected = 8;
            Assert.Equal(expected, actual);
        }
        #endregion GetTollFee(vehicle, date[])-tester

        #region GetTollFee(date, vehicle)-tester
        [Fact]
        public void Test_GetTollFee_WithCar()
        {
            var actual = GetTariff(rushhourRegularDay, car);
            var expected = (int)Tariffs.High;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithMotorbike()
        {
            var actual = GetTariff(rushhourRegularDay, mc);
            var expected = (int)Tariffs.Free;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithTollFreeDate()
        {
            var actual = GetTariff(weekend, car);
            var expected = (int)Tariffs.Free;
            Assert.Equal(expected, actual);
        }
        #endregion GetTollFee(date, vehicle)-tester

        #region Random tests
        [Fact]
        public void MotorbikeShouldAlwaysCost0SEK()
        {
            var actual = GetTariff(rushhourRegularDay, mc);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarDuringNightShouldAlwaysCost0SEK()
        {
            var actual = GetTariff(night, car);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarDuringChristmasShouldAlwaysCost0SEK()
        {
            var actual = GetTariff(christmas, car);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarInJulyShouldAlwaysCost0SEK()
        {
            var actual = GetTariff(july, car);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarDuringWeekendshouldAlwaysCost0SEK()
        {
            var actual = GetTariff(weekend, car);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDateThatWillNeverExist()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                GetTariff(new DateTime(2024, 02, 30, 7, 30, 00), car);
            });
        }
        #endregion Random tests
    }
}