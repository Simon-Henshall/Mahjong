using log4net;
using Mahjong.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mahjong
{
    public class MainLogic
    {
        private readonly ILog _log;
        private Random _random;
        private List<Tile> _tileList;
        private Deck _deck;
        private DiscardPile _discardPile;

        public MainLogic()
        {
            _log = LogManager.GetLogger("mahjong");
            _random = new Random();
            _tileList = new List<Tile>();
            _deck = new Deck();
            _discardPile = new DiscardPile();
        }

        public Player AddPlayer()
        {
            return new Player();
        }
        
        public Deck BuildDeck()
        {
            _log.Info("Building the deck");

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
                        _tileList.Add(new Tile(i, suit));
                    }
                }

                // Handle winds
                foreach (string wind in winds)
                {
                    _tileList.Add(new Tile(0, "wind", wind));
                }

                // Handle dragons
                foreach (string dragon in dragons)
                {
                    _tileList.Add(new Tile(0, "dragon", dragon));
                }
            }

            // Handle pretties
            for (int i = 1; i < 5; i++)
            {
                foreach (string pretty in pretties)
                {
                    _tileList.Add(new Tile(i, "pretty", pretty));
                }
            }

            _tileList.Shuffle(); 
            _deck.Tiles = _tileList;

            return _deck;
        }

        public Tile DrawTile(Deck deck, Player activePlayer, [Optional] bool playerWantsTile)
        {
            _log.Debug($"Player {activePlayer} is drawing a tile");
            int selectedTileIndex = _random.Next(deck.Tiles.Count);
            Tile selectedTile = deck.Tiles[selectedTileIndex];

            // Remove from the deck
            deck.Tiles.RemoveAt(selectedTileIndex);

            // ToDo: Randomise whether or not the player wants the tile (for now)
            // Default parameters must be compile-time constants so we cannot set it as a function parameter
            if (!playerWantsTile)
            {
                playerWantsTile = _random.Next(100) < 50;
            }

            // Check whether or not the player wants the tile
            if (playerWantsTile)
            {
                // Add to the player's hand
                activePlayer.Hand.Add(selectedTile);
            }
            else
            {
                _discardPile.Tiles.Add(selectedTile);
            }

            return deck.Tiles[selectedTileIndex];
        }

        // Set up the starting hand for each player
        public Hand DrawStartingHand(Deck deck, Player activePlayer)
        {
            var hand = new Hand();
            return hand;
        }

        // Called with only three tiles to check whether or not they form a valid chi
        public static bool CalculateChi(Hand hand) // Tile Count = 3
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
        public static bool CalculatePong(Hand hand) // Tile Count = 3
        {
            // Handle regular suits
            if ((hand.Tile1.Suit == hand.Tile2.Suit) && (hand.Tile1.Suit == hand.Tile3.Suit))
            {
                if ((hand.Tile2.Number == hand.Tile1.Number) && (hand.Tile3.Number == hand.Tile1.Number))
                {
                    return true;
                }
            }
            // Handle winds and dragons
            else if ((hand.Tile1.SpecialName != null && hand.Tile2.SpecialName != null && hand.Tile3.SpecialName != null) &&
                (hand.Tile1.SpecialName == hand.Tile2.SpecialName) && (hand.Tile1.SpecialName == hand.Tile3.SpecialName))
            {
                return true;
            }

            return false;
        }

        // Called with only four tiles to check whether or not they form a valid pong
        public static bool CalculateKang(Hand hand) // Tile Count = 4
        {
            // Handle regular suits
            if ((hand.Tile1.Suit == hand.Tile2.Suit) && (hand.Tile1.Suit == hand.Tile3.Suit) && (hand.Tile1.Suit == hand.Tile4.Suit))
            {
                if ((hand.Tile2.Number == hand.Tile1.Number) && (hand.Tile3.Number == hand.Tile1.Number) && (hand.Tile4.Number == hand.Tile1.Number))
                {
                    return true;
                }
            }
            // Handle winds and dragons
            else if ((hand.Tile1.SpecialName != null && hand.Tile2.SpecialName != null && hand.Tile3.SpecialName != null && hand.Tile4.SpecialName != null) &&
                (hand.Tile1.SpecialName == hand.Tile2.SpecialName) && (hand.Tile1.SpecialName == hand.Tile3.SpecialName) && (hand.Tile1.SpecialName == hand.Tile4.SpecialName))
            {
                return true;
            }

            return false;
        }
    }
}