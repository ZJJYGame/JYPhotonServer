using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class FixCollectable : IDisposable
    {
        public int Id { get; set; }
        public Dictionary<int, CollectableRes> CollectDict { get; set; }
        public class CollectableRes : IDisposable
        {
            public int Id { get; set; }
            public bool CanCollected { get; set; }
            public FixTransform FixTransform { get; set; }
            public void Dispose()
            {
                FixTransform = FixTransform.Zero;
                Id = -1;
                CanCollected = true;
            }
        }
        public void RenewalAll()
        {
            foreach (var col in CollectDict.Values)
            {
                col.CanCollected = true;
            }
        }
        public void Renewal(int eleId)
        {
            if (CollectDict.TryGetValue(eleId, out var cr))
            {
                cr.CanCollected = true;
            }
        }
        public void Dispose()
        {
            Id = -1;
            CollectDict.Clear();
        }
    }
}
