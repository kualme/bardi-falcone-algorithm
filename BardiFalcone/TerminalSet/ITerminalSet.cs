using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BardiFalcone.TerminalSet
{
    interface ITerminalSet
    {
        bool BelongsToTerminalSet(Point p);
    }            
}
