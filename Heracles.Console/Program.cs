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

_ = await client.Get($"in_hostname%3A{hostname}", 3);
Thread.Sleep(2000);
_ = await client.Delete(hostname);
Thread.Sleep(2000);
_ = await client.Get($"in_hostname%3A{hostname}", 3);
Thread.Sleep(2000);
_ = await client.Post(newRecord);
Thread.Sleep(2000);
_ = await client.Get($"in_hostname%3A{hostname}", 3);