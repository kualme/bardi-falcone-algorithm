using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.ControlSet
{
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
}
