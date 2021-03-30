using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mahjong
{
    public class Tile
    {
        public Tile(int number, string suit, [Optional] string specialName)
        {
            Number = number;
            Suit = suit;
            SpecialName = specialName;
        }

        public int Number { get; set; }
        public string Suit { get; set; }
        public string SpecialName { get; set; }
    }

    public class TileSet : List<Tile>
    {
        public TileSet(List<Tile> tiles, [Optional] bool declared)
        {
            Tiles = tiles;
            Declared = declared;
        }
        
        public bool Declared { get; set; }
        public List<Tile> Tiles { get; set; }
    }
}