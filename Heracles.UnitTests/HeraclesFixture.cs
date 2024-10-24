using Heracles.Lib;

namespace Heracles.UnitTests
{
    public class HeraclesFixture
    {

        private readonly string? credentials;
        public HydraClient ValidHydraClient { get; }
        public HydraClient InvalidHydraClient { get; }

        private const string uri = "https://devon.netdev.it.ox.ac.uk/api/ipam/";

        public HeraclesFixture()
        {
            credentials = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
            ArgumentNullException.ThrowIfNull(credentials);
            ValidHydraClient = new(uri, credentials);
            InvalidHydraClient = new(uri, "notvalidcredentials");
        }

    }

}
