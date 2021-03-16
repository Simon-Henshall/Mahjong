﻿using System.Collections.Generic;

namespace Mahjong
{
    public class Player
    {
        public bool IsActive { get; set; } = false;

        public bool IsHuman { get; set; } = false;

        public string Wind { get; set; }

        public Hand Hand { get; set; } = new Hand(new List<Tile>());
    }
}
