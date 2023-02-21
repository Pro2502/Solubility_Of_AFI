using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Realization
{
    class CellularAutomata
    {   
        public int CurrentGeneration { get; private set;}
        public string path = @"D:\Дипломная работа\Solubility_Of_API\change_in_concentration.txt";
        public Cells[,] Field;
        private readonly int _rows =50 /*Console.LargestWindowHeight*/;
        private readonly int _cols = 50/*Console.LargestWindowWidth*/;
        private Random _random = new Random();
        double D = 8;
        int MaxSolidsConcent = 500;

        
        public double quantity = 0;

        static void StartCreate(Cells[,] Field)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    Field[x, y] = new Cells();
                    Field[x, y].concentration = 0;
                }
            }
        }

        public void Initialisation()
        {
            Field = new Cells[_rows, _cols];
            CellularAutomata.StartCreate(Field);
            Tablets tablet = new Tablets(Field.GetLength(0), Field.GetLength(1), D);
            //bool intersection_with_other_tablets = false;

            while (tablet.NumberOfTablets > 0)
            {
                int X = _random.Next(0, Field.GetLength(0));
                int Y = _random.Next(0, Field.GetLength(1));

                //while (!intersection_with_other_tablets)
                //{
                    for (int x = X - (int)tablet.R; x <= X + (int)tablet.R; x++)
                    {
                        for (int y = Y - (int)tablet.R; y <= Y + (int)tablet.R; y++)
                        {

                            if (Field[x, y].concentration >= Field[x, y].saturated_solution)
                            {
                                //intersection_with_other_tablets = true;
                            }
                            else
                            {
                                for (x = X - (int)tablet.R; x <= X + (int)tablet.R; x++)
                                {
                                    for (y = Y - (int)tablet.R; y <= Y + (int)tablet.R; y++)
                                    {
                                        var I = (X + x + Field.GetLength(0)) % Field.GetLength(0);
                                        var J = (Y + y + Field.GetLength(1)) % Field.GetLength(1);
                                        Field[I, J].concentration = MaxSolidsConcent - _random.Next(Field[I, J].saturated_solution+1);
                                        tablet.NumberOfTablets--;
                                    }
                                }

                            }
                        }
                    }
                //}

            }
        }

        public void Transition_Rule_dissolution(double k)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (Field[x, y].concentration >= Field[x, y].saturated_solution)
                    {
                        double dC;

                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                var I = (x + i + Field.GetLength(0)) % Field.GetLength(0);
                                var J = (y + j + Field.GetLength(1)) % Field.GetLength(1);
                                var isSelfChecking = I == x && J == y; 
                                if (isSelfChecking)
                                    continue;

                                if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                {
                                    if ((I != (x - 1) & J == y) | (I == x & J != (y + 1)) | (I != (x + 1) & J == y) | (I == x & J != (y - 1)))
                                    {
                                        if (Field[x, y].saturated_solution > Field[x, y].concentration)
                                        {
                                            dC = -k * (Field[x, y].saturated_solution - Field[x, y].concentration);
                                        }
                                        else
                                        {
                                            dC = k;
                                        }
                                        double limitation = Field[x, y].concentration - dC;
                                        if (limitation >= 0)
                                        {
                                            Field[I, J].concentration += dC ;
                                            Field[x, y].concentration -= dC ;
                                            quantity += dC;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Transition_Rule_diffusion(double constD)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (Field[x, y].concentration < Field[x, y].saturated_solution)
                    {
                        double dC;
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            for (int j = y - 1; j < y + 1; j++)
                            {
                                var I = (x + i + Field.GetLength(0)) % Field.GetLength(0);
                                var J = (y + j + Field.GetLength(1)) % Field.GetLength(1);
                                var isSelfChecking = I == x && J == y;
                                if (isSelfChecking)
                                    continue;

                                if ((I != (x - 1) & J == y) | (I == x & J != (y + 1)) | (I != (x + 1) & J == y) | (I == x & J != (y - 1)))
                                {
                                    if (Field[I, J].concentration <= 0)
                                        continue;
                                    if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                    {
                                        if (Field[I, J].concentration > Field[x, y].concentration)
                                        {
                                            dC = constD * (Field[I, J].concentration - Field[x, y].concentration);

                                            double limitation = Field[I, J].concentration - dC;
                                            if (limitation >= 0)
                                            {
                                                Field[I, J].concentration -= dC;
                                                Field[x, y].concentration += dC;
                                                quantity += dC;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            CurrentGeneration++;
        }
        public void Field_output()
        {

            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (Field[x, y].concentration >= Field[x, y].saturated_solution)
                    {
                        int MaxC = (int)(MaxSolidsConcent * 0.9);
                        if (Field[x, y].concentration > MaxC)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("A" + " ");
                            //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("D" + " ");
                            //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                            Console.ResetColor();
                        }
                    }

                    else if (Field[x, y].concentration < Field[x, y].saturated_solution && Field[x, y].concentration > (int)(Field[x, y].saturated_solution * 0.1))
                    {
                        int MinC = (int)(Field[x, y].saturated_solution * 0.3);
                        if (Field[x, y].concentration < MinC)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("S" + " ");
                            //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("L" + " ");
                            //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("W" + " ");
                        //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");
        }
        public void Iteration_Count(ref bool no_end)
        {
            bool equality = false;
            double approximate_concentration = Math.Round(Field[0, 0].concentration, 3);
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (approximate_concentration == Math.Round(Field[x, y].concentration, 3))
                    {
                        equality = true;
                    }
                    else
                    {
                        equality = false;
                        break;
                    }
                }
            }
            if (equality)
            {
                Console.WriteLine("The total time for the dissolution of Arogel:" + CurrentGeneration);

                no_end = false;
            }
        }
        public void WriteAutomataToTxt()
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int x = 0; x < Field.GetLength(0); x++)
                {
                    for (int y = 0; y < Field.GetLength(1); y++)
                    {
                        sw.WriteLine(String.Format("{0}|{1}|{2}|", x, y, Field[x, y].Concentration));
                    }
                }
            }
        }
        public void ReadAutomataTxt()
        {
            if (File.Exists(path))
            {
                //Создаем массив
                StreamReader sr = File.OpenText(path);
                while (!sr.EndOfStream)
                {
                    string data_string = sr.ReadLine();
                    string[] fields = data_string.Split('|');
                    int x = Convert.ToInt32(fields[0]);
                    int y = Convert.ToInt32(fields[1]);
                    Field[x, y] = new Cells();
                    Field[x, y].Concentration = Convert.ToInt32(fields[2]);
                }
            }
        }
    }
}
