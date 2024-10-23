using Heracles.Lib;
using System.Text.Json;
using System.Text.Json.Serialization;

const int pause = 500;
const string devUri = "https://devon.netdev.it.ox.ac.uk/api/ipam/";
const string hostname = "_acme-challenge.imm-dmtmac.imm.ox.ac.uk.";
Record newRecord = new()
{
    Hostname = hostname,
    Type = "TXT",
    Content = $"Test TXT {DateTime.Now}",
    Comment = "WinAcme"
};
JsonSerializerOptions options = new()
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

HydraClient client = new(devUri);
List<Record>? entries;
Record? entry;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"SEARCH {hostname}");
Console.ResetColor();
entries = await client.Search(hostname);
entry = entries.First();
Console.WriteLine(JsonSerializer.Serialize(entries, options));
Thread.Sleep(pause);

entry.Content = $"Test TXT {DateTime.Now}";
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"UPDATE {entry.Id}");
Console.ResetColor();
entry = await client.Update(entry);
Console.WriteLine(JsonSerializer.Serialize(entry, options));
Thread.Sleep(pause);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"GET {entry.Id}");
Console.ResetColor();
entry = await client.Get(entry);
Console.WriteLine(JsonSerializer.Serialize(entry, options));
Thread.Sleep(pause);

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine($"DELETE {entry.Id}");
Console.ResetColor();
entry = await client.Delete(entry);
Console.WriteLine(JsonSerializer.Serialize(entry, options));
Thread.Sleep(pause);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"SEARCH {hostname}");
Console.ResetColor();
entries = await client.Search(hostname);
Console.WriteLine(JsonSerializer.Serialize(entries, options));
Thread.Sleep(pause);

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"ADD {newRecord}");
Console.ResetColor();
entry = await client.Add(newRecord);
Console.WriteLine(JsonSerializer.Serialize(entry, options));
Thread.Sleep(pause);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"GET {entry.Id}");
Console.ResetColor();
entry = await client.Get(entry);
Console.WriteLine(JsonSerializer.Serialize(entry, options));