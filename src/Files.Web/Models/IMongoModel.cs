using MongoDB.Bson;

namespace Files.Web.Models
{
	/// <summary>
	/// The base properties for a mongo model.
	/// </summary>
	interface IMongoModel
	{
		/// <summary>
		/// The unique object ID for this entity.
		/// </summary>
		ObjectId Id { get; set; }
	}
}
