using log4net;
using Mahjong.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Mahjong
{
    public class MainLogic
    {
        private readonly ILog _log;

        public MainLogic()
        {
            _log = LogManager.GetLogger("mahjong");
        }

        public Player AddPlayer()
        {
            return new Player();
        }
        
        public Deck BuildDeck()
        {
            _log.Info("Building the deck");

            var list = new List<Tile>();
            var deck = new Deck();
            var tileDuplicateCount = 4;
            
            var suits = new List<string> { "bamboo", "circles", "characters" };
            var winds = new List<string> { "north", "south", "east", "west" };
            var dragons = new List<string> { "green", "red", "white" };
            var pretties = new List<string> { "flowers", "seasons" };

            for (int tdc = 0; tdc < tileDuplicateCount; tdc++)
            {
                // Handle regular tiles
                foreach (string suit in suits)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        list.Add(new Tile(i, suit));
                    }
                }

                // Handle winds
                foreach (string wind in winds)
                {
                    list.Add(new Tile(0, "wind", wind));
                }

                // Handle dragons
                foreach (string dragon in dragons)
                {
                    list.Add(new Tile(0, "dragon", dragon));
                }
            }

            // Handle pretties
            for (int i = 1; i < 5; i++)
            {
                foreach (string pretty in pretties)
                {
                    list.Add(new Tile(i, "pretty", pretty));
                }
            }

            list.Shuffle(); 
            deck.Tiles = list;

            return deck;
        }

        public Tile DrawTile(Deck deck, Player activePlayer)
        {
            _log.Debug($"Player {activePlayer} is drawing a tile");

            var random = new Random();
            int selectedTileIndex = random.Next(deck.Tiles.Count);
            Tile selectedTile = deck.Tiles[selectedTileIndex];

            // Remove from the deck
            deck.Tiles.RemoveAt(selectedTileIndex);

            // Add to the player's hand
            activePlayer.Hand.Add(selectedTile);

            return deck.Tiles[selectedTileIndex];
        }

        // Called with only three tiles to check whether or not they form a valid chi
        public static bool CalculateChi(Hand hand)
        {
            // Runs are only scored when they're all of the same suit
            if ((hand.Tile1.Suit == hand.Tile2.Suit) && (hand.Tile1.Suit == hand.Tile3.Suit))
            {
                if ((hand.Tile2.Number == hand.Tile1.Number + 1) && (hand.Tile3.Number == hand.Tile1.Number + 2))
                {
                    return true;
                }
            }

            return false;
        }

        // Called with only three tiles to check whether or not they form a valid pong
        public static bool CalculatePong(Hand hand)
        {
            // Runs are only scored when they're all of the same suit
            if ((hand.Tile1.Suit == hand.Tile2.Suit) && (hand.Tile1.Suit == hand.Tile3.Suit))
            {
                if ((hand.Tile2.Number == hand.Tile1.Number) && (hand.Tile3.Number == hand.Tile1.Number))
                {
                    return true;
                }
            }

            return false;
        }

        // Called with only four tiles to check whether or not they form a valid pong
        public static bool CalculateKang(Hand hand)
        {
            // Runs are only scored when they're all of the same suit
            if ((hand.Tile1.Suit == hand.Tile2.Suit) && (hand.Tile1.Suit == hand.Tile3.Suit) && (hand.Tile1.Suit == hand.Tile4.Suit))
            {
                if ((hand.Tile2.Number == hand.Tile1.Number) && (hand.Tile3.Number == hand.Tile1.Number) && (hand.Tile4.Number == hand.Tile1.Number))
                {
                    return true;
                }
            }

            return false;
        }
    }
}