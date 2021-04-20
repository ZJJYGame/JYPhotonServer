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
            public bool IsCollected { get; set; }
            public FixTransform FixTransform { get; set; }
            public void Dispose()
            {
                FixTransform = FixTransform.Zero;
                Id = -1;
                IsCollected = false;
            }
        }
        public bool Collect(int eleId)
        {
            if (CollectDict != null)
                return false;
            if (CollectDict.TryGetValue(eleId, out var cr))
            {
                if (cr.IsCollected)
                    return false;
                else
                    cr.IsCollected = true;
            }
            return false;
        }
        public void RenewalAll()
        {
            foreach (var col in CollectDict.Values)
            {
                col.IsCollected = false;
            }
        }
        public void Renewal(int eleId)
        {
            if (CollectDict.TryGetValue(eleId, out var cr))
            {
                cr.IsCollected = false;
            }
        }
        public void Dispose()
        {
            Id = -1;
            CollectDict.Clear();
        }
    }
}
