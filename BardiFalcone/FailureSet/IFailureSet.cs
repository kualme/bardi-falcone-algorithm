using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.FailureSet
{
    /// <summary>
    /// При попадании в это множество игрок проигрывает
    /// </summary>
    interface IFailureSet
    {
        bool Belongs(Point p);
    }
}
