using System.Text.Json;

namespace Heracles.Lib
{
    public static class RecordHelpers
    {

        public static string RecordToJson(Record r) =>
            JsonSerializer.Serialize(r, typeof(Record), SourceGenerationContext.Default);

        public static string RecordListToJson(List<Record> r) =>
            JsonSerializer.Serialize(r, typeof(List<Record>), SourceGenerationContext.Default);

        public static Record? JsonToRecord(string j) =>
            JsonSerializer.Deserialize(j, typeof(Record), SourceGenerationContext.Default)
        as Record;
        public static List<Record>? JsonToRecordList(string s) =>
            JsonSerializer.Deserialize(s, typeof(List<Record>), SourceGenerationContext.Default)
        as List<Record>;
           
    }
}
