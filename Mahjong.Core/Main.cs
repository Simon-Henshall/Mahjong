using log4net;
using Mahjong.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Hand DrawStartingHand(Deck deck, Player player)
        {
            var hand = new Hand(new List<Tile>());
            for (var i = 0; i < 13; i++)
            {
                hand.Add(DrawTile(deck, player, true));
            }
            return hand;
        }

        // Called with only three tiles to check whether or not they form a valid chi
        public static bool CalculateChi(Hand hand) // hand.Tiles.Count = 3
        {

            var orderedTiles = hand.Tiles.OrderBy(tile => tile.Number);
            int count = 1; // We always have at least one tile
            Tile firstTile = hand.Tiles.First();
            int firstNumber = 0;
            foreach (var tile in orderedTiles)
            {
                // Skip duplicate values
                if (tile.Number == firstNumber + count - 1)
                {
                    // No need to do anything
                }
                // New value contributes to sequence
                // Note runs are only scored when all tiles are of the same suit
                else if (tile.Number == firstTile.Number + count && tile.Suit == firstTile.Suit)
                {
                    count++;
                }
                // End of one sequence, start of another
                else
                {
                    if (count >= 3)
                    {
                        //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                        return true;
                    }
                    count = 1;
                    firstNumber = tile.Number;
                }
            }
            if (count >= 3)
            {
                //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                return true;
            }

            return false;
        }

        // Called with only three tiles to check whether or not they form a valid pong
        public static bool CalculatePong(Hand hand) // hand.Tiles.Count = 3
        {
            // Sets are only scored when they're all of the same suit
            var firstTile = hand.Tiles.First();
            return hand.Tiles.All(tile => tile.Suit == firstTile.Suit && tile.Number == firstTile.Number);
        }

        // Called with only four tiles to check whether or not they form a valid pong
        public static bool CalculateKang(Hand hand) // hand.Tiles.Count = 4
        {
            // Sets are only scored when they're all of the same suit
            var firstTile = hand.Tiles.First();
            return hand.Tiles.All(tile => tile.Suit == firstTile.Suit && tile.Number == firstTile.Number);
        }
    }
}