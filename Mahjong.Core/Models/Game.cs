using System.Collections.Generic;

namespace Mahjong
{
    public class Game
    {
        public List<Player> Players = new List<Player>();
        public Deck Deck = new Deck();
        public DiscardPile DiscardPile = new DiscardPile();

        public Game(List<Player> players, Deck _deck, DiscardPile _discardPile)
        {
            foreach (Player player in players)
            {
                Players.Add(player);
            }

            Deck = _deck;
            DiscardPile = _discardPile;
        }
    }
}
