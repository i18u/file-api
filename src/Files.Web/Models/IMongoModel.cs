using MongoDB.Bson;

namespace Files.Web.Models
{
	interface IMongoModel
	{
		ObjectId Id { get; set; }
	}
}
