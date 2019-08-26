using System;
using System.Collections.Generic;
using Files.Web.Models;
using MongoDB.Bson;

namespace Files.Web.Repositories
{
	/// <inheritdoc />
	internal interface IReadOnlyRepository<TModel> : IReadOnlyRepository<TModel, TModel> where TModel : IMongoModel {}

	/// <summary>
	/// Interface to define a repository storing <typeparamref name="TModel"/> objects.
	/// </summary>
	/// <typeparam name="TModel">Type of object stored in this repository.</typeparam>
	internal interface IReadOnlyRepository<TModel, TProjection> where TModel : IMongoModel
	{
		/// <summary>
		/// Retrieve a single <typeparamref name="TModel"/> instance from
		/// the database using a unique <see cref="Guid"/>.
		/// </summary>
		/// <returns>Single <typeparamref name="TModel"/> instance.</returns>
		TProjection Get(ObjectId id);
		
		/// <summary>
		/// Retrieve a set of <typeparamref name="TModel"/> instances from the database
		/// using pagination parameters <paramref name="page"/> and <paramref name="limit"/>.
		/// </summary>
		/// <param name="page">Page number.</param>
		/// <param name="limit">Number of results to return.</param>
		/// <returns>Set of <typeparamref name="TModel"/> objects for the specified page.</returns>
		IEnumerable<TProjection> Get(int page, int limit);
	}
}
