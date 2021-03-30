using System.Collections.Generic;

namespace Mahjong
{
    public class Hand : List<Tile>
    {
        public Hand()
        {
        }
        
        public List<Tile> Tiles = new List<Tile>();

        public Hand(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                Tiles.Add(tile);
            }
        }

        public List<TileSet> Pairs = new List<TileSet>();

        public List<TileSet> Pongs = new List<TileSet>();

        public List<TileSet> Kangs = new List<TileSet>();

        public List<TileSet> Chis = new List<TileSet>();
    }
}