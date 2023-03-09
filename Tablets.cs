using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realization
{
    class Tablets
    {
        Random rnd = new Random();
        public int NumberOfTablets =1;
        public double R;
        public int TabletArea;
        public int totalTablets;
        public int percentLuquid;
        public double percent;
        public int percentForTablets;
        public double numbOfCellsForTablets;
        public Tablets(int length, int width,double D)
        {
            int FieldArea = length * width;
            percentLuquid = 100 - rnd.Next(2);
            percentForTablets = 100 - percentLuquid;
            percent = (double)percentForTablets / 100.0;
            numbOfCellsForTablets = FieldArea * percent;
            totalTablets = (int)numbOfCellsForTablets;
            R = D / 2;
            TabletArea = (int)(((int)Math.Pow(R, 2)) * Math.PI);
            NumberOfTablets = totalTablets / TabletArea;
        }
    }
}
