using System.Text.Json.Serialization;

namespace Heracles.Lib
{
	public partial class Record
	{

		// See https://networks.it.ox.ac.uk/university/ipam/help/record-fields
		// for full details of record fields.

		[JsonPropertyName("comment")]
		public required string Comment {  get; set; }

		[JsonPropertyName("content")]
		public required string Content { get; set; }

		[JsonPropertyName("created_in_transaction")]
		public required Int64 CreatedInTransaction { get; set; }

		[JsonPropertyName("deleted_in_transaction")]
		public Int64? DeletedInTransaction { get; set; }

		[JsonPropertyName("hostname")]
		public required string Hostname { get; set; }

		[JsonPropertyName("href")]
		public Uri? Href { get; set; }

		[JsonPropertyName("id")]
		public required Guid Id { get; set; }

		[JsonPropertyName("is_in_users_view")]
		public required bool IsInUsersView { get; set; }

		[JsonPropertyName("is_locked")]
		public bool? IsLocked { get; set; }

		[JsonPropertyName("lock_id")]
		public Guid? LockId { get; set; }

		[JsonPropertyName("prio_et_al")]
		public required string PrioEtAl { get; set; }

		[JsonPropertyName("table_name")]
		public required string TableName { get; set; }

		[JsonPropertyName("target")]
		public string? Target { get; set; }

		[JsonPropertyName("type")]
		public required string Type { get; set; }

	}
}
