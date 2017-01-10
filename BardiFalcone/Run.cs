using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using BardiFalcone.TerminalSet;
using BardiFalcone.FailureSet;
using System.Diagnostics;
using BardiFalcone.ControlSet;
using BardiFalcone.Parameters;

namespace BardiFalcone
{
    class Run
    {
        private const string FILENAME_SIMPLE = "inputSimple.xml";
        private const string FILENAME_SIMPLE_ROUND = "inputSimpleRound.xml";
        private const string FILENAME_MATERIAL_POINT = "inputMaterialPoint.xml";
        private const string FILENAME_CHAUFFEUR = "inputChauffeur.xml";
        private const string FILENAME_CHAUFFEUR_FALCONE = "inputChauffeurFalcone.xml";
        private const string FILENAME_3D_MATERIAL_POINT = "input3DMaterialPoint.xml";
        private const string FILENAME_DUBINS_CAR = "inputDubinsCar.xml";
        private const string FILENAME_2D_DUBINS_CAR = "input2DDubinsCar.xml";
        private const string FILENAME_PENDULUM = "inputPendulum.xml";
        private const string FILENAME_TAD_PROBLEM = "inputTadProblem.xml";

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //Upgrade();
            //StartCountingSimple();
            //StartCountingSimpleRound();
            //StartMaterialPointGame();
            //StartMaterialPoint();
            //StartPendulum();
            //StartCountingChauffeur();
            //StartCountingChauffeurFalcone();
            //StartCounting3DimMaterialPoint();           
            //StartCountingDubinsCar();
            //StartCounting2DDubinsCar();
            StartTadProblem();
        }

        /// <summary>
        /// Метод "улучшает" решетку
        /// </summary>
        private static void Upgrade()
        {
            double[] sizeOfDomain = new double[] { 3, 3 };
            double[] sizeOfBox = new double[] { 0.05, 0.05 };
            Point initialPoint = new Point(new double[] { -1.5, -1.5 });
            //ITerminalSet terminalSet = new CircleTerminalSet(0.5);
            ITerminalSet terminalSet = new CircleTerminalSet(0.2, new Point(new double[] { 0, 0 }));
            Grid grid = new Grid(sizeOfDomain, sizeOfBox, initialPoint, terminalSet);

            //IControlSet controlSet1 = new RingControlSet(new Point(new double[] { 0, 0 }), 2, 0.25);
            //IControlSet controlSet2 = new RingControlSet(new Point(new double[] { 0, 0 }), 1, 0.25);
            //IControlSet controlSet1 = new CircleControlSet(new Point(new double[] { -2, -2 }), new Point(new double[] { 2, 2 }), 0.5);
            //IControlSet controlSet2 = new CircleControlSet(new Point(new double[] { -1, -1 }), new Point(new double[] { 1, 1 }), 0.5);   
            //IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -2, -2 }), new Point(new double[] { 2, 2 }), 0.25);
            //IControlSet controlSet2 = new RectangularControlSet(new Point(new double[] { -1, -1 }), new Point(new double[] { 1, 1 }), 0.25);

            /*
            //Для проверки, что множества управлений нормально строятся
            System.IO.File.WriteAllText(@"output.txt", string.Empty);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"output.txt", true))
            {
                foreach (var o in controlSet1.Set)
                    file.WriteLine("{0} {1}", o.coords[0], o.coords[1]);
            }
             * */

            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1, 0 }), new Point(new double[] { 1, 0 }), 0.05);
            IControlSet controlSet2 = new RectangularControlSet(new Point(new double[] { -0.2, 0 }), new Point(new double[] { 0.2, 0 }), 0.05);

            Function.function function = delegate(Point p, Point u, Point v)
            {
                //return u + v;
                double[] coord = new double[2];
                coord[0] = p[1] + v[0];
                coord[1] = u[0];
                return new Point(coord, true);
            };

            UpgradeGrid upGrid = new UpgradeGrid(grid, function, controlSet1, controlSet2);
            
            grid.FillBasis();
            
            for (int i = 0; i < 100; i++)
            {
                bool finish = true;
                grid.FillInitialFacesSet();                
                foreach (Face face in grid.faces)
                {
                    if (upGrid.NeedToAddCube(face))
                    {
                        finish = false;
                        upGrid.AddCube(face);
                    }
                }

                if (File.Exists(@"outputPoints" + i + ".txt"))
                    System.IO.File.WriteAllText(@"outputPoints" + i + ".txt", string.Empty);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"outputPoints" + i + ".txt", true))
                {
                    foreach (Point point in grid.points[0].Keys)
                        file.WriteLine("{0} {1}", point.coords[0], point.coords[1]);
                }

                if (finish == true)
                    continue;
            }

            if (upGrid.result.Count > 0)
                Console.WriteLine("lol");
            else Console.WriteLine("notlol");            
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "простые движения"
        /// </summary>
        private static void StartCountingSimple()
        {            
            ITerminalSet terminalSet = new SquareTerminalSet(0.5);            
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -2, -2 }), new Point(new double[] { 2, 2 }), 0.25);
            IControlSet controlSet2 = new RectangularControlSet(new Point(new double[] { -1, -1 }), new Point(new double[] { 1, 1 }), 0.25);
            Function.function function = delegate(Point p, Point u, Point v)
            {
                return u + v;
            };

            UseParameters(function, terminalSet, FILENAME_SIMPLE);         
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "простые движения" c круглыми терминальным множеством и множествами управлений
        /// </summary>
        private static void StartCountingSimpleRound()
        {
            ITerminalSet terminalSet = new CircleTerminalSet(0.5);
            IControlSet controlSet1 = new CircleControlSet(new Point(new double[] { -2, -2 }), new Point(new double[] { 2, 2 }), 0.25);
            IControlSet controlSet2 = new CircleControlSet(new Point(new double[] { -1, -1 }), new Point(new double[] { 1, 1 }), 0.25);
            Function.function function = delegate(Point p, Point u, Point v)
            {
                return u + v;
            };

            UseParameters(function, terminalSet, FILENAME_SIMPLE_ROUND);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "материальная точка"
        /// из тетрадки. Вроде бы, у этой игры получается кривое множество, на котором надо считать функцию цены
        /// и эта игра подойдет для улучшения решетки.
        /// </summary>
        private static void StartMaterialPointGame()
        {
            double epsilon = 0.05;
            ITerminalSet terminalSet = new SquareTerminalSet(epsilon, new Point(new double[] { 0, 1 }));
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1 }), new Point(new double[] { 1 }), 0.1);
            IControlSet controlSet2 = new RectangularControlSet(new Point(new double[] { -0.5 }), new Point(new double[] { 0.5 }), 0.1);
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[2];
                coord[0] = p[1] + v[0];
                coord[1] = u[0];
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_MATERIAL_POINT);
        }

        /// <summary>
        /// Метод, начинающий пересчет задачи управления "материальная точка"        
        /// </summary>
        private static void StartMaterialPoint()
        {
            double epsilon = 0.05;
            ITerminalSet terminalSet = new SquareTerminalSet(epsilon, new Point(new double[] { 0, 1 }));
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1 }), new Point(new double[] { 1 }), 0.05);
            IControlSet controlSet2 = new PointControlSet(new Point(new double[] {0, 0}));
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[2];
                coord[0] = p[1];
                coord[1] = u[0];
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_MATERIAL_POINT);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "Нелинейный маятник"        
        /// </summary>
        private static void StartPendulum()
        {
            double epsilon = 0.05;
            ITerminalSet terminalSet = new SquareTerminalSet(epsilon, new Point(new double[] { 0, 0 }));
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1.5}), new Point(new double[] { 1.5 }), 0.05);
            IControlSet controlSet2 = new PointControlSet(new Point(new double[] { 0 }));
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[2];
                coord[0] = p[1];
                coord[1] = Math.Cos(p[0]) * Math.Sin(p[0]) + Math.Sin(p[0]) + u[0];
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_PENDULUM);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "шофер-убийца" с данными Сергея Сергеевича 
        /// </summary>
        private static void StartCountingChauffeur()
        {
            ITerminalSet terminalSet = new CircleTerminalSet(0.3);
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1, 0 }), new Point(new double[] { 1, 0 }), 0.05);
            IControlSet controlSet2 = new CircleControlSet(new Point(new double[] { -0.3, -0.3 }), new Point(new double[] { 0.3, 0.3 }), 0.05);
            Function.function function = delegate(Point p, Point u, Point v)
            {                
                double[] coord = new double[2];
                coord[0] = -p[1] * u[0] + v[0];
                coord[1] = p[0] * u[0] + v[1] - 1;
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_CHAUFFEUR);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "шофер-убийца" с данными из статьи М.Фальконе
        /// </summary>
        private static void StartCountingChauffeurFalcone()
        {
            ITerminalSet terminalSet = new CircleTerminalSet(0.1);
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1, 0}), new Point(new double[] { 1, 0 }), 0.05);
            IControlSet controlSet2 = new RectangularControlSet(new Point(new double[] { -Math.PI, 0 }), new Point(new double[] { Math.PI, 0 }), 0.2);
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[2];
                coord[0] = -5 * p[1] * u[0] + 0.5 * Math.Sin(v[0]);
                coord[1] = 5 * p[0] * u[0] + 0.5 * Math.Cos(v[0]) - 1;
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_CHAUFFEUR_FALCONE);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "трехмерная материальная точка"
        /// </summary>
        private static void StartCounting3DimMaterialPoint()
        {
            double epsilon = 0.5;
            ITerminalSet terminalSet = new SquareTerminalSet(epsilon, new Point(new double[] { 0, 0, 0 }));
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -2, 0, 0 }), new Point(new double[] { 2, 0, 0 }), 0.1);
            IControlSet controlSet2 = new RectangularControlSet(new Point(new double[] { -0.4, 0, 0 }), new Point(new double[] { 0.4, 0, 0 }), 0.1);
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[3];
                coord[0] = p[1] + v[0];
                coord[1] = p[2];
                coord[2] = u[0];
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_3D_MATERIAL_POINT);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "Машина Дубинса"
        /// </summary>
        private static void StartCountingDubinsCar()
        {
            double epsilon = 0.1;
            ITerminalSet terminalSet = new SquareTerminalSet(epsilon, new Point(new double[] { 0, 0, 0 }));
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1 }), new Point(new double[] { 1 }), 0.05);
            IControlSet controlSet2 = new PointControlSet(new Point(new double[] {0}));
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[3];
                coord[0] = Math.Cos(p[2]);
                coord[1] = Math.Sin(p[2]);
                coord[2] = u[0];
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_DUBINS_CAR);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры "Машина Дубинса"
        /// </summary>
        private static void StartCounting2DDubinsCar()
        {
            double epsilon = 0.1;
            ITerminalSet terminalSet = new SquareTerminalSet(epsilon, new Point(new double[] { 0, 0 }));
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -1 }), new Point(new double[] { 1 }), 0.05);
            IControlSet controlSet2 = new PointControlSet(new Point(new double[] { 0 }));
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[2];
                coord[0] = -p[1] * u[0];
                coord[1] = p[0] * u[0] - 1;                
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_2D_DUBINS_CAR);
        }

        /// <summary>
        /// Метод, начинающий пересчет игры Target-Attacker-Defender        
        /// </summary>
        private static void StartTadProblem()
        {
            double Rc = 0.5, rc = 1, alpha = 0.3, beta = 0.6;
            ITerminalSet terminalSet = new TadTerminalSet(rc);
            IFailureSet failureSet = new TadFailureSet(Rc);
            IControlSet controlSet1 = new RectangularControlSet(new Point(new double[] { -Math.PI }), new Point(new double[] { Math.PI }), 0.1);
            IControlSet controlSet2 = new RectangularControlSet(new Point(new double[] { -Math.PI, -Math.PI }), new Point(new double[] { Math.PI, Math.PI }), 0.1);
            // phi = v[0], psi = v[1], xsi = u[0]
            // R = coords[0], r = coord[1], theta = coord[2]            
            Function.function function = delegate(Point p, Point u, Point v)
            {
                double[] coord = new double[3];
                // \dot R = alpha * cos(phi) - cos(theta - xsi)
                coord[0] = alpha * Math.Cos(v[0]) - Math.Cos(p[2] - u[0]);
                // \dot r = - cos(xsi) - beta cos(psi)
                coord[1] = - Math.Cos(u[0]) - beta * Math.Cos(v[1]);
                // \dot theta = - (alpha / R) * sin(phi) + 1/R sin(theta - xsi) - beta/r * sin(psi) + 1/r sin(xsi)
                if (p[0] == 0 && p[1] == 0)
                {
                    if (Math.Sin(p[2] - u[0]) - alpha * Math.Sin(v[0]) + Math.Sin(u[0]) - beta * Math.Sin(v[1]) > 0)
                        coord[2] = 100;
                    else coord[2] = -100;
                }
                else if (p[0] == 0) 
                {
                    if (Math.Sin(p[2] - u[0]) - alpha * Math.Sin(v[0]) > 0)
                        coord[2] = 100; // будет такой аналог бесконечности
                    else coord[2] = -100;
                    //coord[2] = -(beta / p[1]) * Math.Sin(v[1]) + (1 / p[1]) * Math.Sin(u[0]);                
                }
                else if (p[1] == 0)
                {
                    if (Math.Sin(u[0]) - beta * Math.Sin(v[1]) > 0)
                        coord[2] = 100; // будет такой аналог бесконечности
                    else coord[2] = -100;
                    //coord[2] = -(alpha / p[0]) * Math.Sin(v[0]) + (1 / p[0]) * Math.Sin(p[2] - u[0]);
                }
                else
                {
                    coord[2] = -(alpha / p[0]) * Math.Sin(v[0]) + (1 / p[0]) * Math.Sin(p[2] - u[0]) - (beta / p[1]) * Math.Sin(v[1]) + (1 / p[1]) * Math.Sin(u[0]);
                }
                return new Point(coord, true);
            };

            UseParameters(function, terminalSet, FILENAME_TAD_PROBLEM, failureSet);
        }

        /// <summary>
        /// Метод считывает параметры задачи из xml-файла
        /// </summary>
        /// <param name="filename">Имя файла с параметрами задачи</param>
        /// <returns></returns>
        private static GameParameters GetParameters(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameParameters));            
            using (XmlReader reader = XmlReader.Create(filename))
            {
                var parameters = (GameParameters)serializer.Deserialize(reader);
                return parameters;
            }           
        }

        /// <summary>
        /// Метод считывает параметры задачи и запускает инициализацию с помощью параметров
        /// либо использует некоторые параметры по умолчанию
        /// </summary>
        /// <param name="function">Функция динамики</param>
        /// <param name="controlSet1">Множество управлений первого игрока</param>
        /// <param name="controlSet2">Множество управлений второго игрока</param>
        /// <param name="terminalSet">Терминальное множество</param>
        private static void UseParameters(Function.function function, ITerminalSet terminalSet, string filename, IFailureSet failureSet = null)
        {
            var parameters = GetParameters(filename);
            var controlSetU = MakeControlSet(parameters.U);
            var controlSetV = MakeControlSet(parameters.V);
            Initialization(parameters, function, controlSetU, controlSetV, terminalSet, failureSet);

            //else Initialization(new GameParameters()
            //{
            //    TimeStep = 0.5,
            //    Iter = 11,
            //    SizeOfBox = new double[] { 0.25, 0.25 },
            //    SaveIter = 1
            //}, function, controlSet1, controlSet2, terminalSet, failureSet);
        }

        private static IControlSet MakeControlSet(ControlSetParameter parameters)
        {
            switch (parameters.Type)
            {
                case "Circle":
                    return new CircleControlSet(parameters.Start, parameters.End, parameters.Step);
                case "Boundary":
                    return new BoundaryControlSet(parameters.Start, parameters.End, parameters.Step);
                case "Point":
                    return new PointControlSet(parameters.Point);
                case "Rectangular":
                    return new RectangularControlSet(parameters.Start, parameters.End, parameters.Step);
                case "Ring":
                    return new RingControlSet(parameters.Center, parameters.Radius, parameters.Step);
                default:
                    return new PointControlSet(new Point(new double[] { 0 }));
            }            
        }

        /// <summary>
        /// Метод инициализирует необходимые классы с помощью параметров задачи и запускает счет
        /// </summary>
        /// <param name="parameter">Параметры задачи, считываемые из xml-файла</param>
        /// <param name="function">Функция динамики</param>
        /// <param name="controlSet1">Множество управлений первого игрока</param>
        /// <param name="controlSet2">Множество управлений второго игрока</param>
        /// <param name="terminalSet">Терминальное множество</param>
        private static void Initialization(GameParameters parameter, Function.function function, IControlSet controlSet1, IControlSet controlSet2, ITerminalSet terminalSet, IFailureSet failureSet = null)
        {               
            double[] sizeOfBox = new double[parameter.GridParameters.SizeOfBox.Length];
            parameter.GridParameters.SizeOfBox.CopyTo(sizeOfBox, 0);            
            Grid grid = new Grid(parameter.GridParameters.SizeOfDomain, sizeOfBox, parameter.GridParameters.InitialPoint, terminalSet);
                                  
            Compute worker = new Compute(grid:          grid,
                                         function:      function,
                                         controlSet1:   controlSet1,
                                         controlSet2:   controlSet2,
                                         terminalSet:   terminalSet,
                                         iters:         parameter.Iter,
                                         timeStep:      parameter.GridParameters.TimeStep,
                                         saveIter:      parameter.SaveIter,
                                         failureSet:    failureSet,
                                         folderName:    parameter.FolderName);

            Console.WriteLine("Would you like to start a new instance? y / n");
            char key = Console.ReadKey().KeyChar;
            if (key == 'y')
            {
                Console.WriteLine();
                worker.Work();
            }
            else if (key == 'n')
            {
                Console.WriteLine("Please enter a path to the file to continue");
                string path = Console.ReadLine();
                worker.Continue(path);
            }
            else Console.WriteLine("pfffff");            
        }        
    }
}
