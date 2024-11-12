using Heracles.Console;
using Heracles.Lib;

// Globals
string? Uri;
string? Token;
string? Input;
ProgramMode Mode;
HydraClient Client;

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
    Helpers.Usage();
}

try
{
    // Init
    Uri = Environment.GetEnvironmentVariable("HYDRA_URI");
    Token = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
    ArgumentNullException.ThrowIfNull(Uri, "HYDRA_URI");
    ArgumentNullException.ThrowIfNull(Token, "HYDRA_TOKEN");
    Client = new(Uri, Token);

    // Run
    List<Record> r = Mode switch
    {
        ProgramMode.Search => await Client.Search(Input!),
        ProgramMode.Get => await Helpers.LoopJsonRecords(Input!, Client.Get),
        ProgramMode.Add => await Helpers.LoopJsonRecords(Input!, Client.Add),
        ProgramMode.Update => await Helpers.LoopJsonRecords(Input!, Client.Update, true),
        ProgramMode.Delete => await Helpers.LoopJsonRecords(Input!, Client.Delete, true),
        _ => new List<Record> { } // Should never get here
    };

    Console.WriteLine(RecordHelpers.RecordListToJson(r));

}
catch (Exception ex)
{
    Console.WriteLine($"CRIT: {ex.Message}");
}