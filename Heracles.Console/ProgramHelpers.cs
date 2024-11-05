using Heracles.Lib;
using System.Text.Json;

partial class Program()
{
	public static void Usage()
	{
        string usage = """
            USAGE: heracles search string
                            get    [json]
                            add    [json]
                            update [json]
                            delete [json]

            All commands output a JSON encoded list of the found or affected records.

            [json] must be either provided on the command line as a single "" protected
            string, or piped into STDIN, but not both.
            """;
        Console.WriteLine(usage);

        Environment.Exit(1);
	}

	public static List<Record>? JsonToRecords(string s)
	{
        List<Record>? r = null;
        try
        {
            r = JsonSerializer.Deserialize<List<Record>>(s);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("CRIT: Cannot serialize input to a valid DNS record");
            System.Console.WriteLine(ex.Message.ToString());
            Usage();
        }
        return r;
    }

    public static async Task<List<Record>> LoopJsonRecords(string s, Func<Record, Task<Record>> f)
    {
        List<Record> rs = [];
        foreach (Record r in JsonToRecords(s)!)
        {
            rs.Add(await f(r));
        }
        return rs;
    }
}