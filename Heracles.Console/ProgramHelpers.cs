using Heracles.Lib;
using System.Text.Json;

partial class Program()
{
	public static void Usage()
	{
		System.Console.WriteLine("USAGE: heracles [search|get|add|update|delete] [record]");
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