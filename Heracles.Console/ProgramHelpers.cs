using Heracles.Lib;

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

            [json] must be either provided on the command line as a single '' protected
            string, or piped into STDIN, but not both.

            EXAMPLES:

            $ heracles search web
            
            $ heracles search web | heracles get
                        
            $ echo '[{ "comment": "Test", "content": "foobar", "hostname": "_acme-challenge.foo.imm.ox.ac.uk.", "type": "TXT" }]' | heracles add
            
            $ heracles search _acme-challenge.foo | sed 's/comment": "Test"/comment": "Another Test"/' | heracles update
            
            $ heracles search _acme-challenge.foo | heracles delete
            """;
        Console.WriteLine(usage);

        Environment.Exit(1);
	}

    /// <summary>
    /// Deserialises a JSON encoded list of Hydra Records and passes them one
    /// at a time to the provided function. If "safeMode" is true, the program
    /// will terminate when given more than one Record. To override the protection,
    /// set the environment variable "HYDRA_UNSAFE" to any non-null value.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="func"></param>
    /// <param name="safeMode"></param>
    /// <returns>The list of Hydra Records resulting from the function calls.</returns>
    public static async Task<List<Record>> LoopJsonRecords(string json, Func<Record, Task<Record>> func, bool safeMode = false)
    {
        List<Record>? inRecords = RecordHelpers.JsonToRecordList(json);
        string? safeModeOverride = Environment.GetEnvironmentVariable("HYDRA_UNSAFE");
        if ((inRecords?.Count > 1) && safeMode && safeModeOverride is null)
        {
            throw new Exception("Attempted to process multiple Records and HYDRA_UNSAFE is not set");
        }
        
        List<Record> outRecords = [];
        try
        {
            foreach (Record r in inRecords!)
            {
                outRecords.Add(await func(r));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}\n...aborting");
        }
        
        return outRecords;
    }
}