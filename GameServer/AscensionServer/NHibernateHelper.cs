﻿using NHibernate;
using NHibernate.Cfg;

namespace AscensionServer
{
    class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure("hibernate.cfg.xml");//解析hibernate.cfg.xml
                    configuration.AddAssembly("ClassLibrary");//解析映射文件User.hbm.xml   //ClassLibrary

                    _sessionFactory = configuration.BuildSessionFactory();
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
