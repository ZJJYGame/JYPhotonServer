using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class MultiplayLevelRes
    {
        public LevelTypeEnum LevelType { get; private  set; }
        Dictionary<int, FixCollectable> collectableDict;
        public MultiplayLevelRes()
        {
            collectableDict = new Dictionary<int, FixCollectable>();
        }
        public void Collect(int gId,int  eleId)
        {
            if( collectableDict.TryGetValue(gId,out var col))
            {

            }
        }

    }
}
