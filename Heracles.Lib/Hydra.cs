using System.Diagnostics;
using System.Text.Json;
using Csutils;

namespace Heracles.Lib
{
	public static class Hydra
	{
		public static List<Record>? Read(int limit = 500000)
		{
			Process p = new();
			(string output, string error) = p.Run("hydra.sh", limit.ToString());
			return JsonSerializer.Deserialize<List<Record>>(output);
		}
	}
}
