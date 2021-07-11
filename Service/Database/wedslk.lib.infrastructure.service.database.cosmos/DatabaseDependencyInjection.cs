using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace wedslk.lib.infrastructure.service.database.cosmos
{
    public static class DatabaseClientDependencyInjection
    {
        public static CosmosClient InitializeDatabaseClientInstanceAsync(IConfigurationSection configurationSection)
        {
            
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;

            return new CosmosClient(account, key);
        }

        public static async Task<IDatabaseService<T>> InitializeDatabaseEntityInstanceAsync<T>(CosmosClient client, 
                                                                                                IConfigurationSection configurationSection,
                                                                                                string databaseName,
                                                                                                string containerName,
                                                                                                string partitionKey)
        {
            IDatabaseService<T> cosmosDbService = new DatabaseService<T>(client, databaseName, containerName, partitionKey);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKey);

            return cosmosDbService;
        }
    }
}
