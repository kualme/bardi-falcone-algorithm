using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone
{
    class UpgradeGrid
    {
        private Grid _grid;
        private Function.function _function;
        private IControlSet _controlSet1;
        private IControlSet _controlSet2;
        private Point[] _basis;

        //public Dictionary<Point, Point> result { get; set; }
        public List<Face> result { get; set; }

        public UpgradeGrid(Grid grid, Function.function function, IControlSet controlSet1, IControlSet controlSet2)
        {
            _grid = grid;
            _function = function;
            _controlSet1 = controlSet1;
            _controlSet2 = controlSet2;
            FillBasis();
            //result = new Dictionary<Point, Point>();
            result = new List<Face>();
        }

        public void FillBasis()
        {
            _basis = new Point[_grid.dim];
            for (int i = 0; i < _grid.dim; i++)
            {
                double[] coords = new double[_grid.dim];
                //coords[i] = _grid.sizeOfBox[i];
                coords[i] = 1;
                _basis[i] = new Point(coords);
            }
        }

        #region hide
        /*
        public void AddCubes()
        {
            Point basis = null;
            foreach (Point point in _grid.points[0].Keys)
            {
                if (NeedToAddCube(point, out basis))
                {
                    //_grid.points[0].Add(new Point((point + basis).Coords, true), new Value(1.0));
                    _grid.points[1].Add(new Point((point + basis).Coords, true), new Value(1.0));
                }
            }
        }
        */
        /*
        public bool NeedToAddCube(Point p, out Point basis)
        {
            double max = 0;
            double min = 1E10;
            Point bestControl1 = null;
            Point bestControl2 = null;
            basis = null;
            for (int i = 0; i < _grid.dim; i++)
            {
                foreach (Point control1 in _controlSet1.Set)
                {
                    foreach (Point control2 in _controlSet2.Set)
                    {
                        double res1 = _function(p, control1, control2) * _basis[i];
                        double res2 = _function(p, control1, control2) * (-_basis[i]);
                        if (res1 >= max)
                        {
                            max = res1;
                            basis = _basis[i];
                            bestControl2 = control2;
                        }
                        else if (res2 > max)
                        {
                            max = res2;
                            basis = -_basis[i];
                            bestControl2 = control2;
                        }
                    }
                    if (max <= min)
                    {
                        min = max;
                        bestControl1 = control1;
                    }
                }
            }

            if (_function(p, bestControl1, bestControl2) * basis > 0)
            {
                result.Add(p, basis);
                return true;
            }
            else return false;
                
            /*
            basis = null;
            int idx = 0;
            for (int i = 0; i < _grid.dim; i++)
            {
                foreach (Point control2 in _controlSet2.Set)
                {
                    foreach (Point control1 in _controlSet1.Set)
                    {
                        double res = _function(p, control1, control2) * (p - _basis[i]);
                        if (res > 0)
                        {
                            //basis = _basis[i];
                            idx++;
                        }
                    }
                    if (idx == _controlSet1.Set.Length)
                    {
                        basis = - _basis[i];
                        result.Add(p, basis);
                        return true;
                    }
                    idx = 0;
                }
            }

            for (int i = 0; i < _grid.dim; i++)
            {
                foreach (Point control2 in _controlSet2.Set)
                {
                    foreach (Point control1 in _controlSet1.Set)
                    {
                        double res = _function(p, control1, control2) * (p + _basis[i]);
                        if (res > 0)
                        {
                            //basis = _basis[i];
                            idx++;
                        }
                    }
                    if (idx == _controlSet1.Set.Length)
                    {
                        basis = _basis[i];
                        result.Add(p, basis);
                        return true;
                    }
                    idx = 0;
                }
            }
            return false;
            
        }
    */
        #endregion

        /// <summary>
        /// Метод проверяет, необходимо ли к этой грани добавить еще "куб" решетки
        /// Т.е. проверяем, является ли эта грань частью допустимой области. Если является, то ничего добавлять не надо
        /// </summary>
        /// <param name="face">Грань</param>
        /// <returns></returns>
        public bool NeedToAddCube(Face face)
        {
            
            double min = 1E10;
            Point bestControl1 = null;
            Point bestControl2 = null;            
            
            foreach (Point control1 in _controlSet1.Set)
            {
                double max = 0;
                foreach (Point control2 in _controlSet2.Set)
                {
                    double res = _function(face.x, control1, control2) * (_basis[face.k] * face.direction);                        
                    if (res >= max)
                    {
                        max = res;                            
                        bestControl2 = control2;
                    }                        
                }
                if (max <= min)
                {
                    min = max;
                    bestControl1 = control1;
                }                    
            }
            
            if (bestControl1 == null || bestControl2 == null)
                return false;

            if (_function(face.x, bestControl1, bestControl2) * (_basis[face.k] * face.direction) > 0)
            {
                result.Add(face);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Метод добавления "куба" решетки к данной грани
        /// </summary>
        /// <param name="face"></param>
        public void AddCube(Face face)
        {
            Point newPoint = face.x + _basis[face.k] * face.direction * _grid.sizeOfBox[face.k];
            _grid.points[0].Add(newPoint, new Value(1.0));
            _grid.points[1].Add(newPoint, new Value(1.0));
        }

        /// <summary>
        /// Метод добавления новой грани к данной грани
        /// </summary>
        /// <param name="face"></param>
        private void AddFace(Face face)
        {
 
        }
    }
}
