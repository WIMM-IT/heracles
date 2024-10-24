namespace Heracles.UnitTests
{

    public class HeraclesUpdateTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesUpdateTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void UpdateWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.InvalidHydraClient.Update(Fixture.DummyRecord);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async void UpdateWithValidRecordNoIdThrowsArgumentNullException()
        {
            //Act
            Task result() => Fixture.ValidHydraClient.Update(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(result);
        }

    }

}