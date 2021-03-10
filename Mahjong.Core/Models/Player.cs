using System.Collections.Generic;

namespace Mahjong
{
    public class Player
    {
        public bool IsActive { get; set; } = false;

        public Hand Hand { get; set; } = new Hand(new List<Tile>());
    }
}
