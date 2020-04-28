using NHibernate;
using NHibernate.Cfg;

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
                    var configuration = new Configuration();
                    configuration.Configure("hibernate.cfg.xml");//解析hibernate.cfg.xml
                    configuration.AddAssembly("AscensionServer");//解析映射文件User.hbm.xml   //ClassLibrary  //AscensionServer

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
