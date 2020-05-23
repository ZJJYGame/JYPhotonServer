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

namespace AscensionServer
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public  static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Fluently.Configure().
                        Database(MySQLConfiguration.Standard.
                        ConnectionString(db => db.Server("192.168.0.117").
                        Database("jygame").Username("jieyou").
                        Password("jieyougamePWD"))).
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
