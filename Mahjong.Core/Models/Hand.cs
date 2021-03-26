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

        public List<List<Tile>> Pairs = new List<List<Tile>>();

        public List<List<Tile>> Pongs = new List<List<Tile>>();

        public List<List<Tile>> Kangs = new List<List<Tile>>();

        public List<List<Tile>> Chis = new List<List<Tile>>();
    }
}