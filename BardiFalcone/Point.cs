using System;
using PolygonLibrary;

namespace BardiFalcone
{
    [Serializable]
    public class Point : IEquatable<Point>
    {
        public double[] coords;
        
        public int Dim { get; set; }
                       
        public const int MAXDIM = 10;

        public Point()
        {
            this.Dim = 2;                        
            this.coords = new double[2];                        
        }

        /// <summary>
        /// Конструткор точки
        /// </summary>
        /// <param name="coords">Координаты точки</param>
        /// <param name="copyFlag">Флаг, определяющий, копировать ли массив с координатами точки</param>
        public Point(double[] coords, bool copyFlag = false)        
        {
#if DEBUG
            if (coords == null)
                throw new ArgumentNullException("coords");
#endif
            int dim = coords.Length;
#if DEBUG
            if (dim <= 0 || dim > MAXDIM)
                throw new ArgumentException(string.Format("Invalid dimension: {0}", dim), "coords");
#endif
            this.Dim = dim;

            if (copyFlag)
            {
                this.coords = new double[dim];
                coords.CopyTo(this.coords, 0);
            }
            else 
                this.coords = coords;            
        }

        public double this[int idx]
        {
            get
            {
#if DEBUG
                if (idx < 0 || idx >= Dim)
                    throw new ArgumentOutOfRangeException("idx");
#endif
                return coords[idx];
            }
            set
            {
#if DEBUG
                if (idx < 0 || idx >= Dim)
                    throw new ArgumentOutOfRangeException("idx");
#endif
                coords[idx] = value;
            }
        }

        /*
        public int getDim()
        {
            return dim;
        }
         */

        public override string ToString()
        {
            string r = "(";
            for (int i = 0; i < Dim; ++i)
                r += (r != "(" ? ", " : "") + this[i];
            r += ")";
            return r;
        }

        public double[] Coords
        {
            get { return coords; }
        }

        static public Point operator -(Point point1, Point point2)
        {
#if DEBUG
            if (point1.Dim != point2.Dim)
                throw new ArgumentException("Different dimensions");
#endif
            double[] arr = new double[point1.Dim];
            for (int i = 0; i < point1.Dim; ++i)
                arr[i] = point1[i] - point2[i];
            return new Point(arr);
        }

        static public Point operator -(Point point)
        {
            double[] arr = new double[point.Dim];
            for (int i = 0; i < point.Dim; ++i)
                arr[i] = - point[i];
            return new Point(arr);
        }

        static public Point operator +(Point point1, Point point2)
        {
#if DEBUG
            if (point1.Dim != point2.Dim)
                throw new ArgumentException("Different dimensions");
#endif
            double[] arr = new double[point1.Dim];
            for (int i = 0; i < point1.Dim; ++i)
                arr[i] = point1[i] + point2[i];
            return new Point(arr);
        }

        static public Point operator *(Point point, double coef)
        {
            double[] arr = new double[point.Dim];
            for (int i = 0; i < point.Dim; ++i)
                arr[i] = point[i] * coef;
            return new Point(arr);
        }

        static public double operator *(Point point1, Point point2)
        {
            double result = 0;
            for (int i = 0; i < point1.Dim; i++)
                result += point1.Coords[i] * point2.Coords[i];
            return result;
        }

        public bool Equals(Point other)
        {            
            if (Dim != other.Dim)
                return false;
            for (int i = 0; i < Dim; ++i)
                if (Tools.NE(this[i], other[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            int ans = 0;
            for (int i = 0; i < Dim; ++i)
                ans += (int)Math.Floor(this[i]);
            return ans;
        }

        public Point Copy()
        {
            return new Point(coords, true);
        }
    }
}