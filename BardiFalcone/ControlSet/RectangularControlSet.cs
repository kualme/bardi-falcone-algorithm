using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.ControlSet
{
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
}
