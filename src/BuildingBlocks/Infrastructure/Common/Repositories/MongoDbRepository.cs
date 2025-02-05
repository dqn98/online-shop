﻿using System.Linq.Expressions;
using Contracts.Domains;
using Contracts.Domains.Interfaces;
using Infrastructure.Extensions;
using MongoDB.Driver;
using Shared.Configurations;

namespace Infrastructure.Common.Repositories;

public class MongoDbRepository<T> : IMongoDbRepositoryBase<T> where T : MongoEntity
{
    private readonly IMongoDatabase _database;
    
    public MongoDbRepository(IMongoClient client, MongoDbSettings databaseSettings)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(databaseSettings);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseSettings.DatabaseName);

        _database = client.GetDatabase(databaseSettings.DatabaseName);
    }
    
    protected virtual IMongoCollection<T> Collection 
        => _database.GetCollection<T>(GetCollectionName());

    public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
        => _database.WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName());

    public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        => Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Expression<Func<T, string?>> func = f => f.Id;
        
        var value = entity.GetType()
            .GetProperty(func.Body.ToString()
                .Split(".")[1])?.GetValue(entity, null);
        
        if(value is null) throw new ArgumentNullException($"The entity {entity.GetType().Name} is not existed.");

        var filed = value.ToString();
        var filter = Builders<T>.Filter.Eq(func, filed);
        
        return Collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => Collection.DeleteOneAsync(x => x.Id != null && x.Id.Equals(id.ToString()), 
            cancellationToken: cancellationToken);
    
    protected static string GetCollectionName()
    {
        var collectionAttribute = (BsonCollectionAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(BsonCollectionAttribute))!;
        return collectionAttribute?.CollectionName ?? typeof(T).Name;
    }
}