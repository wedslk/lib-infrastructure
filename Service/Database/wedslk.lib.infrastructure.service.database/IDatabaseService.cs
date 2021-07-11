using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace wedslk.lib.infrastructure.service.database
{
    public interface IDatabaseService<T>
    {
        public Task<IEnumerable<T>> GetAsync(string query);

        public Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> whereExpression);
        public Task<T> GetByIDAsync(string id);
        public Task<T> AddAsync(T item);
        public Task<T> UpdateAsync(string id, T item);
        public Task<bool> DeleteAsync(string id);

        public void SetPartitionKeyValue(object value);
    }
}
