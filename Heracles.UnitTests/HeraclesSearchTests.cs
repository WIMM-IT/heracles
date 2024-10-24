namespace Heracles.UnitTests
{

    public class HeraclesSearchTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesSearchTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void SearchWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.InvalidHydraClient.Search("_acme-challenge");

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async void SearchWithValidSubstringReturnsValidResult()
        {
            //Act
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("_acme-challenge");
            
            //Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
        }

        [Fact]
        public async void SearchWithInvalidSubstringReturnsEmptyResult()
        {
            //Act
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("willnevermatch");

            //Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void SearchWithValidLimitReturnsValidResult(int x)
        {
            //Act
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("_acme-challenge", x);

            //Assert
            Assert.NotNull(results);
            Assert.Equal(x, results.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(500001)]
        public async void SearchWithInvalidLimitThrowsArgumentOutOfRangeExceptionException(int x)
        {
            //Act
            Task result() => Fixture.InvalidHydraClient.Search("_acme-challenge", x);

            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(result);
        }

    }

}