using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace ProtocolCore
{
    public interface IRedisHelperHash : IService<IRedisHelperHash>
    {
       Task< UnaryResult<bool>> HashExistAsync(string key, string dataKey);
        Task<UnaryResult<bool>> HashSetAsync(string key, string dataKey, string jsonVar);
        Task<UnaryResult<bool>> HashDeleteAsync(string key, string dataKey);
        Task<UnaryResult<long>> HashsDeleteAsync(string key, params string[] dataKeys);
        Task<UnaryResult<string>> HashGetAsync(string key, string dataKey);
        Task<UnaryResult<double>> HashIncrementAsync(string key, string dataKey, double value);
        Task<UnaryResult<double>> HashDecrementAsync(string key, string dataKey, double value);
        Task<UnaryResult<string[]>> HashKeysAsync(string key);
        Task<UnaryResult<Dictionary<string, string>>> HashGetAllAsync(string key);
    }
}
