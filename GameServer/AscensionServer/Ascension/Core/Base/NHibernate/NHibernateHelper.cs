/*
*Author : Don
*Since 	:2020-04-18
*Description  : NHibernate帮助类
*/
using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using Cosmos;

namespace AscensionServer
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        static MySqlData sqlData;
        static NHibernateHelper()
        {
            var result = GameEntry.DataManager.TryGetValue(out sqlData);
            if (!result)
                Utility.Debug.LogError("Get MySqlData fail，check your config file !");
        }
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Fluently.Configure().
                        Database(MySQLConfiguration.Standard.
                        ConnectionString(db => db.Server(sqlData.Address).
                        Database(sqlData.Database).Username(sqlData.Username).
                        Password(sqlData.Password))).
                        Mappings(x => { x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>(); }).
                        BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();//打开一个跟数据库的会话
        }
    }

}


