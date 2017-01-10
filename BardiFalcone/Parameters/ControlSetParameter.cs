using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.Parameters
{
    /// <summary>
    /// Параметры для инициализации множества управлений
    /// </summary>
    [Serializable]
    public class ControlSetParameter
    {
        /// <summary>
        /// Начальная точка; задается для типов: Rectangular, Circle, Boundary
        /// </summary>
        public Point Start { get; set; }

        /// <summary>
        /// Конечная точка; задается для типов: Rectangular, Circle, Boundary
        /// </summary>
        public Point End { get; set; }

        /// <summary>
        /// Шаг; задается для типов: Rectangular, Circle, Boundary, Ring
        /// </summary>
        public double Step { get; set; }

        /// <summary>
        /// Точка одноточечного множества для типа Point
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Центр кольца множества для типа Ring
        /// </summary>
        public Point Center { get; set; }

        /// <summary>
        /// Радиус кольца для типа Ring
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Тип множества управлений
        /// </summary>
        public string Type { get; set; }
    }
}
