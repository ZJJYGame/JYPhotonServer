using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
   public interface INHManager
    {
        T Insert<T>(T data) where T : class, new();
        void Update<T>(T data) where T:new();
        void Delete<T>(T data) where T:new();
        T Get<T>(object key) where T : new();
        //T CriteriaGet<T>(string columnName,object key) ;
        T CriteriaSelect<T>(params NHCriteria[] columns);
    }
}
