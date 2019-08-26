using System.Runtime.Serialization;
using MongoDB.Bson;

namespace Files.Web.Models
{
	[DataContract]
	public class FileDetails
	{
		private readonly ObjectId _fileId;

		[DataMember(Name = "location")]
		public string FileLocation => $"/api/files/{_fileId}";

		public FileDetails(ObjectId fileId)
		{
			_fileId = fileId;
		}
	}
}
