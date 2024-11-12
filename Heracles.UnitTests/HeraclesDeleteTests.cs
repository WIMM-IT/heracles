namespace Heracles.UnitTests
{

    public class HeraclesDeleteTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesDeleteTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void DeleteWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.InvalidHydraClient.Delete(Fixture.DummyRecord);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async void DeleteWithNonExistantRecordThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.ValidHydraClient.Delete(Fixture.DummyRecord);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async void DeleteWithValidRecordNoIdThrowsArgumentNullException()
        {
            //Act
            Task result() => Fixture.ValidHydraClient.Delete(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(result);
        }

    }

}