using TollFeeCalculator;

namespace UnitTests
{
    public class UnitTest1
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