using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone
{
    /// <summary>
    /// Внешняя грань
    /// Находим ее таким образом: пусть point - точка, принадлежащая решетке. Если точка point + e_k != point_another, 
    /// где e_k - базисный вектор (но не нормированный), а point_another - какая-то другая точка, 
    /// принадлежащая решетке, то e_k - вектор внешней нормали!
    /// </summary>
    public struct Face
    {
        /// <summary>
        /// Support point - левая нижняя точка куба, которому принадлежит грань, от которого вычислилась внешняя нормаль
        /// </summary>
        private Point _x;

        /// <summary>
        /// Номер направления вектора нормали (со знаком). Если k < 0, то берется направление, противоположное направлению вектора e_k
        /// </summary>
        private int _k;

        /// <summary>
        /// Направление базисного вектора. Может быть равно 1 или -1
        /// </summary>
        private int _direction;

        public Face(Point x, int k, int direction)
        {
            this._x = x.Copy();
            this._k = k;
            this._direction = direction;
        }

        public Point x
        {
            get { return _x; }
        }

        public int k
        {
            get { return _k; }
        }

        public int direction
        {
            get { return _direction; }
        }
    }
}
