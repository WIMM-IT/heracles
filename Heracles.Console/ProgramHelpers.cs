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

    public static async Task<List<Record>> LoopJsonRecords(string s, Func<Record, Task<Record>> f)
    {
        List<Record> rs = [];
        foreach (Record r in RecordHelpers.JsonToRecordList(s)!)
        {
            rs.Add(await f(r));
        }
        return rs;
    }
}