using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BardiFalcone
{
  /// <summary>
  /// Структура, содержащая значение функции цены и флаг, 
  /// определяющий, принадлежит ли точка терминальному множеству
  /// </summary>
  [Serializable]
  public struct Value
  {
    /// <summary>
    /// Определяет попадание точки в терминальное множество
    /// </summary>
    public bool isTerminalPoint;

    /// <summary>
    /// Значение функции цены в точке
    /// </summary>
    public double value;

    public Value(double value)
    {
      this.isTerminalPoint = false;
      this.value = value;
    }

    public Value(bool isTerminalPoint, double value)
    {
      this.isTerminalPoint = isTerminalPoint;
      this.value = value;
    }

    public Value(bool isTerminalPoint, double value, bool isSupportPoint)
    {
      this.isTerminalPoint = isTerminalPoint;
      this.value = value;
    }
  }
}
