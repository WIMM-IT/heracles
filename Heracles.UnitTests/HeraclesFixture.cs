using Heracles.Lib;

namespace Heracles.UnitTests
{
    public class HeraclesFixture
    {

        private readonly string? uri;
        private readonly string? credentials;
        public HydraClient ValidHydraClient { get; }
        public HydraClient InvalidHydraClient { get; }
        public Heracles.Lib.Record DummyRecord = new()
        {
            Hostname = "notarealhostname",
            Content = "test",
            Type = "TXT",
            Id = Guid.NewGuid()
        };
        public Heracles.Lib.Record DummyRecordNoId = new()
        {
            Hostname = "notarealhostname",
            Content = "test",
            Type = "TXT"
        };
        public List<Heracles.Lib.Record> DummyRecordList = 
        [
            new () {Hostname = "foo", Content = "foo", Type = "TXT", Id = Guid.NewGuid()},
            new () {Hostname = "bar", Content = "bar", Type = "TXT", Id = Guid.NewGuid()},
        ];

        public HeraclesFixture()
        {
            credentials = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
            ArgumentNullException.ThrowIfNull(credentials);
            uri = Environment.GetEnvironmentVariable("HYDRA_URI");
            ArgumentNullException.ThrowIfNull(uri);
            ValidHydraClient = new(uri, credentials);
            InvalidHydraClient = new(uri, "notvalidcredentials");
        }

    }

}
