using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realization
{
    class Cells
    {
        public int saturated_solution = 250;
        public float x { get; set; }
        public float y { get; set; }

        public double concentration;
        public bool IsSolid { get; private set; }
        public bool Luquid { get; private set; }

        private void Switch_Solid()
        {
            if (concentration < saturated_solution)
            {
                IsSolid = false;
            }
        }
        public int Concentration
        {
            set
            {
                concentration = value;
            }
            get
            {
                int new_C = Convert.ToInt32(concentration);
                return new_C;
            }
        }
        public bool CalcSolid()
        {
            return concentration >= saturated_solution;

        }
        public bool CalcLuquid()
        {
            return concentration < saturated_solution;

        }
    }
}
