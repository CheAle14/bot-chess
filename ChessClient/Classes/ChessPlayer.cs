﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class ChessPlayer : APIObject<int>
    {
        public string Name { get; set; }
        public PlayerSide Side { get; set; }
    }
}
