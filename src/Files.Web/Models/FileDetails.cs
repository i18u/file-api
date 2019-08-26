using System.Runtime.Serialization;
using MongoDB.Bson;

namespace Files.Web.Models
{
	/// <summary>
	/// Details of a file for access.
	/// </summary>
	[DataContract]
	public class FileDetails
	{
		private readonly ObjectId _fileId;

		/// <summary>
		/// The location to retrieve this file from.
		/// </summary>
		[DataMember(Name = "location")]
		public string FileLocation => $"/api/files/{_fileId}";

		/// <summary>
		/// Creates a new instance of the <see cref="FileDetails"/> class.
		/// </summary>
		/// <param name="fileId">The object id of the file.</param>
		public FileDetails(ObjectId fileId)
		{
			_fileId = fileId;
		}
	}
}
