namespace Heracles.UnitTests
{

    [Collection("Do Not Run In Parallel")]
    public class HeraclesCrudLifecycleTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesCrudLifecycleTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void CrudLifecycleTest()
        {
        
            // Want to test the vaild use cases in a single CRUD chain that runs separately
            // from the other tests. To do so, we use a DNS entry that will never exist,.

            Heracles.Lib.Record r = new()
            {
                Hostname = "_acme-challenge.heracles-unit-testing.imm.ox.ac.uk",
                Content = "WinACME Test",
                Type = "TXT"
            };

            // Create
            Heracles.Lib.Record? r1 = await Fixture.ValidHydraClient.Add(r);
            Assert.NotNull(r1);
            Thread.Sleep(100);

            // Read
            Heracles.Lib.Record? r2 = await Fixture.ValidHydraClient.Get(r1);
            Assert.NotNull(r2);
            Thread.Sleep(100);

            // Update
            r2.Content = "WinACME Modify Test";
            Heracles.Lib.Record? r3 = await Fixture.ValidHydraClient.Update(r2);
            Assert.NotNull(r3);
            Assert.NotEqual(r1.Content, r2.Content); // Before
            Assert.Equal(r2.Content, r3.Content);    // After
            Thread.Sleep(100);

            // Delete
            Heracles.Lib.Record? r4 = await Fixture.ValidHydraClient.Delete(r3);
            Assert.NotNull(r4);
        }

    }

}