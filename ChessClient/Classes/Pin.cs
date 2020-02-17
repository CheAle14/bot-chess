using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    [Flags]
    public enum Pin
    {
        // can be ORed with the PlayerSide enum for rightmost two flags.
        None =        0b0000,
        Horizontal =  0b0001,
        Vertical =    0b0010,
        LeftDiagonal= 0b0100,
        RightDiagonal=0b1000,
    }
}
