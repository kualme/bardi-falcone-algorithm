using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.TerminalSet
{
    /// <summary>
    /// Терминальное множество для задачи TAD
    /// </summary>
    class TadTerminalSet : ITerminalSet
    {
        /// <summary>
        /// Ограничение на размер второй координаты
        /// </summary>
        private double rc;

        /// <summary>
        /// Конструктор терминального множества для задачи TAD
        /// </summary>        
        public TadTerminalSet(double rc)
        {
            this.rc = rc;
        }

        /// <summary>
        /// Возвращает true, если попадаем в терминальное множество - шар радиуса rc вокруг второй координаты точки
        /// </summary>
        /// <param name="p">Точка</param>
        /// <returns></returns>
        public bool BelongsToTerminalSet(Point p)
        {
            if (-rc <= p[1] && p[1] <= rc)
                return true;
            return false;
        }
    }
}
