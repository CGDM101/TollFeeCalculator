using System.Linq;
using TollFeeCalculator;
using Xunit;

namespace UnitTests
{
    /// <summary>
    /// Tests that a non-tollfree vehicle will never have to pay >60SEk for one hour, regardless of number of times it passes the toll.
    /// </summary>
    public class TestFor_TollsAbove_60SEK
    {
        // CONCLUSION FOR THESE TESTS: The calculation for when a non toll-free vehicle tollfee is above 60SEK, does not work correctly. It seems as if only one of the passings count...
        // Further, the last test has data for when two different tariffs exist during on hour (for example when the vehicle has passes betwen 6.30-7.30), in that case some calculation seems to take place, but with the sum 23 which is unreasonable because no combination of tariffs results in a sum of 23. However, sometimes the "sum" is 13 or 18, depending on the order of the dates in the list...

        TollCalculator calculator = new TollCalculator();
        Car car = new Car();

        [Fact]
        public void NeverAbove60SEKForOneDay_JustRushHourMornings()
        {
            var rush1 = new DateTime(2024, 01, 09, 7, 31, 00);
            var rush2 = new DateTime(2024, 01, 09, 7, 32, 00);
            var rush3 = new DateTime(2024, 01, 09, 7, 33, 00);
            var rush4 = new DateTime(2024, 01, 09, 7, 34, 00);
            var list = new DateTime[] { rush1, rush2, rush3, rush4 }; // 18*4 = 72

            var expected = 60;
            var acutal = calculator.GetTollFee(car, list);
            Assert.Equal(expected, acutal); // FAIL: Actual 18. Even with three or two passes the result is 18...
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

            var expected = 60;
            var acutal = calculator.GetTollFee(car, list);
            Assert.Equal(expected, acutal); // FAIL: Actual 13. Even with three or two passes the result is 13... 
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

            var expected = 60;
            var acutal = calculator.GetTollFee(car, list);
            Assert.Equal(expected, acutal); // FAIL: Actual 8. Even with three or two passes the result is 8... 
        }

        [Fact]
        public void NeverAbove60SEKForOneDay_TwoDifferentTariffsDuringOneHour()
        {
            var time1 = new DateTime(2024, 01, 09, 6, 45, 00); // 13 SEK
            var time2 = new DateTime(2024, 01, 09, 7, 15, 00); // 18
            var time3 = new DateTime(2024, 01, 09, 6, 50, 00); // 13
            var time4 = new DateTime(2024, 01, 09, 7, 20, 00); // 18

            var list = new DateTime[] { time3, time4, time1, time2 }; // = 62

            var expected = 60;
            var acutal = calculator.GetTollFee(car, list); // FAIL: Actual 23. However it varies depending on the order of the dates in the list...
            Assert.Equal(expected, acutal);
        }
    }
    public class RandomTests
    {
        TollCalculator calculator = new TollCalculator();

        Car car = new Car();
        Motorbike mc = new Motorbike();

        DateTime noonRegularDay = new DateTime(2024, 01, 08, 12, 30, 00);
        DateTime rushhourRegularDay = new DateTime(2024, 01, 09, 7, 30, 00);
        DateTime night = new DateTime(2024, 01, 09, 01, 30, 00);
        DateTime christmas = new DateTime(2023, 12, 24, 16, 30, 00);
        DateTime july = new DateTime(2023, 07, 24, 16, 30, 00);
        DateTime weekend = new DateTime(2024, 01, 13, 16, 30, 00);

        #region IsTollFreeDate-tester
        // OBS: IsTollFreeDate() är egentligen private!
        [Fact]
        public void Test_IsTollFreeDate_WithTollFreeDate()
        {
            var expected = true;
            bool actual = calculator.IsTollFreeDate(christmas);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_IsTollFreeDate_WithNonTollFreeDate()
        {
            var expected = false;
            bool actual = calculator.IsTollFreeDate(noonRegularDay);
            Assert.Equal(expected, actual);
        }
        #endregion IsTollFreeDate-tester

        #region IsTollFreeVehicle-tester
        // OBS: IsTollFreeVehicle() är egentligen private!
        [Fact]
        public void Test_IsTollFreeVehicle_WithMotorbike()
        {
            bool result = calculator.IsTollFreeVehicle(mc);
            var expected = true;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_IsTollFreeVehicle_WithCar()
        {
            bool result = calculator.IsTollFreeVehicle(car);
            var expected = false;
            Assert.Equal(expected, result);
        }
        #endregion IsTollFreeVehicle-tester

        #region GetTollFee(vehicle, date[])-tester
        [Fact]
        public void Test_GetTollFee_WithSeveralDates()
        {
            DateTime[] dates = new DateTime[] { rushhourRegularDay, noonRegularDay };
            int actual = calculator.GetTollFee(car, dates);
            int expected = 26; // (18+8) FAIL: Actual 28!

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithSeveralDates_ButTollFree()
        {
            DateTime[] dates = new DateTime[] { christmas, weekend };
            int actual = calculator.GetTollFee(car, dates);
            int expected = 0;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithSeveralDates_OneTollFree()
        {
            DateTime[] dates = new DateTime[] { weekend, noonRegularDay };
            int actual = calculator.GetTollFee(car, dates);
            int expected = 8;
            Assert.Equal(expected, actual);
        }
        #endregion GetTollFee(vehicle, date[])-tester

        #region GetTollFee(date, vehicle)-tester
        [Fact]
        public void Test_GetTollFee_WithCar()
        {
            int actual = calculator.GetTollFee(rushhourRegularDay, car);
            int expected = 18;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithMotorbike()
        {
            int actual = calculator.GetTollFee(rushhourRegularDay, mc);
            int expected = 0;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetTollFee_WithTollFreeDate()
        {
            int actual = calculator.GetTollFee(weekend, car);
            int expected = 0;
            Assert.Equal(expected, actual);
        }
        #endregion GetTollFee(date, vehicle)-tester

        #region Random tests
        [Fact]
        public void GetVehicleTypeForCarWorks()
        {
            var actual = car.GetVehicleType();
            var expected = "Car";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetVehicleTypeForMcWorks()
        {
            var actual = mc.GetVehicleType();
            var expected = "Motorbike";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MotorbikeShouldAlwaysCost0SEK()
        {
            var actual = calculator.GetTollFee(rushhourRegularDay, mc);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarDuringNightShouldAlwaysCost0SEK()
        {
            var actual = calculator.GetTollFee(night, car);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarDuringChristmasShouldAlwaysCost0SEK()
        {
            var actual = calculator.GetTollFee(christmas, car);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarInJulyShouldAlwaysCost0SEK()
        {
            var actual = calculator.GetTollFee(july, car);
            var expected = 0; // FAIL: Actual 18!

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CarDuringWeekendshouldAlwaysCost0SEK()
        {
            var actual = calculator.GetTollFee(weekend, car);
            var expected = 0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDateThatWillNeverExist()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                calculator.GetTollFee(new DateTime(2024, 02, 30, 7, 30, 00), car);
            });
        }
        #endregion Random tests
    }
}