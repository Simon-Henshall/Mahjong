using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mahjong
{
    public class Hand: List<Tile>
    {
        public Tile Tile1 { get; set; }

        public Tile Tile2 { get; set; }

        public Tile Tile3 { get; set; }
        public Tile Tile4 { get; set; }
        public Tile Tile5 { get; set; }
        public Tile Tile6 { get; set; }
        public Tile Tile7 { get; set; }
        public Tile Tile8 { get; set; }
        public Tile Tile9 { get; set; }
        public Tile Tile10 { get; set; }
        public Tile Tile11 { get; set; }
        public Tile Tile12 { get; set; }
        public Tile Tile13 { get; set; }
        public Tile Tile14 { get; set; }

        public Hand([Optional] Tile tile1, [Optional] Tile tile2, [Optional] Tile tile3, [Optional] Tile tile4)
        {
            Tile1 = tile1;
            Tile2 = tile2;
            Tile3 = tile3;
            Tile4 = tile4;
        }
    }
}