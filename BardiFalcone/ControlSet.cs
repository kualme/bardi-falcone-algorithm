using System;
using System.Collections.Generic;

namespace BardiFalcone
{
    interface IControlSet
    {
        Point[] Set { get; }
    }

    /// <summary>
    /// Множество управлений - прямоугольник
    /// </summary>
    class RectangularControlSet : IControlSet
    {
        /// <summary>
        /// Множество управлений игрока
        /// </summary>
        private Point[] _set;

        /// <summary>
        /// Множество управлений игрока
        /// </summary>        
        public Point[] Set { get { return _set; } }
        
        public RectangularControlSet(Point start, Point end, double step)
        {        
            _set = GenerateControlSet(start, end, step);
        }

        /// <summary>
        /// Функция генерирует прямоугольное (box) множество управлений
        /// </summary>
        /// <param name="start">Нижняя левая точка множества</param>
        /// <param name="end">Правая верхняя точка множества</param>
        /// <param name="step">Шаг</param>
        /// <returns>Возвращает массив точек</returns>
        public Point[] GenerateControlSet(Point start, Point end, double step)
        {
#if DEBUG
            if (start.Dim != end.Dim)
                throw new ArgumentException("Dimensions of points are different");
#endif
            List<Point> points = new List<Point>();
            int[] qntSteps = new int[start.Dim];
            for (int i = 0; i < start.Dim; i++)
                qntSteps[i] = (int)Math.Round((end[i] - start[i]) / step);

            double[] initialCoords = new double[start.Dim];
            start.Coords.CopyTo(initialCoords, 0);
            AddPoints(0, initialCoords, points, start.Dim, step, start, qntSteps);
            return points.ToArray();
        }

        /// <summary>
        /// Функция рекурсивно добавляет точки к списку points
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="coordinates"></param>
        /// <param name="points"></param>
        /// <param name="dim"></param>
        /// <param name="step"></param>
        /// <param name="initialPoint"></param>
        /// <param name="qntSteps"></param>
        private void AddPoints(int dimension, double[] coordinates, List<Point> points, int dim, double step, Point initialPoint, int[] qntSteps)
        {
            if (dimension >= dim)
                points.Add(new Point(coordinates, true));            
            else
            {
                for (int i = 0; i <= qntSteps[dimension]; i++)
                {
                    coordinates[dimension] = initialPoint[dimension] + i * step;
                    AddPoints(dimension + 1, coordinates, points, dim, step, initialPoint, qntSteps);
                }
            }
        }        
    }

    /// <summary>
    /// Множество управлений - круг
    /// </summary>
    class CircleControlSet : IControlSet
    {
        /// <summary>
        /// Множество управлений игрока
        /// </summary>
        private Point[] _set;

        /// <summary>
        /// Множество управлений игрока
        /// </summary>        
        public Point[] Set { get { return _set; } }

        public CircleControlSet(Point start, Point end, double step)
        {
            _set = GenerateCircleControlSet(start, end, step);
        }

        /// <summary>
        /// Метод строит круглое множество ограничений на управление. Этот круг вписан в квадрат, образованный значениями 
        /// start, end и step
        /// </summary>
        /// <param name="start">Нижняя левая точка квадрата</param>
        /// <param name="end">Правая верхняя точка квадрата</param>
        /// <param name="step">Шаг</param>
        /// <returns>Возвращает массив точек</returns>
        private Point[] GenerateCircleControlSet(Point start, Point end, double step)
        {
            List<Point> points = new List<Point>();
            int[] qntSteps = new int[start.Dim];
            for (int i = 0; i < start.Dim; i++)
                qntSteps[i] = (int)Math.Round((end[i] - start[i]) / step);

            double radius = step * qntSteps[0] / 2;
            double[] initialCoords = new double[start.Dim];
            start.Coords.CopyTo(initialCoords, 0);
            double[] centerCoords = new double[start.Dim];
            for (int i = 0; i < start.Dim; i++ )
                centerCoords[i] = (start.coords[i] + end.coords[i]) / 2;
            Point center = new Point(centerCoords);
            AddPointsCircle(0, initialCoords, points, start.Dim, step, start, qntSteps, radius, center);
            return points.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="coordinates"></param>
        /// <param name="points"></param>
        /// <param name="dim"></param>
        /// <param name="step"></param>
        /// <param name="initialPoint"></param>
        /// <param name="qntSteps"></param>
        /// <param name="radius"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        private void AddPointsCircle(int dimension, double[] coordinates, List<Point> points, int dim, double step, Point initialPoint, int[] qntSteps, double radius, Point center)
        {
            if (dimension >= dim)
            {
                Point point = new Point(coordinates, true);
                double value = 0.0;
                for (int i = 0; i < dim; i++ )
                    value += (point[i] - center.coords[i]) * (point[i] - center.coords[i]);
                if (Math.Sqrt(value) <= radius)
                    points.Add(new Point(coordinates, true));
                //if (Math.Sqrt((point[0] - center.coords[0]) * (point[0] - center.coords[0] ) + (point[1] - center.coords[1]) * (point[1] - center.coords[1])) <= radius)                    
            }
            else
            {
                for (int i = 0; i <= qntSteps[dimension]; i++)
                {
                    coordinates[dimension] = initialPoint[dimension] + i * step;
                    AddPointsCircle(dimension + 1, coordinates, points, dim, step, initialPoint, qntSteps, radius, center);
                }
            }
        }
    }

    /// <summary>
    /// Множество управлений - окружность
    /// </summary>
    class RingControlSet : IControlSet
    {
        /// <summary>
        /// Множество управлений игрока
        /// </summary>
        private Point[] _set;

        /// <summary>
        /// Множество управлений игрока
        /// </summary>        
        public Point[] Set { get { return _set; } }

        public RingControlSet(Point center, double radius, double step)
        {
            List<Point> points = new List<Point>();            
            for (double angle = 0; angle <= 2 * Math.PI; angle = angle + step)
            {
                points.Add(new Point(new double[] { center.coords[0] + Math.Cos(angle) * radius, center.coords[1] + Math.Sin(angle) * radius }, true));
            }
            _set = points.ToArray();
        }
    }

    /// <summary>
    /// Множество управлений - граница прямоугольника (только для размерности 2)
    /// </summary>
    class BoundaryControlSet : IControlSet
    {
        /// <summary>
        /// Множество управлений игрока
        /// </summary>
        private Point[] _set;

        /// <summary>
        /// Множество управлений игрока
        /// </summary>        
        public Point[] Set { get { return _set; } }

        public BoundaryControlSet(Point start, Point end, double step)
        {
            _set = GenerateBoundaryControlSet(start, end, step);
        }

        /// <summary>
        /// Функция генерирует множество управлений, состоящее только из границы
        /// Для размерности 2 множества управлений
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        private Point[] GenerateBoundaryControlSet(Point start, Point end, double step)
        {
#if DEBUG
            if (start.Dim != end.Dim)
                throw new ArgumentException("Dimensions of points are different");
#endif
            List<Point> points = new List<Point>();
            int[] qntSteps = new int[start.Dim];
            for (int i = 0; i < start.Dim; i++)
                qntSteps[i] = (int)Math.Round((end[i] - start[i]) / step);

            for (int i = 0; i < start.Dim; i++)
            {
                for (int j = 0; j < qntSteps[i]; j++)
                {
                    Point addLeftPoint = start.Copy();
                    addLeftPoint[i] += j * step;
                    points.Add(addLeftPoint);
                    Point addRightPoint = end.Copy();
                    addRightPoint[i] -= j * step;
                    points.Add(addRightPoint);
                }
            }
            points.Add(new Point(new double[] { end[0], start[1] }, true));
            points.Add(new Point(new double[] { start[1], end[0] }, true));
            return points.ToArray();
        }
    }

    /// <summary>
    /// Множество ограничений на управление - одноточечное множество
    /// </summary>
    class PointControlSet : IControlSet
    {
        /// <summary>
        /// Множество управлений игрока
        /// </summary>
        private Point[] _set;

        /// <summary>
        /// Множество управлений игрока
        /// </summary>        
        public Point[] Set { get { return _set; } }

        public PointControlSet(Point point)
        {
            _set = new Point[] { point };
        }
    }
}

#region Старые функции
/*
        public static Point[] generateSquareSet(Point p1, Point p2, double step)
        {
#if DEBUG
            if (p1.dim != p2.dim)
                throw new ArgumentException("Dimensions of points are different");
#endif
            Point center = (p1 + p2) * .5;
            int count = 1;
            for (int i = 0; i < p1.dim; ++i)
                count *= (int)Math.Floor((p2[i] - p1[i]) / step);
            Point[] ans = new Point[count];
            int curCount = 1;
            ans[0] = (Point)p1.Copy();
            for (int i = 0; i < p1.dim; ++i)
            {
                int countAtDim = (int)Math.Floor((p2[i] - p1[i]) / step);
                for (int j = curCount - 1; j >= 0; --j)
                {
                    for (int k = 0; k < countAtDim; ++k)
                    {
                        ans[j * countAtDim + k] = (Point)ans[j].Copy();                        
                        ans[j * countAtDim + k][i] = p1[i] + step * k;
                    }
                }
                curCount *= countAtDim;
            }
            return ans;
        }

        public static Point[] generateEllipseSet(Point p1, Point p2, double step)
        {
            Point[] inSquare = generateSquareSet(p1, p2, step);
            Point center = (p1 + p2) * 0.5;
            Point[] ans = new Point[inSquare.Length];
            int count = 0;
            foreach (Point x in inSquare)
            {
                double d = 0;
                for (int i = 0; i < p1.dim; ++i)
                    d += (x[i] - center[i]) * (x[i] - center[i]) / ((p2[i] - center[i]) * (p2[i] - center[i]));
                if (d <= 1)
                    ans[count++] = x;
            }
            Point[] ans2 = new Point[count];
            for (int i = 0; i < count; ++i)
                ans2[i] = (Point)ans[i].Copy();
            return ans2;
        }
        */
#endregion