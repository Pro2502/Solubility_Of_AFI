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
        public string path = @"D:\Дипломная работа\Realization\change_in_concentration.txt";
        public Cells[,] Field;
        private readonly int _rows =70 /*Console.LargestWindowHeight*/;
        private readonly int _cols = 70/*Console.LargestWindowWidth*/;
        private Random _random = new Random();
        double D = 5;
        int MaxSolidsConcent = 50;
        public Calculation[,] Field_for_calculation;

        public double quantity = 0;
        public double difference;
        public List<int> quantityCurve = new List<int>();

        static void StartCreate(Cells[,] Field, Calculation[,] Field_for_calculation)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    Field[x, y] = new Cells();
                    Field[x, y].concentration = 0;
                    Field_for_calculation[x, y] = new Calculation();
                    Field_for_calculation[x, y].accumulation_concentration = 0;
                }
            }
        }

        public void Initialisation()
        {
            Field = new Cells[_rows, _cols];
            Field_for_calculation = new Calculation[_rows, _cols];

            CellularAutomata.StartCreate(Field, Field_for_calculation);
            Tablets tablet = new Tablets(Field.GetLength(0), Field.GetLength(1), D);
            bool intersection_with_other_tablets;

            Console.WriteLine("Процент жидкости:" + tablet.percentLuquid);

            Console.WriteLine("Процент для таблеток:" + tablet.percentForTablets);

            Console.WriteLine("Процент для таблеток в долях:" + tablet.percent);

            Console.WriteLine("Количество клеток под таблетки:" + tablet.totalTablets);

            Console.WriteLine("Площадь под таблетку:" + tablet.TabletArea);

            Console.WriteLine("Таблеток:" + tablet.NumberOfTablets);

            //while (tablet.NumberOfTablets > 0)
            //{
                intersection_with_other_tablets = false;
                int X = 35 /*_random.Next(0, Field.GetLength(0))*/;
                int Y = 35/*_random.Next(0, Field.GetLength(1))*/;

            while (!intersection_with_other_tablets)
            {
                for (int x = -(int)tablet.R; x <= +(int)tablet.R; x++)
                    {
                        for (int y = -(int)tablet.R; y <= +(int)tablet.R; y++)
                        {
                            var I = (X + x + Field.GetLength(0)) % Field.GetLength(0);
                            var J = (Y + y + Field.GetLength(1)) % Field.GetLength(1);

                            if (Field[I, J].concentration >= Field[I, J].saturated_solution)
                            {
                            intersection_with_other_tablets = true;
                            continue;
                            }
                        }
                    }
                if (!intersection_with_other_tablets)
                {
                    for (int x = -(int)tablet.R; x <= +(int)tablet.R; x++)
                        {
                            for (int y = -(int)tablet.R; y <= +(int)tablet.R; y++)
                            {
                                var I = (X + x + Field.GetLength(0)) % Field.GetLength(0);
                                var J = (Y + y + Field.GetLength(1)) % Field.GetLength(1);
                                Field[I, J].concentration = MaxSolidsConcent - _random.Next(Field[I, J].saturated_solution + 1);
                            }
                    }
                    tablet.NumberOfTablets--;
                    Console.WriteLine(tablet.NumberOfTablets);
                }
            }
        //}
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

                        for (int i =  - 1; i <=  + 1; i++)
                        {
                            for (int j =  - 1; j <=  + 1; j++)
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
                    difference = 0;
                    double recalculation = 0;
                    if (Field[x, y].concentration < Field[x, y].saturated_solution)
                    {
                        double dC;
                        for (int i = x- 1; i <= x+ 1; i++)
                        {
                            for (int j =  y - 1; j <= y + 1; j++)
                            {
                                var I = ( i + Field.GetLength(0)) % Field.GetLength(0);
                                var J = ( j + Field.GetLength(1)) % Field.GetLength(1);

                                var isSelfChecking = I == x && J == y;
                                if (isSelfChecking)
                                    continue;

                                //if ((I != (x - 1) & J == y) | (I == x & J != (y + 1)) | (I != (x + 1) & J == y) | (I == x & J != (y - 1)))
                                //{
                                    if (Field[I, J].concentration <= 0)
                                        continue;
                                    if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                    {
                                        if (Field[I, J].concentration <Field[x, y].concentration)
                                        {
                                            dC = constD * (Field[x, y].concentration - Field[I, J].concentration);

                                            double limitation = Field[x, y].concentration - dC;

                                        if (limitation >= 0)
                                        {
                                            difference += dC;
                                        }
                                    }
                                    }
                                 //}
                            }
                        }
                    }
                    if (Field_for_calculation[x, y].accumulation_concentration - difference < 0)
                    {
                        recalculation = Field_for_calculation[x, y].accumulation_concentration / difference;

                        if (Field[x, y].concentration < Field[x, y].saturated_solution)
                        {
                            
                            for (int i = x - 1; i <= x + 1; i++)
                            {
                                for (int j = y - 1; j <= y + 1; j++)
                                {
                                    var I = (i + Field.GetLength(0)) % Field.GetLength(0);
                                    var J = (j + Field.GetLength(1)) % Field.GetLength(1);

                                    var isSelfChecking = I == x && J == y;
                                    if (isSelfChecking)
                                        continue;
                                   
                                    if (Field[I, J].concentration <= 0)
                                        continue;
                                    if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                    {
                                        if (Field[I, J].concentration < Field[x, y].concentration)
                                        {
                                            double dC = constD * (Field[x, y].concentration - Field[I, J].concentration);
                                            double remainder = dC * recalculation;

                                            Field_for_calculation[x, y].accumulation_concentration = Field_for_calculation[x, y].accumulation_concentration- remainder;
                                            Field_for_calculation[I, J].accumulation_concentration = Field_for_calculation[I, J].accumulation_concentration+ remainder;
                                            quantity += remainder;
                                        }
                                    }
                                   
                                }
                            }
                        }   
                    }
                    else
                    {
                        double dC;
                       
                                if (Field[x, y].concentration < Field[x, y].saturated_solution)
                                {

                                    for (int i = x - 1; i <= x + 1; i++)
                                    {
                                        for (int j = y - 1; j <= y + 1; j++)
                                        {
                                            var I = (i + Field.GetLength(0)) % Field.GetLength(0);
                                            var J = (j + Field.GetLength(1)) % Field.GetLength(1);

                                            var isSelfChecking = I == x && J == y;
                                            if (isSelfChecking)
                                                continue;

                                            if (Field[I, J].concentration <= 0)
                                                continue;
                                            if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                            {

                                                if (Field[I, J].concentration < Field[x, y].concentration)
                                                {
                                                    dC = constD * (Field[x, y].concentration - Field[I, J].concentration);

                                                    double limitation = Field[x, y].concentration - dC;

                                                    if (limitation >= 0)
                                                    {
                                                        Field_for_calculation[x, y].accumulation_concentration = Field_for_calculation[x, y].accumulation_concentration - dC;
                                                        Field_for_calculation[I, J].accumulation_concentration = Field_for_calculation[I, J].accumulation_concentration + dC;
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

                    else if (Field[x, y].concentration < Field[x, y].saturated_solution && Field[x, y].concentration > 0/*(int)(Field[x, y].saturated_solution * 0.1)*/)
                    {
                        int MinC = (int)(Field[x, y].saturated_solution * 0.4);
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
            Console.WriteLine(quantity);
        }
        public void Transformation()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {

                    Field[i, j].concentration = Field[i, j].concentration + Field_for_calculation[i, j].accumulation_concentration;
                    Field_for_calculation[i, j].accumulation_concentration = 0;
                    if (Field[i, j].concentration < 0)
                    {
                        Field[i, j].concentration = 0;
                    }
                }
            }

        }
        public void Iteration_Count(ref bool no_end)
        {
            bool equality = false;
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    int MinC = (int)(Field[x, y].saturated_solution * 0.4);
                    if (Field[x, y].concentration < Field[x, y].saturated_solution && Field[x, y].concentration < MinC && Field[x, y].concentration != 0)
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
                //for (int x = 0; x < Field.GetLength(0); x++)
                //{
                //    for (int y = 0; y < Field.GetLength(1); y++)
                //    {
                //        sw.WriteLine(String.Format("{0}|{1}|{2}|", x, y, Field[x, y].Concentration));
                //    }
                //}
                foreach (var i in quantityCurve)
                {
                    sw.WriteLine(String.Format("{0}", i));
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
                    //Field[x, y] = new Cells();
                    //Field[x, y].Concentration = Convert.ToInt32(fields[2]);
                }
            }
        }
    }
}
