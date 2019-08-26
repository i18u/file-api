using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Files.Web.Models
{
	/// <summary>
	/// The class representation of a file asset in MongoDB.
	/// </summary>
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

		/// <summary>
		/// The object id
		/// </summary>
		[BsonId]
		public ObjectId Id { get; set; }

		/// <summary>
		/// The file name of the uploaded file
		/// </summary>
		[BsonElement("fileName")]
		public string FileName { get; set; }

		/// <summary>
		/// The type of the file (stored for download)
		/// </summary>
		[BsonElement("contentType")]
		public string ContentType { get; set; }

		/// <summary>
		/// Whether or not the file has been verified.
		/// </summary>
		[BsonElement("verified")]
		public bool Verified { get; set; }
	}
}
