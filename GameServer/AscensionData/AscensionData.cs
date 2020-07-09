using System;

namespace AscensionData
{
    public class AscensionData : IDisposable
    {
        public int Index { get; set; }

        public void Clear()
        {
            Index = -1;
        }

        public void Dispose()
        {
            Index = -1;
        }
    }
}
