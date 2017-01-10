using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.TerminalSet
{
    /// <summary>
    /// Терминальное множество в виде куба с центром в конкретной точке
    /// </summary>
    class SquareTerminalSet : ITerminalSet
    {
        /// <summary>
        /// Половина длины стороны куба
        /// </summary>
        private double _halfSide;

        /// <summary>
        /// Центр куба
        /// </summary>
        private Point _center;

        /// <summary>
        /// Конструктор терминального множества в виде куба со стороной равной единице с центром в начале координат
        /// </summary>        
        public SquareTerminalSet(int dim = 2)
        {
            _halfSide = 1;
            _center = new Point(new double[dim]);
        }

        /// <summary>
        /// Конструктор терминального множества в виде куба с центром в начале координат
        /// </summary>
        /// <param name="halfSide">Половина стороны квадрата</param>
        public SquareTerminalSet(double halfSide, int dim = 2)
        {
            _halfSide = halfSide;
            _center = new Point(new double[dim]);
        }

        /// <summary>
        /// Конструктор терминального множества в виде куба с центром в конкретной точке
        /// </summary>
        /// <param name="halfSide">Половина стороны куба</param>
        /// <param name="center">Центр куба</param>
        public SquareTerminalSet(double halfSide, Point center)
        {
            _halfSide = halfSide;
            _center = center;
        }

        /// <summary>
        /// Возвращает true, если попадаем в терминальное множество - квадрат со стороной _halfSide * 2 с центром в нуле
        /// </summary>
        /// <param name="p">Точка</param>
        /// <returns></returns>
        public bool BelongsToTerminalSet(Point p)
        {
            double value = 0.0;
            for (int i = 0; i < p.Dim; i++)
                value = Math.Max(Math.Abs(p[i] - _center[i]), value);            
            return value <= _halfSide;            
        }
    }
}
