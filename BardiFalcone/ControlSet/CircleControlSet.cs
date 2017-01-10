using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.ControlSet
{
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
            for (int i = 0; i < start.Dim; i++)
                centerCoords[i] = (start.coords[i] + end.coords[i]) / 2;
            Point center = new Point(centerCoords);
            AddPointsCircle(0, initialCoords, points, start.Dim, step, start, qntSteps, radius, center);
            return points.ToArray();
        }

        private void AddPointsCircle(int dimension, double[] coordinates, List<Point> points, int dim, double step, Point initialPoint, int[] qntSteps, double radius, Point center)
        {
            if (dimension >= dim)
            {
                Point point = new Point(coordinates, true);
                double value = 0.0;
                for (int i = 0; i < dim; i++)
                    value += (point[i] - center.coords[i]) * (point[i] - center.coords[i]);
                if (Math.Sqrt(value) <= radius)
                    points.Add(new Point(coordinates, true));                
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
}
