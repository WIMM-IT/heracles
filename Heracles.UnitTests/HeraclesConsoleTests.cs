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
        public async void LoopJsonRecordsWithRecordAndSafeModeAndInvalidClientReturnsEmptyList()
        {
            //Arrange
            string json = $"[{Lib.RecordHelpers.RecordToJson(Fixture.DummyRecord)}]";

            //Act
            List<Lib.Record> results = await Console.Helpers.LoopJsonRecords(json, Fixture.InvalidHydraClient.Get, true);

            //Assert
            Assert.Empty(results);
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

}