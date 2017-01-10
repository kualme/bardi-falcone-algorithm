using System;
using System.Collections.Generic;
using PolygonLibrary;
using BardiFalcone.TerminalSet;

namespace BardiFalcone
{
  #region Comparers

  [Serializable]
  class PointComparer : IComparer<Point>
  {
    private int _dim;
    private double[] _sizeOfBox;

    public PointComparer()
    {
      this._dim = 2;
      this._sizeOfBox = new double[] { 0.25, 0.25 };
    }

    public PointComparer(int dim, double[] sizeOfBox)
    {
      this._dim = dim;
      this._sizeOfBox = sizeOfBox;
    }

    public int Compare(Point x, Point y)
    {
      for (int i = 0; i < _dim; i++)
      {
        if (Tools.LE(x[i] + _sizeOfBox[i], y[i]))
          return -1;
        else if (Tools.GT(x[i], y[i]))
          return 1;
      }
      return 0;
    }
  }

  class idxPointComparer : IComparer<KeyValuePair<int, double>>
  {
    public int Compare(KeyValuePair<int, double> x, KeyValuePair<int, double> y)
    {
      return x.Value.CompareTo(y.Value);
    }
  }

  class FaceComparer : IComparer<Face>
  {
    private PointComparer _pointComparer;

    public FaceComparer(int dim, double[] sizeOfBox)
    {
      _pointComparer = new PointComparer(dim, sizeOfBox);
    }

    public int Compare(Face x, Face y)
    {
      if (!x.x.Equals(y.x))
        return _pointComparer.Compare(x.x, y.x);
      else if (x.k != y.k)
        return x.k.CompareTo(y.k);
      else return x.direction.CompareTo(y.direction);
    }
  }

  #endregion

  class Grid
  {
    #region Fields

    private idxPointComparer _idxComparer;

    private PointComparer _pointComparer;

    private FaceComparer _faceComparer;

    private ITerminalSet _terminalSet;

    private Point[] _basis;

    public List<Point> _keys;

    public int dim { get; set; }

    // private Point[,] points;
    public SortedDictionary<Point, Value>[] points { get; private set; }

    /// <summary>
    /// Массив количеств параллелотопов по координатам 
    /// </summary>
    public int[] quantityOfBoxes;

    /// <summary>
    /// Массив размеров параллелотопов по координатам
    /// </summary>
    public double[] sizeOfBox;

    /// <summary>
    /// Начальная точка, от которой ведется построение множества Q ("левая нижняя")
    /// </summary>
    public Point initialPoint;

    public SortedSet<Face> faces { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Конструктор решетки через задание размеров маленьких боксов
    /// </summary>
    /// <param name="dim">Размерность пространства</param>
    /// <param name="quantityOfBoxes">Массив количеств параллелотопов по каждому измерению</param>
    /// <param name="sizeOfBox">Массив размеров сторон параллелотопов по каждому измерению</param>
    /// <param name="initialPoint">Начальная точка построения решетки ("левая нижняя")</param>
    public Grid(int[] quantityOfBoxes, double[] sizeOfBox, Point initialPoint, ITerminalSet terminalSet)
    {
      if ((quantityOfBoxes.Length != sizeOfBox.Length) && (sizeOfBox.Length != initialPoint.Dim))
        throw new ArgumentException("Dimensions doesn't match");

      this._terminalSet = terminalSet;
      this.dim = quantityOfBoxes.Length;
      this.quantityOfBoxes = new int[this.dim];
      this.sizeOfBox = new double[this.dim];
      Array.Copy(quantityOfBoxes, this.quantityOfBoxes, this.dim);
      Array.Copy(sizeOfBox, this.sizeOfBox, this.dim);
      this.initialPoint = initialPoint.Copy();

      _pointComparer = new PointComparer(quantityOfBoxes.Length, sizeOfBox);
      _faceComparer = new FaceComparer(quantityOfBoxes.Length, sizeOfBox);

      points = new SortedDictionary<Point, Value>[2];
      points[0] = new SortedDictionary<Point, Value>(_pointComparer);
      points[1] = new SortedDictionary<Point, Value>(_pointComparer);

      AddGridPoints(0, initialPoint.Coords);

      _idxComparer = new idxPointComparer();

      this._keys = new List<Point>(points[0].Keys);
    }

    /// <summary>
    /// Конструктор решетки через задание размеров области
    /// </summary>
    /// <param name="sizeOfDomain">Массив длин сторон области по каждому измерению</param>
    /// <param name="sizeOfBox">Массив длин сторон параллелотопов по каждому измерению</param>
    /// <param name="initialPoint">Начальная точка построения решетки ("левая нижняя")</param>
    public Grid(double[] sizeOfDomain, double[] sizeOfBox, Point initialPoint, ITerminalSet terminalSet)
    {
      if ((sizeOfDomain.Length != sizeOfBox.Length) && (sizeOfBox.Length != initialPoint.Dim))
        throw new ArgumentException("Dimensions doesn't match");

      this._terminalSet = terminalSet;
      this.dim = sizeOfBox.Length;
      int[] quantityOfBoxes = new int[this.dim];
      for (int i = 0; i < this.dim; i++)
        quantityOfBoxes[i] = (int)(sizeOfDomain[i] / sizeOfBox[i]);

      this.quantityOfBoxes = new int[this.dim];
      this.sizeOfBox = new double[this.dim];
      Array.Copy(quantityOfBoxes, this.quantityOfBoxes, this.dim);
      Array.Copy(sizeOfBox, this.sizeOfBox, this.dim);
      this.initialPoint = initialPoint.Copy();

      //points = new HashSet<Point>();

      _pointComparer = new PointComparer(quantityOfBoxes.Length, sizeOfBox);
      _faceComparer = new FaceComparer(quantityOfBoxes.Length, sizeOfBox);

      points = new SortedDictionary<Point, Value>[2];
      points[0] = new SortedDictionary<Point, Value>(_pointComparer);
      points[1] = new SortedDictionary<Point, Value>(_pointComparer);

      double[] initialCoords = new double[dim];
      initialPoint.Coords.CopyTo(initialCoords, 0);
      AddGridPoints(0, initialCoords);

      _idxComparer = new idxPointComparer();

      this._keys = new List<Point>(points[0].Keys);
    }

    #endregion

    /// <summary>
    /// Рекурсивная функция для построения узлов решетки; строит и точки "по правому краю"
    /// Все точки, кроме точек "по правому краю", помечает флагом isSupportPoint,
    /// т.е. такая точка является левой нижней для какого-то куба
    /// </summary>
    /// <param name="dimension">Пространственная координата (по ней идет итерация)</param>
    /// <param name="coordinates">Координаты точки</param>
    private void AddGridPoints(int dimension, double[] coordinates)
    {
      if (dimension >= dim)
      {
        //points.Add(new Point(coordinates, true));
        Point point = new Point(coordinates, true);

        Value value = _terminalSet.BelongsToTerminalSet(point) ? new Value(true, 0) : new Value(false, 1);
        points[0].Add(point, value);
        points[1].Add(point, value);
      }
      else
      {
        for (int i = 0; i <= quantityOfBoxes[dimension]; i++)
        {
          coordinates[dimension] = initialPoint[dimension] + i * sizeOfBox[dimension];
          AddGridPoints(dimension + 1, coordinates);
        }
      }
    }

    /// <summary>
    /// Метод возвращает true, если точка попадает в множество Q
    /// </summary>
    /// <param name="p">Точка</param>
    /// <returns></returns>
    public virtual bool PointInside(Point p)
    {
      for (int i = 0; i < dim; ++i)
        //if (p[i] < initialPoint[i] ||
        //    p[i] >= initialPoint[i] + sizeOfBox[i] * quantityOfBoxes[i])
        if (Tools.LT(p[i], initialPoint[i]) ||
            Tools.GE(p[i], initialPoint[i] + sizeOfBox[i] * quantityOfBoxes[i]))
          return false;
      return true;
    }

    /// <summary>
    /// Возвращает нижнюю левую точку параллелотопа, которому принадлежит точка p
    /// </summary>
    /// <param name="point">Точка, не обязательно принадлежащая решетке</param>
    /// <returns></returns>
    public virtual bool FindSupportPoint(Point point, out Point supportPoint)
    {
      int idx = _keys.BinarySearch(point, _pointComparer);
      supportPoint = null;
      if (idx < 0)
      {
        return false;
      }
      else
      {
        supportPoint = _keys[idx];
        return true;
      }
      
      //supportPoint = new Point(new double[dim]);

      //// floor((x - alpha)/delta) - получили номер куба, в котором лежит точка            
      //for (int d = 0; d < dim; d++)
      //  supportPoint[d] = initialPoint[d] + Math.Floor((point[d] - initialPoint[d]) / sizeOfBox[d]) * sizeOfBox[d];
      //return true;
    }

    public virtual void Out()
    {
      Console.Out.WriteLine("dim = " + dim);
      Console.Out.WriteLine("quantity of points = " + points[0].Count);
      foreach (Point point in points[0].Keys)
      {
        Console.Out.Write(point.ToString());
        Console.Out.WriteLine();
      }
    }

    /// <summary>
    /// Метод возвращает барицентрические координаты точки point
    /// </summary>
    /// <param name="point">Точка</param>
    /// <param name="start">Левая нижняя точка куба, которому принадлежит точка point</param>        
    /// <returns></returns>
    public KeyValuePair<Point, double>[] GetBarycentricCoordinates(Point point, Point start)
    {
#if DEBUG
      if (start == null) throw new ArgumentNullException("start");
      if (point == null) throw new ArgumentNullException("point");
      if (point.Dim != dim)
        throw new ArgumentException(string.Format("Invalid point dimension for this PointSearcher: {0} instead of {1}", point.Dim, dim), "point");
#endif
      KeyValuePair<int, double>[] arr = new KeyValuePair<int, double>[dim];

      for (int i = 0; i < dim; ++i)
      {
        arr[i] = new KeyValuePair<int, double>(i, point[i] - start[i]); //p[i]);
#if DEBUG
        //if (arr[i].Value < 0 || arr[i].Value > sizeOfBox[i])
        //if (Tools.LT(arr[i].Value) || Tools.GE(arr[i].Value, sizeOfBox[i]))
        //    throw new ArgumentException("Point must be from [0-size] square", string.Format("point[{0}]", i));
#endif
      }

      Array.Sort(arr, _idxComparer);

      KeyValuePair<Point, double>[] coef = new KeyValuePair<Point, double>[dim + 1];

      coef[0] = new KeyValuePair<Point, double>
          (GeneratePoint(start, arr, 0), arr[0].Value / sizeOfBox[arr[0].Key]);
      double s = coef[0].Value;
      for (int i = 1; i < dim; i++)
      {
        coef[i] = new KeyValuePair<Point, double>(GeneratePoint(start, arr, i),
            arr[i].Value / sizeOfBox[arr[i].Key] -
            arr[i - 1].Value / sizeOfBox[arr[i - 1].Key]);
        s += coef[i].Value;
      }
      coef[dim] = new KeyValuePair<Point, double>(start, 1.0 - s);

      return coef;
    }

    /// <summary>
    /// Возвращает одну из "опорных" точек куба, по которым будут считаться барицентрические координаты.          
    /// считается для точки, которая находится в кубе, для которого опорной точкой (т.е. левой нижней)
    /// является точка (0, 0). 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="idxs"></param>
    /// <param name="n"></param>
    /// <param name="size">Длина одной из сторон куба</param>
    /// <returns></returns>
    private Point GeneratePoint(Point start, KeyValuePair<int, double>[] idxs, int n)
    {
#if DEBUG
      if (idxs == null) throw new ArgumentNullException("idxs");
#endif
      int dim = idxs.Length, k;
      var ans = start.Copy();

      for (int i = n; i < dim; i++)
      {
        k = idxs[i].Key;
        ans[k] += sizeOfBox[k];
      }
      return ans;
    }

    /// <summary>
    /// Функция получает барицентрические координаты новой точки, вычисляет значение функции цены
    /// </summary>
    /// <param name="newPoint"></param>
    /// <param name="valuesAtPoints"></param>
    /// <returns></returns>
    public double GetValueAtPoint(Point newPoint, int thisIdx, out bool supportPointExist)
    {
      double valueAtPoint = 0;
      // находим левую нижнюю вершину куба  
      Point start;
      //bool hasValue;
      Value val = new Value(false, 1.0);
      supportPointExist = FindSupportPoint(newPoint, out start);

      if (supportPointExist)
      {
        KeyValuePair<Point, double>[] coef = GetBarycentricCoordinates(newPoint, start);
        foreach (KeyValuePair<Point, double> pair in coef)
        {
            //Так делать нельзя, потому что если pair.Key не принадлежит решетке, то val будет равно 0
            //points[thisIdx].TryGetValue(pair.Key, out val);
            //valueAtPoint += pair.Value * val.value;
          
            if (points[thisIdx].TryGetValue(pair.Key, out val))
                valueAtPoint += pair.Value * val.value;
            else 
                valueAtPoint += pair.Value; // если барицентрическая координата не принадлежит решетке, то полагаем значение ф цены = 1
        }

        return valueAtPoint;
      }
      return 0;
    }


    #region Методы, улучшающие решетку

    /// <summary>
    /// Метод строит базисные векторы
    /// </summary>
    public void FillBasis()
    {
      _basis = new Point[dim];
      for (int i = 0; i < dim; i++)
      {
        double[] coords = new double[dim];
        coords[i] = sizeOfBox[i];
        _basis[i] = new Point(coords);
      }
    }

    /// <summary>
    /// Метод строит множество граней по первоначальному состоянию решетки
    /// Использовать сразу после построения решетки
    /// </summary>
    /// <returns></returns>
    public void FillInitialFacesSet()
    {
      faces = new SortedSet<Face>(_faceComparer);
      foreach (Point point in points[0].Keys)
        for (int i = 0; i < dim; i++)
        {
          Value value;
          points[0].TryGetValue(point + _basis[i], out value);
          //if (!value.isSupportPoint)
          if (!points[0].ContainsKey(point + _basis[i]))
            faces.Add(new Face(point.Copy(), i, 1));
          else
          {
            points[0].TryGetValue(point - _basis[i], out value);
            //if (!value.isSupportPoint)
            if (!points[0].ContainsKey(point - _basis[i]))
              faces.Add(new Face(point.Copy(), i, -1));
          }
        }

      foreach (var f in faces)
        Console.WriteLine("Point: {0}, vector: {1}, dir: {2}", f.x, f.k, f.direction);
    }

    /// <summary>
    /// Метод добавляет узлы к решетке так, чтобы множество Q стало стабильным за первого игрока
    /// </summary>
    public void UpdateGrid()
    {

    }

    /// <summary>
    /// Метод добавляет точку к решетке (к )
    /// </summary>
    public void AddPointToGrid(Point point)
    {

    }

    public bool NeedToAddCube(Point func, Point point)
    {
      return func * point > 0;
    }

    #endregion

  }
}