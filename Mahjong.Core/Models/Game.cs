using System.Collections.Generic;

namespace Mahjong
{
    public class Game
    {
        public List<SpecialHand> SpecialHands { get; set; } = new List<SpecialHand>(); 
        public List<Player> Players { get; set; } = new List<Player>();
        public Deck Deck { get; set; } = new Deck();
        public DiscardPile DiscardPile { get; set; } = new DiscardPile();
        public bool Finished { get; set; } = false;

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
