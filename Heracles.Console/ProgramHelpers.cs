using Heracles.Lib;
using System.Text.Json;

partial class Program()
{
	public static void Usage()
	{
		System.Console.WriteLine("USAGE: heracles [search|get|add|update|delete] [record]");
		Environment.Exit(1);
	}

	public static Record? JsonToRecord(string s)
	{
        Record? r = null;
        try
        {
            r = JsonSerializer.Deserialize<List<Record>>(s)?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("CRIT: Cannot serialize input to a valid DNS record");
            System.Console.WriteLine(ex.Message.ToString());
            Usage();
        }
        return r;
    }
}