using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolygonLibrary
{
    /// <summary>
    /// Class with general purpose procedures
    /// </summary>
    public class Tools
    {
        #region Double comparison

        /// <summary>
        /// Absolute accuracy for comparison
        /// </summary>
        static private double _eps = 1e-8;

        /// <summary>
        /// Property to deal with the accuracy
        /// </summary>
        static public double Eps
        {
            get { return _eps; }
            set 
            {
#if DEBUG
                if (value <= 0)
                    throw new ArgumentOutOfRangeException ();
#endif
                _eps = value; 
            }
        }
        
        /// <summary>
        /// Comparer of two doubles with the precision
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>+1, if a > b; -1, if a < b; 0, otherwise</returns>
        static public int CMP (double a, double b = 0)
        {
            if (Tools.EQ (a, b))
                return 0;
            else if (a > b)
                return +1;
            else
                return -1;
        }
        
        /// <summary>
        /// Equality of two doubles
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true, if |a-b|&lt;eps; false, otherwise</returns>
        static public bool EQ (double a, double b = 0)
        {
            return Math.Abs(a - b) < _eps;
        }

        /// <summary>
        /// Non-equality of two doubles
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true, if |a-b|&gt;=eps; false, otherwise</returns>
        static public bool NE (double a, double b = 0)
        {
             return !EQ(a,b);
        }

        /// <summary>
        /// "Greater" comparison
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true, if a&gt;=b+eps; false, otherwise</returns>
        static public bool GT (double a, double b = 0)
        {
             return a >= b + _eps;
        }

        /// <summary>
        /// "Greater or equal" comparison
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true, if a&gt;=b-eps; false, otherwise</returns>
        static public bool GE (double a, double b = 0)
        {
             return a > b - _eps;
        }

        /// <summary>
        /// "Less" comparison
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true, if a&lt;=b-eps; false, otherwise</returns>
        static public bool LT (double a, double b = 0)
        {
             return a <= b - _eps;
        }

        /// <summary>
        /// "Less or equal" comparison
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true, if a&lt;b+eps; false, otherwise</returns>
        static public bool LE (double a, double b = 0)
        {
             return a < b + _eps;
        }

        #endregion              
    }
}

