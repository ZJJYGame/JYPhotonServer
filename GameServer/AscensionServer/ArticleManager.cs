using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Manager;
using NHibernate;

namespace AscensionServer.Model
{
    public class ArticleManager : IArticleManager
    {
        //获取文章的内容
        public ArticleModel GetArticleContent(ArticleModel _id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction  transaction = session.BeginTransaction())
                {
                    ArticleModel articleModel = session.Get<ArticleModel>(_id.Id);
                    transaction.Commit();
                    return articleModel;
                }
            }
        }
    }
}
