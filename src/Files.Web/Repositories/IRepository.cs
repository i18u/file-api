using System;
using System.Collections.Generic;
using Files.Web.Models;
using MongoDB.Bson;

namespace Files.Web.Repositories
{
	/// <inheritdoc />
	internal interface IRepository<TModel> : IRepository<TModel, TModel>, IReadOnlyRepository<TModel> where TModel : IMongoModel {}

	/// <summary>
	/// An interface building on <see cref="IReadOnlyRepository{TModel}"/> with the
	/// ability to allow writing and updating of <typeparamref name="TModel"/> objects.
	/// </summary>
	/// <typeparam name="TModel">Type of object stored in this repository.</typeparam>
	internal interface IRepository<TModel, TProjection> : IReadOnlyRepository<TModel, TProjection> where TModel : IMongoModel
	{
		/// <summary>
		/// Upsert a <typeparamref name="TModel"/> object into this repository, either inserting a new document or
		/// updating an existing document if one already exists, and return the object's <see cref="Guid"/>.
		/// </summary>
		/// <param name="model"><typeparamref name="TModel"/> instance to upsert.</param>
		/// <returns>Provided <typeparamref name="TModel"/> object.</returns>
		ObjectId Upsert(TModel model);

		/// <summary>
		/// Upsert many <typeparamref name="TModel"/> objects into this repository, either inserting new documents or
		/// updating existing documents if they don't already exist, and return a set of each <see cref="Guid"/>.
		/// </summary>
		/// <param name="models">Set of <typeparamref name="TModel"/> instances to upsert.</param>
		/// <returns>Set of upserted <see cref="Guid"/> values for specified documents.</returns>
		IEnumerable<ObjectId> UpsertMany(IEnumerable<TModel> models);
	}
}
