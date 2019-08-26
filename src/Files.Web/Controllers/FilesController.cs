using System;
using System.IO;
using System.Runtime.InteropServices;
using Files.Web.Models;
using Files.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Files.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FilesController : ControllerBase
	{
		private string DefaultFolderPath 
		{
			get 
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					return "C:\\ProgramData\\cookbook\\files";
				}

				return "/srv/cookbook/files";
			}
		}

		private string GetStoragePath()
		{
			return Environment.GetEnvironmentVariable("FILE_LOCATION") ?? DefaultFolderPath;
		}

		private string GetFilePath(ObjectId fileId)
		{
			return Path.Combine(GetStoragePath(), fileId.ToString());
		}

		private object GetMessage(string message)
		{
			return new { message = message };
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			if (!ObjectId.TryParse(id, out var fileId)) 
			{
				return BadRequest(GetMessage("Provided id was of invalid format."));
			}

			var fileRepository = new MongoRepository<FileAsset>(FileAsset.Database, FileAsset.Collection);
			var fileAsset = fileRepository.Get(fileId);

			if (fileAsset == null) 
			{
				return NotFound(GetMessage("The file was not found."));
			}

			var filePath = GetFilePath(fileId);

			var fileBytes = System.IO.File.ReadAllBytes(filePath);

			return File(fileBytes, fileAsset.ContentType);
		}

		[HttpPost]
		public IActionResult Post(IFormFile file)
		{
			if (file == null) 
			{
				return BadRequest(GetMessage("File not sent."));
			}

			if (file.Length == 0) 
			{
				return BadRequest(GetMessage("File zero-length."));
			}

			var projection = Builders<FileAsset>.Projection.Expression(z => new FileDetails(z.Id));
			var fileRepository = new MongoRepository<FileAsset, FileDetails>(FileAsset.Database, FileAsset.Collection, projection);

			var fileAsset = new FileAsset 
			{
				FileName = file.FileName,
				ContentType = file.ContentType,
				Verified = true, // This will need to be improved upon later. Prevent dick picks and other mcgubbins from being uploaded.
			};

			var insertedId = fileRepository.Insert(fileAsset);

			var storagePath = GetStoragePath();

			if (!Directory.Exists(storagePath)) 
			{
				Directory.CreateDirectory(storagePath);
			}

			var filePath = GetFilePath(insertedId);

			using (var stream = System.IO.File.Open(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			return Ok(new FileDetails(insertedId));
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(string id)
		{
			if (!ObjectId.TryParse(id, out var fileId)) 
			{
				return BadRequest(GetMessage("Provided id was of invalid format."));
			}

			var fileRepository = new MongoRepository<FileAsset>(FileAsset.Database, FileAsset.Collection);
			fileRepository.Delete(fileId);

			var filePath = GetFilePath(fileId);
			System.IO.File.Delete(filePath);

			return Ok(GetMessage("File deleted succesfully."));
		}
	}
}
