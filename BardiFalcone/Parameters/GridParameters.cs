using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.Parameters
{
    /// <summary>
    /// Параметры решетки
    /// </summary>
    [Serializable]
    public class GridParameters
    {
        /// <summary>
        /// Шаг по пространству
        /// </summary>
        public double[] SizeOfBox { get; set; }

        /// <summary>
        /// Шаг по времени
        /// </summary>
        public double TimeStep { get; set; }

        /// <summary>
        /// Размер области
        /// </summary>
        public double[] SizeOfDomain { get; set; }

        /// <summary>
        /// Начальная точка
        /// </summary>
        public Point InitialPoint { get; set; }
    }
}
