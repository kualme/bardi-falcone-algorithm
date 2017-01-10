using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.ControlSet
{
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
