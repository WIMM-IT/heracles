using Heracles.Console;
using Heracles.Lib;
using System.Text.Json;
using System.Text.Json.Serialization;

// Globals
string? Uri;
string? Credentials;
string? Input;
ProgramMode Mode;
HydraClient Client;
JsonSerializerOptions options = new()
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

// Configure args
Mode = (args.Length > 0) switch
{
    false => ProgramMode.Unknown,
    true => args[0] switch
    {
        "search" => ProgramMode.Search,
        "get" => ProgramMode.Get,
        "add" => ProgramMode.Add,
        "update" => ProgramMode.Update,
        "delete" => ProgramMode.Delete,
        _ => ProgramMode.Unknown
    }
};

Input = (System.Console.IsInputRedirected, args.Length == 2) switch
{
    (true, false) => System.Console.In.ReadToEnd(),
    (false, true) => args[1],
    _ => null
};

if (Input is null || Mode == ProgramMode.Unknown)
{
    Program.Usage();
}

// Init
Uri = Environment.GetEnvironmentVariable("HYDRA_URI");
Credentials = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
ArgumentNullException.ThrowIfNull(Uri);
ArgumentNullException.ThrowIfNull(Credentials);
Client = new(Uri, Credentials);

// Run
List<Record> r = Mode switch
{
    ProgramMode.Search => await Client.Search(Input!),
    ProgramMode.Get    => [await Client.Get(JsonToRecord(Input!)!)],
    ProgramMode.Add    => [await Client.Add(JsonToRecord(Input!)!)],
    ProgramMode.Update => [await Client.Update(JsonToRecord(Input!)!)],
    ProgramMode.Delete => [await Client.Delete(JsonToRecord(Input!)!)],
    _ => new List<Record> { } // Should never get here
};

Console.WriteLine(JsonSerializer.Serialize<List<Record>>(r, options));