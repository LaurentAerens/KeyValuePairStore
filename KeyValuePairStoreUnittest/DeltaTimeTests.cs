namespace AerensStoreTest
{
    [TestFixture]
    public class DeltaTimeTests
    {
        [Test]
        public void DefaultConstructor_WhenCalled_SetsAllPropertiesToZero()
        {
            var deltaTime = new DeltaTime();

            Assert.That(deltaTime.Years, Is.EqualTo(0));
            Assert.That(deltaTime.Months, Is.EqualTo(0));
            Assert.That(deltaTime.Days, Is.EqualTo(0));
            Assert.That(deltaTime.Hours, Is.EqualTo(0));
        }

        [Test]
        public void IsOff_WhenAllPropertiesAreZero_ReturnsTrue()
        {
            var deltaTime = new DeltaTime();

            Assert.That(deltaTime.IsOff, Is.True);
        }

        [Test]
        public void IsOff_WhenAnyPropertyIsNonZero_ReturnsFalse()
        {
            var deltaTime = new DeltaTime(0, 0, 0, 1);

            Assert.That(deltaTime.IsOff, Is.False);
        }

        [Test]
        public void Constructor_WhenCalledWithValuesHours_SetsPropertiesCorrectly()
        {
            var deltaTime = new DeltaTime(1, 2, 3, 4);

            Assert.That(deltaTime.Years, Is.EqualTo(0));
            Assert.That(deltaTime.Months, Is.EqualTo(0));
            Assert.That(deltaTime.Days, Is.EqualTo(0));
            Assert.That(deltaTime.Hours, Is.EqualTo(4));
        }

        [Test]
        public void Constructor_WhenYearsIsNegative_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DeltaTime(-1, 0, 0, 0));
        }

        [Test]
        public void Constructor_WhenMonthsIsNegative_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DeltaTime(0, -1, 0, 0));
        }

        [Test]
        public void Constructor_WhenDaysIsNegative_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DeltaTime(0, 0, -1, 0));
        }

        [Test]
        public void Constructor_WhenHoursIsNegative_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DeltaTime(0, 0, 0, -1));
        }

        [Test]
        public void Constructor_WhenMonthsIsGreaterThan11_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DeltaTime(0, 12, 0, 0));
        }

        [Test]
        public void Constructor_WhenDaysIsGreaterThan30_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DeltaTime(0, 0, 31, 0));
        }

        [Test]
        public void Constructor_WhenHoursIsGreaterThan23_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new DeltaTime(0, 0, 0, 24));
        }

        [Test]
        public void GetTimeValuesHours_WhenCalled_ReturnsArrayWithCorrectValues()
        {
            var deltaTime = new DeltaTime(1, 2, 3, 4);

            var result = deltaTime.GetTimeValues();

            Assert.That(result, Is.EquivalentTo(new[] { 4, 0, 0, 0 }));
        }

        [Test]
        public void Constructor_WhenCalledWithValuesDays_SetsPropertiesCorrectly()
        {
            var deltaTime = new DeltaTime(1, 2, 3, 0);

            Assert.That(deltaTime.Years, Is.EqualTo(0));
            Assert.That(deltaTime.Months, Is.EqualTo(0));
            Assert.That(deltaTime.Days, Is.EqualTo(3));
            Assert.That(deltaTime.Hours, Is.EqualTo(0));
        }

        [Test]
        public void GetTimeValuesDays_WhenCalled_ReturnsArrayWithCorrectValues()
        {
            var deltaTime = new DeltaTime(1, 2, 3, 0);

            var result = deltaTime.GetTimeValues();

            Assert.That(result, Is.EquivalentTo(new[] { 0, 3, 0, 0 }));
        }

        [Test]
        public void Constructor_WhenCalledWithValuesMonths_SetsPropertiesCorrectly()
        {
            var deltaTime = new DeltaTime(1, 2, 0, 0);

            Assert.That(deltaTime.Years, Is.EqualTo(0));
            Assert.That(deltaTime.Months, Is.EqualTo(2));
            Assert.That(deltaTime.Days, Is.EqualTo(0));
            Assert.That(deltaTime.Hours, Is.EqualTo(0));
        }

        [Test]
        public void GetTimeValuesMonths_WhenCalled_ReturnsArrayWithCorrectValues()
        {
            var deltaTime = new DeltaTime(1, 2, 0, 0);

            var result = deltaTime.GetTimeValues();

            Assert.That(result, Is.EquivalentTo(new[] { 0, 0, 2, 0 }));
        }

        [Test]
        public void Constructor_WhenCalledWithValuesYears_SetsPropertiesCorrectly()
        {
            var deltaTime = new DeltaTime(1, 0, 0, 0);

            Assert.That(deltaTime.Years, Is.EqualTo(1));
            Assert.That(deltaTime.Months, Is.EqualTo(0));
            Assert.That(deltaTime.Days, Is.EqualTo(0));
            Assert.That(deltaTime.Hours, Is.EqualTo(0));
        }

        [Test]
        public void GetTimeValuesYears_WhenCalled_ReturnsArrayWithCorrectValues()
        {
            var deltaTime = new DeltaTime(1, 0, 0, 0);

            var result = deltaTime.GetTimeValues();

            Assert.That(result, Is.EquivalentTo(new[] { 0, 0, 0, 1 }));
        }

    }
}
