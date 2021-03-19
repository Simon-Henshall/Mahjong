using System.Collections.Generic;

namespace Mahjong
{
    public class Hand : List<Tile>
    {
        public List<Tile> Tiles = new List<Tile>();

        public Hand(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                Tiles.Add(tile);
            }
        }
    }
}