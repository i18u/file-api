using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Files.Web.Models
{
	class FileAsset : IMongoModel
	{
		/// <summary>
		/// The database this entity resides in.
		/// </summary>
		public const string Database = "cookbook";

		/// <summary>
		/// The collection this entity resides in.
		/// </summary>
		public const string Collection = "files";

		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("fileName")]
		public string FileName { get; set; }

		[BsonElement("contentType")]
		public string ContentType { get; set; }

		[BsonElement("verified")]
		public bool Verified { get; set; }
	}
}