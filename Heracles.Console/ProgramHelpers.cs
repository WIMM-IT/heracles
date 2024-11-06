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
    /// at a time to the provided function.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="f"></param>
    /// <returns>The list of Hydra Records resulting from the function calls.</returns>
    public static async Task<List<Record>> LoopJsonRecords(string s, Func<Record, Task<Record>> f)
    {
        List<Record> rs = [];
        try
        {
            foreach (Record r in RecordHelpers.JsonToRecordList(s)!)
            {
                rs.Add(await f(r));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}\n...aborting");
        }
        return rs;
    }
}