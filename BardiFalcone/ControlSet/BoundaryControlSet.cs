using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.ControlSet
{
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
}
