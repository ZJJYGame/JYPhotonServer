using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionRegion
{
    public class Region : IDisposable
    {
        Tile stratRegion;
        Tile endRegion;
        public Region()
        {

        }
        public void Dispose()
        {
        }
        public Tile RegionSize()
        {
            return stratRegion - endRegion;
        }
    }
}
