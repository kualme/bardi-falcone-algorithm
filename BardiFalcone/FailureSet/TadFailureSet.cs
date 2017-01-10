using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.FailureSet
{
    /// <summary>
    /// При попадании системы в это множество первый игрок (Attacker) проигрывает
    /// </summary>
    class TadFailureSet : IFailureSet
    {
        /// <summary>
        /// Ограничение на размер первой координаты
        /// </summary>
        private double Rc;

        /// <summary>
        /// Конструктор множества, в котором первый игрок (Attacker) проигрывает
        /// </summary>
        /// <param name="Rc">Ограничение на размер первой координаты</param>
        public TadFailureSet(double Rc)
        {
            this.Rc = Rc;
        }

        public bool Belongs(Point p)
        {
            if (-Rc <= p[0] && p[0] <= Rc)
                return true;
            return false;
        }
    }
}
