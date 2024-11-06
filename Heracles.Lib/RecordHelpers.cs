using System.Text.Json;

namespace Heracles.Lib
{
    public static class RecordHelpers
    {

        /// <summary>
        /// Serialise a record to its JSON representation.
        /// </summary>
        /// <param name="r"></param>
        /// <returns>A JSON string.</returns>
        public static string RecordToJson(Record r) =>
            JsonSerializer.Serialize(r, typeof(Record), SourceGenerationContext.Default);

        /// <summary>
        /// Serialise a list of records to their JSON representation.
        /// </summary>
        /// <param name="r"></param>
        /// <returns>A JSON string.</returns>
        public static string RecordListToJson(List<Record> r) =>
            JsonSerializer.Serialize(r, typeof(List<Record>), SourceGenerationContext.Default);

        /// <summary>
        /// Deerialise a record from its JSON representation.
        /// </summary>
        /// <param name="j"></param>
        /// <returns>A Hydra Record</returns>
        public static Record? JsonToRecord(string j) =>
            JsonSerializer.Deserialize(j, typeof(Record), SourceGenerationContext.Default)
        as Record;

        /// <summary>
        /// Deerialise a list of records from their JSON representation.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>A list of Hydar Records</returns>
        public static List<Record>? JsonToRecordList(string s) =>
            JsonSerializer.Deserialize(s, typeof(List<Record>), SourceGenerationContext.Default)
        as List<Record>;
           
    }
}
