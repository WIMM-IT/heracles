using Heracles.Lib;

const string devUri = "https://devon.netdev.it.ox.ac.uk/api/ipam/";
const string hostname = "_acme-challenge.imm-dmtmac.imm.ox.ac.uk.";
Record newRecord = new Record
{
    Hostname = hostname,
    Type = "TXT",
    Content = $"Test TXT {DateTime.Now}",
    Comment = "WinAcme"
};

HydraClient client = new(devUri);

var matches = await client.Search(hostname);
Thread.Sleep(2000);
var match = matches.First();
match.Content = $"Test TXT {DateTime.Now}";

match = await client.Put(match);
Thread.Sleep(2000);

match = await client.Get(match);
Thread.Sleep(2000);

_ = await client.Delete(match);
Thread.Sleep(2000);

_ = await client.Search(hostname);
Thread.Sleep(2000);

match = await client.Post(newRecord);
Thread.Sleep(2000);

_ = await client.Get(match);