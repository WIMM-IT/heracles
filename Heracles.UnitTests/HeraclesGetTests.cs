namespace Heracles.UnitTests
{

    public class HeraclesGetTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesGetTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void GetWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.InvalidHydraClient.Get(Fixture.DummyRecord);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async void GetWithNonExistantRecordThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.ValidHydraClient.Get(Fixture.DummyRecord);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async void GetWithValidRecordNoIdThrowsArgumentNullException()
        {
            //Act
            Task result() => Fixture.ValidHydraClient.Get(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(result);
        }

        [Fact]
        public async void GetWithValidRecordReturnsCopy()
        {
            //Arrange
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("", 1);

            //Act
            Heracles.Lib.Record result = await Fixture.ValidHydraClient.Get(results.First());

            //Assert
            Assert.Equal(results.First().Id, result.Id);
        }

    }

}