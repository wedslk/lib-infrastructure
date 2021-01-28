using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wedslk.lib.infrastructure.service.database.cosmos
{
    public class DatabaseService<T> : IDatabaseService<T>
    {
        private Container _container;
        private readonly string _partitionKey = String.Empty;
        public DatabaseService(CosmosClient dbClient, string databaseName, string containerName) 
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(T item)
        {
            await this._container.CreateItemAsync<T>(item, new PartitionKey(_partitionKey));
        }

        public async Task DeleteAsync(string id)
        {
            await this._container.DeleteItemAsync<T>(id, new PartitionKey(_partitionKey));
        }

        public async Task<IEnumerable<T>> GetAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await this._container.ReadItemAsync<T>(id, new PartitionKey(_partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default(T);
            }
        }

        public async Task UpdateAsync(string id, T item)
        {
            await this._container.UpsertItemAsync<T>(item, new PartitionKey(_partitionKey));
        }
    }
}
