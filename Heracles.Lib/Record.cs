using System.Text.Json.Serialization;

namespace Heracles.Lib
{
    public partial class Record
	{

        // See https://networks.it.ox.ac.uk/university/ipam/help/record-fields
        // for full details of record fields.

        [JsonPropertyName("big_endian_labels")]
        public List<string>? BigEndianLables { get; set; }

        [JsonPropertyName("bind_rdata")]
        public string? BindRdata { get; set; }

        [JsonPropertyName("comment")]
		public string? Comment { get; set; }

		[JsonPropertyName("content")]
		public string? Content { get; set; }

		[JsonPropertyName("created_in_transaction")]
		public Int64? CreatedInTransaction { get; set; }

		[JsonPropertyName("deleted_in_transaction")]
		public Int64? DeletedInTransaction { get; set; }

		[JsonPropertyName("hostname")]
		public required string Hostname { get; set; }

		[JsonPropertyName("href")]
		public Uri? Href { get; set; }

		[JsonPropertyName("id")]
		public Guid? Id { get; set; }

        [JsonPropertyName("in_domains")]
        public List<string>? InDomains { get; set; }

        [JsonPropertyName("ip")]
        public string? Ip { get; set; }

        [JsonPropertyName("is_in_users_view")]
		public bool? IsInUsersView { get; set; }

		[JsonPropertyName("is_locked")]
		public bool? IsLocked { get; set; }

		[JsonPropertyName("lock_id")]
		public Guid? LockId { get; set; }

		[JsonPropertyName("prio_et_al")]
		public string? PrioEtAl { get; set; }

		[JsonPropertyName("table_name")]
		public string? TableName { get; set; }

        [JsonPropertyName("target")]
        public string? Target { get; set; }

        [JsonPropertyName("ttl")]
        public uint? Ttl { get; set; }

        [JsonPropertyName("type")]
		public required string Type { get; set; }

        [JsonPropertyName("zone_name")]
        public string? ZoneName { get; set; }

    }
}
