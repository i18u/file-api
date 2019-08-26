using System;
using System.Collections.Generic;
using System.Linq;
using Files.Web.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Files.Web.Repositories
{
	/// <inheritdoc />
	internal class MongoRepository<TModel> : MongoRepository<TModel, TModel> where TModel : IMongoModel
	{
		/// <inheritdoc />
		public MongoRepository(string database, string collection, ProjectionDefinition<TModel, TModel> projection = null) : base(database, collection, projection)
		{
		}
	}

	/// <summary>
	/// Repository pattern for MongoDB for retrieval of <typeparamref name="TModel"/> objects
	/// </summary>
	/// <typeparam name="TModel">Type of object in this repository</typeparam>
	internal class MongoRepository<TModel, TProjection> : IRepository<TModel, TProjection> where TModel : IMongoModel
	{
		/// <summary>
		/// The number of max items that can be queried at a time from MongoDB.
		/// </summary>
		public const int MaxItems = 250;

		public const string AdminDatabase = "admin";

		private Lazy<IMongoClient> _client = new Lazy<IMongoClient>(GetClient);

		/// <summary>
		/// The client to use for mongodb connections.
		/// </summary>
		protected IMongoClient Client => _client.Value;

		/// <summary>
		/// The backing <see cref="IMongoCollection{TModel}"/> for this <see cref="MongoRepository{TModel}"/>
		/// </summary>
		protected IMongoCollection<TModel> Collection { get; }

		/// <summary>
		/// The projection to apply to all queries.
		/// </summary>
		protected ProjectionDefinition<TModel, TProjection> Projection { get; }

		/// <summary>
		/// Create a new <see cref="MongoRepository{TModel}"/> backed
		/// with the specified <see cref="IMongoCollection{TModel}"/>.
		/// </summary>
		/// <param name="collection">Backing <see cref="IMongoCollection{TModel}"/> to use.</param>
		public MongoRepository(string database, string collection, ProjectionDefinition<TModel, TProjection> projection = null)
		{
			Collection = GetCollection(database, collection);
			Projection = projection;
		}

		/// <inheritdoc />
		public TProjection Get(ObjectId id)
		{
			var filter = Builders<TModel>.Filter.Eq(document => document.Id, id);

			return Collection.Find(filter).Project(Projection).FirstOrDefault();
		}

		/// <inheritdoc />
		public IEnumerable<TProjection> Get(int page, int limit)
		{
			if (page <= 0 || limit <= 0 || limit > MaxItems)
			{
				return new List<TProjection>();
			}

			var totalSkipped = page * limit;

			return Collection
				.Find(FilterDefinition<TModel>.Empty)
				.Project(Projection)
				.Skip(totalSkipped)
				.Limit(limit)
				.ToEnumerable();
		}

		public ObjectId Insert(TModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			Collection.InsertOne(model);

			return model.Id;
		}

		/// <inheritdoc />
		public ObjectId Upsert(TModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			var filter = Builders<TModel>.Filter.Eq(document => document.Id, model.Id);

			var upsertResult = Collection.ReplaceOne(filter, model, new UpdateOptions
			{
				IsUpsert = true
			});

			if (upsertResult.UpsertedId == null) 
			{
				return ObjectId.Empty;
			}

			return upsertResult.UpsertedId.AsObjectId;
		}

		public void Delete(ObjectId id)
		{
			var filter = Builders<TModel>.Filter.Eq(document => document.Id, id);

			Collection.DeleteOne(filter);
		}

		/// <inheritdoc />
		public IEnumerable<ObjectId> UpsertMany(IEnumerable<TModel> models)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a collection by the given database and collection names.
		/// </summary>
		/// <param name="database">The name of the database to retrieve the collection from.</param>
		/// <param name="collection">The name of the collection to retrieve.</param>
		/// <returns>An <see cref="IMongoCollection{TModel}"/> object representing the MongoDB collection.</returns>
		protected IMongoCollection<TModel> GetCollection(string database, string collection)
		{
			return Client.GetDatabase(database).GetCollection<TModel>(collection);
		}

		private static MongoClientSettings GetClientSettings()
		{
			var mongoAddress = Environment.GetEnvironmentVariable("MONGO_SERVER") ?? "127.0.0.1:27017";

			var mongoUsername = Environment.GetEnvironmentVariable("MONGO_USER");
			var mongoPassword = Environment.GetEnvironmentVariable("MONGO_PASS");

			var settings = new MongoClientSettings
			{
				Servers = mongoAddress.Split(",", StringSplitOptions.RemoveEmptyEntries)
					.Select(address => address.Split(":", StringSplitOptions.RemoveEmptyEntries))
					.Select(addressParts => new MongoServerAddress(addressParts[0], addressParts.Length == 2 ? int.Parse(addressParts[1]) : 27017))
					.ToArray(),
			};

			if (!string.IsNullOrWhiteSpace(mongoUsername))
			{
				settings.Credential = MongoCredential.CreateCredential(AdminDatabase, mongoUsername, mongoPassword);
			}

			return settings;
		}

		private static IMongoClient GetClient()
		{
			var settings = GetClientSettings();

			return new MongoClient(settings);
		}
	}
}
