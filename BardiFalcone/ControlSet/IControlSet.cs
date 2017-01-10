using System;
using System.Collections.Generic;

namespace BardiFalcone.ControlSet
{
    interface IControlSet
    {
        Point[] Set { get; }
    }
}

#region Старые функции
/*
        public static Point[] generateSquareSet(Point p1, Point p2, double step)
        {
#if DEBUG
            if (p1.dim != p2.dim)
                throw new ArgumentException("Dimensions of points are different");
#endif
            Point center = (p1 + p2) * .5;
            int count = 1;
            for (int i = 0; i < p1.dim; ++i)
                count *= (int)Math.Floor((p2[i] - p1[i]) / step);
            Point[] ans = new Point[count];
            int curCount = 1;
            ans[0] = (Point)p1.Copy();
            for (int i = 0; i < p1.dim; ++i)
            {
                int countAtDim = (int)Math.Floor((p2[i] - p1[i]) / step);
                for (int j = curCount - 1; j >= 0; --j)
                {
                    for (int k = 0; k < countAtDim; ++k)
                    {
                        ans[j * countAtDim + k] = (Point)ans[j].Copy();                        
                        ans[j * countAtDim + k][i] = p1[i] + step * k;
                    }
                }
                curCount *= countAtDim;
            }
            return ans;
        }

        public static Point[] generateEllipseSet(Point p1, Point p2, double step)
        {
            Point[] inSquare = generateSquareSet(p1, p2, step);
            Point center = (p1 + p2) * 0.5;
            Point[] ans = new Point[inSquare.Length];
            int count = 0;
            foreach (Point x in inSquare)
            {
                double d = 0;
                for (int i = 0; i < p1.dim; ++i)
                    d += (x[i] - center[i]) * (x[i] - center[i]) / ((p2[i] - center[i]) * (p2[i] - center[i]));
                if (d <= 1)
                    ans[count++] = x;
            }
            Point[] ans2 = new Point[count];
            for (int i = 0; i < count; ++i)
                ans2[i] = (Point)ans[i].Copy();
            return ans2;
        }
        */
#endregion