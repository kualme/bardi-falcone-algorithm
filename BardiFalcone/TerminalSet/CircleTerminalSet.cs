using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.TerminalSet
{
    /// <summary>
    /// Терминальное множество в виде шара с центром в конкретной точке
    /// </summary>
    class CircleTerminalSet : ITerminalSet
    {
        /// <summary>
        /// Радиус шара
        /// </summary>
        private double _radius;

        /// <summary>
        /// Центр шара
        /// </summary>
        private Point _center;

        /// <summary>
        /// Конструктор терминального множества в виде шара единичного радиуса с центром в начале координат
        /// </summary>        
        public CircleTerminalSet(int dim = 2)
        {
            _radius = 1;
            _center = new Point(new double[2]);
        }

        /// <summary>
        /// Конструктор терминального множества в виде шара с центром в начале координат
        /// </summary>
        /// <param name="halfSide">Радиус шара</param>
        public CircleTerminalSet(double radius, int dim = 2)
        {
            _radius = radius;
            _center = new Point(new double[2]);
        }

        /// <summary>
        /// Конструктор терминального множества в виде шара с центром в конкретной точке
        /// </summary>
        /// <param name="radius">Радиус шара</param>
        /// <param name="center">Центр шара</param>
        public CircleTerminalSet(double radius, Point center)
        {
            _radius = radius;
            _center = center;
        }

        /// <summary>
        /// Возвращает true, если попадаем в терминальное множество - шар радиуса _radius с центром в точке _center
        /// </summary>
        /// <param name="p">Точка</param>
        /// <returns></returns>
        public bool BelongsToTerminalSet(Point p)
        {
            double value = 0.0;
            for (int i = 0; i < p.Dim; i++)
                value += (p[i] - _center[i]) * (p[i] - _center[i]);
            return Math.Sqrt(value) <= _radius;
        }
    }
}
