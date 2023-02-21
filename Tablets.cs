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
        public int NumberOfTablets = 1;
        public double R;
        //int TabletArea;
        public Tablets(int length, int width,double D)
        {
            //int FieldArea = length * width ;
            //int percentLuquid = 90-rnd.Next(30);
            //int percentForTablets = 100 - percentLuquid;
            //double percent = percentForTablets / 100;
            //double numbOfCellsForTablets = FieldArea * percent;
            //int totalTablets = (int)numbOfCellsForTablets;
            R = D / 2;
            //TabletArea = (int)(((int)Math.Pow(R,2)) * Math.PI);
            //NumberOfTablets = totalTablets / TabletArea;
        }
    }
}
