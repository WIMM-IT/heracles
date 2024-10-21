using Heracles.Lib;

const string devUri = "https://devon.netdev.it.ox.ac.uk/api/ipam/";
HydraClient client = new(devUri);

const string testRecord = "_acme-challenge.imm-dmtmac.imm.ox.ac.uk.";
await client.Get(3, $"in_hostname%3A{testRecord}");
Thread.Sleep(2000);
await client.Delete(testRecord);
Thread.Sleep(2000);
await client.Get(3, $"in_hostname%3A{testRecord}");
Console.WriteLine(); // No newline
Thread.Sleep(2000);
string testTxt = $"Test TXT {DateTime.Now}";
await client.Post(testRecord, testTxt);
Thread.Sleep(2000);
await client.Get(3, $"in_hostname%3A{testRecord}");