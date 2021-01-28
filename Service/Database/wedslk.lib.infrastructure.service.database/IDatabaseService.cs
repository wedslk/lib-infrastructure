using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace wedslk.lib.infrastructure.service.database
{
    /*https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnet-application*/
    public interface IDatabaseService<T>
    {
        public Task<IEnumerable<T>> GetAsync(string query);
        public Task<T> GetItemAsync(string id);
        public Task AddAsync(T item);
        public Task UpdateAsync(string id, T item);
        public Task DeleteAsync(string id);
    }
}
