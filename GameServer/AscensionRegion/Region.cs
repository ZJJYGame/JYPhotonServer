using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionRegion
{
    public class Region : IDisposable
    {
        Vector2 stratRegion;
        Vector2 endRegion;
        public Region()
        {

        }
        public void Dispose()
        {
        }
        public Vector2 RegionSize()
        {
            return stratRegion - endRegion;
        }
    }
}
