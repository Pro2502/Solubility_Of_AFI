using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Realization
{
    class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr nWnd, int nCmdShow);

        private const int MAXIMIZE = 3;

        const double Dt = 0.1;
        const double k = 0.0001;
        static void Main(string[] args)
        {
            Console.ReadLine();
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(GetConsoleWindow(), MAXIMIZE);
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);


            CellularAutomata cellularAutomata = new CellularAutomata();
            cellularAutomata.Initialisation();
            cellularAutomata.Field_output();

            bool no_end = true;
            bool can_update = true;
            while (no_end)
            {
                Console.Title = cellularAutomata.CurrentGeneration.ToString();

                while (can_update)
                {

                    //Console.WriteLine("Press ENTER to go through the next iteration");
                    //string str = Console.ReadLine();
                    //if (str == "")
                    //{
                        if (no_end)
                    {
                            cellularAutomata.Transition_Rule_dissolution(k);
                            Console.WriteLine("After dissolution");
                            cellularAutomata.Field_output();
                            cellularAutomata.Transition_Rule_diffusion(Dt);
                            cellularAutomata.Transformation();
                            Console.WriteLine("After diffusion");
                            cellularAutomata.Field_output();
                            cellularAutomata.quantityCurve.Add((int)cellularAutomata.quantity);

                            cellularAutomata.Iteration_Count(ref no_end);
                           
                            //cellularAutomata.ReadAutomataTxt();

                    }
                    else
                    {
                        cellularAutomata.WriteAutomataToTxt();
                        break;
                    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("!!!You have exited the dissolution visualization program!!!");
                    //    can_update = false;
                    //}
                }
            }
        }
    }
}
