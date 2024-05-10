using Csutils;
using Heracles.Lib;

List<Record>? records = Hydra.Read(3);
if (records == null )
{
	Console.WriteLine("CRIT: Hydra did not return data");
	Environment.Exit(1);
}

foreach (var obj in records)
{
	Console.WriteLine(obj.ToPrettyString());
}

