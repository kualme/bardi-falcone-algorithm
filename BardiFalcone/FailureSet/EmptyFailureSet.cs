using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.FailureSet
{
    class EmptyFailureSet : IFailureSet
    {
        public bool Belongs(Point p)
        {
            return false;
        }
    }
}
