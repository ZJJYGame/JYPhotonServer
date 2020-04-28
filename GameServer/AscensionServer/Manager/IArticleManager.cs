using AscensionServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Manager
{
    public  interface IArticleManager
    {
        ArticleModel GetArticleContent(ArticleModel _id);
    }
}
