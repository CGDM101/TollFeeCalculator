using PublicHoliday;
using TollFeeCalculator;
using static TollFeeCalculator.TollCalculator;

namespace UnitTests
{
    public class Tests_For_GetTariffMethod
    {
        DateTime rushhourRegularDay = new DateTime(2024, 01, 09, 7, 30, 00);
        DateTime night = new DateTime(2024, 01, 09, 01, 30, 00);
        DateTime christmas16h30 = new DateTime(2023, 12, 24, 16, 30, 00); // 16.30 would be rush hour on a regular day.
        DateTime july16h30 = new DateTime(2023, 07, 24, 16, 30, 00); // 16.30 would be rush hour on a regular day.
        DateTime weekend16h30 = new DateTime(2024, 01, 13, 16, 30, 00); // 16.30 would be rush hour on a regular day.

        [Fact]
        public void GetTariff_ShouldReturnCorrectValue()
        {
            var car = new Car();

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

        [Fact]
        public void CarDuringNight() => Assert.Equal((int)Tariffs.Free, GetTariff(night, new Car()));

        [Fact]
        public void CarDuringRushHour() => Assert.Equal((int)Tariffs.High, GetTariff(rushhourRegularDay, new Car()));

        [Fact]
        public void MotorBikeDuringRushhour() => Assert.Equal((int)Tariffs.Free, GetTariff(rushhourRegularDay, new Motorbike()));

        [Fact]
        public void CarDuringWeekend() => Assert.Equal((int)Tariffs.Free, GetTariff(weekend16h30, new Car()));

        [Fact]
        public void CarDuringChristmas() => Assert.Equal((int)Tariffs.Free, GetTariff(christmas16h30, new Car()));

        [Fact]
        public void CarInJuly() => Assert.Equal((int)Tariffs.Free, GetTariff(july16h30, new Car()));
    }

    public class TestsFor_TollFreeDate_Method
    {
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

        [Fact]
        public void Test_FixedHolidayDates()
        {
            var christmas = new DateTime(2019, 12, 24);
            Assert.True(new SwedenPublicHoliday().IsPublicHoliday(christmas));
            Assert.False(new SwedenPublicHoliday().IsPublicHoliday(christmas.AddDays(-1)));
        }

        [Fact]
        public void Test_NonfixedHolidayDates()
        {
            var ascension = new DateTime(2025, 05, 29);
            Assert.True(new SwedenPublicHoliday().IsPublicHoliday(ascension));
            Assert.False(new SwedenPublicHoliday().IsPublicHoliday(ascension.AddDays(-1)));
        }

        [Fact]
        public void Test_IsTollFreeDate_WithNonTollFreeDate()
        {
            var noonRegularDay = new DateTime(2024, 01, 08, 12, 30, 00);
            Assert.False(IsTollFreeDate(noonRegularDay));
        }
    }

    public class Tests_For_CalculateTotalTollFeeMethod
    {
        // WIP: the calculation seems erroneous, with the sum 23 which is unreasonable because no combination of tariffs results in a sum of 23. However, sometimes the "sum" is 13 or 18, depending on the order of the dates in the list...
        [Fact]
        public void HighestValueSHouldBeChosenWhenSeveralPassingsInOneHour()
        {
            Car car = new Car();

            var time1 = new DateTime(2024, 01, 09, 6, 45, 00); // 13 SEK
            var time2 = new DateTime(2024, 01, 09, 7, 15, 00); // 18
            var time3 = new DateTime(2024, 01, 09, 6, 50, 00); // 13
            var time4 = new DateTime(2024, 01, 09, 7, 20, 00); // 18

            var list = new DateTime[] { time3, time4, time1, time2 };

            var expected = 18;
            var acutal = CalculateTotalTollFee(car, list); // FAIL: Actual 23. However it varies depending on the order of the dates in the list...
            Assert.Equal(expected, acutal);
        }

        [Fact]
        public void ExactTime()
        {
            var dates = new DateTime[] { new DateTime(2024, 01, 17, 7, 30, 0), new DateTime(2024, 01, 17, 7, 30, 0) };
            Assert.Equal((int)Tariffs.High, CalculateTotalTollFee(new Car(), dates)); // Bug? No car can pass a toll two times at the exact same time?
        }

        [Fact]
        public void JulyShouldCost0SEK()
        {
            var mondayInJuly = new DateTime(2024, 07, 01);
            var list = new DateTime[] { mondayInJuly };
            Assert.Equal((int)Tariffs.Free, CalculateTotalTollFee(new Car(), list));
        }

        [Fact]
        public void WeekendDatesShouldCost0SEK()
        {
            var saturday = new DateTime(2024, 01, 13);
            var sunday = new DateTime(2024, 01, 14);
            var list = new DateTime[] { saturday, sunday };
            Assert.Equal((int)Tariffs.Free, CalculateTotalTollFee(new Car(), list));
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

            var dates = new DateTime[] { newYearsDay, valborg, labourDay, nationalDay, christmas, christmasDay, boxingDay, newYearsEve, ascension }; // Bug that several days can be calculated?

            Assert.Equal((int)Tariffs.Free, CalculateTotalTollFee(new Car(), dates));
        }
    }

    public class TollFreVehicles
    {
        [Fact]
        public void MotorbikeShouldBeTollFree() => Assert.True(new Motorbike().IsTollFree);

        [Fact]
        public void CarShouldNotBeTollFree() => Assert.False(new Car().IsTollFree);
    }

    public class RandomTests
    {
        [Fact]
        public void TestDateThatWillNeverExist()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                GetTariff(new DateTime(2024, 02, 30, 7, 30, 00), new Car());
            });
        }
    }
}