using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace wedslk.lib.infrastructure.service.database.cosmos
{
    public class DatabaseService<T> : IDatabaseService<T>
    {

        #region Members 

        private Container _container;
        private readonly string _partitionKey = String.Empty;
        private PartitionKey _partitionKeyValue;

        #endregion

        #region Constructor
        public DatabaseService(CosmosClient dbClient, string databaseName, string containerName, string partitionKey)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
            this._partitionKey = partitionKey;
        }
        #endregion

        #region Get

        public async Task<T> GetByIDAsync(string id)
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

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> whereExpression)
        {
            List<T> results = new List<T>();
            using (FeedIterator<T> setIterator = this._container.GetItemLinqQueryable<T>()
                      .Where(whereExpression)
                      .ToFeedIterator<T>())
            {
                //Asynchronous query execution
                while (setIterator.HasMoreResults)
                {
                    var response = await setIterator.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
            }

            return results;
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

        #endregion

        #region Add

        public async Task<T> AddAsync(T item)
        {
            var result = await this._container.CreateItemAsync<T>(item, this._partitionKeyValue);

            if (result.StatusCode == System.Net.HttpStatusCode.Created) 
            {
                return result.Resource;
            }

            return default(T);
        }

        #endregion

        #region Delete 

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await this._container.DeleteItemAsync<T>(id, this._partitionKeyValue);

            if (result.StatusCode == System.Net.HttpStatusCode.OK) 
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Update

        public async Task<T> UpdateAsync(string id, T item)
        {
            var result = await this._container.UpsertItemAsync<T>(item, this._partitionKeyValue);

            if (result.StatusCode == System.Net.HttpStatusCode.OK) 
            {
                return result.Resource;
            }

            return default(T);
        }

        #endregion

        #region  Utility

        public void SetPartitionKeyValue(object value)
        {
            string resultValue = value.ToString();
            double doubleTypeValue = double.MinValue;
            bool boolTypeValue = false;

            if (double.TryParse(resultValue, out doubleTypeValue))
            {
                this._partitionKeyValue = new PartitionKey(doubleTypeValue);
            }
            else if (bool.TryParse(resultValue, out boolTypeValue))
            {
                this._partitionKeyValue = new PartitionKey(boolTypeValue);
            }
            else 
            {
                this._partitionKeyValue = new PartitionKey(resultValue);
            }
        }

        #endregion
    }
}
