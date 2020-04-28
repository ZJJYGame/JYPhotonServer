using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    public class ArticleModel
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual ContentModel Content { get; set; }
    }
}
