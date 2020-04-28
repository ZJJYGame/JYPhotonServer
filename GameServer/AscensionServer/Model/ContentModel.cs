using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    public class ContentModel
    {
        public virtual int Id { get; set; }
        public virtual string Content { get; set; }
        public virtual ArticleModel Article { get; set; }
    }
}
