namespace Heracles.UnitTests
{

    public class HeraclesConsoleTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesConsoleTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void LoopJsonRecordsWithRecordAndSafeModeAndInvalidClientReturnsHttpRequestExeption()
        {
            //Arrange
            string json = $"[{Lib.RecordHelpers.RecordToJson(Fixture.DummyRecord)}]";

            //Act
            Task result() => Console.Helpers.LoopJsonRecords(json, Fixture.InvalidHydraClient.Get, true);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async void LoopJsonRecordsWithRecordListAndSafeModeAndInvalidClientThrowsInvalidDataException()
        {
            //Arrange
            string json = Lib.RecordHelpers.RecordListToJson(Fixture.DummyRecordList);

            //Act
            Task result() => Console.Helpers.LoopJsonRecords(json, Fixture.InvalidHydraClient.Get, true);

            //Assert
            await Assert.ThrowsAsync<InvalidDataException>(result);
        }

    }

    [Collection("Do Not Run In Parallel")]
    public class HeraclesConsoleSerialTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesConsoleSerialTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void LoopJsonRecordsWithRecordListAndNoSafeModeAndInvalidClientReturnsHttpRequestExeption()
        {
            //Arrange
            string json = Lib.RecordHelpers.RecordListToJson(Fixture.DummyRecordList);
            System.Environment.SetEnvironmentVariable("HYDRA_UNSAFE", "true");

            //Act
            Task result() => Console.Helpers.LoopJsonRecords(json, Fixture.InvalidHydraClient.Get, true);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
            System.Environment.SetEnvironmentVariable("HYDRA_UNSAFE", null);

        }

    }
}