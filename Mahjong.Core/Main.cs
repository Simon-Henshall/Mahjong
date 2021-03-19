﻿using log4net;
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
        private List<Player> _players;

        // ToDo: Shift this logic
        // Game variables
        private const int _playerCount = 4;

        public MainLogic()
        {
            _log = LogManager.GetLogger("mahjong");
            _random = new Random();
            _tileList = new List<Tile>();
            _discardPile = new DiscardPile();
            _deck = BuildDeck();
            _players = SetUpPlayers();
        }

        public Player AddPlayer()
        {
            return new Player();
        }

        public List<Player> SetUpPlayers()
        {
            _players = new List<Player>();

            // Set up the AI players
            for (var i = 0; i < _playerCount; i++)
            {
                Player player = AddPlayer();
                DrawStartingHand(_deck, player);
                _players.Add(player);
            }

            // Set up the human player
            _players[0].IsHuman = true;

            // ToDo: Rotate this for subsequent games
            // ToDo: Tie to a die function
            // Assign winds
            var winds = new List<string> { "east", "south", "west", "north" };
            var randomIndex = _random.Next(winds.Count - 1);
            for (var i = 0; i < _playerCount; i++)
            {
                if (randomIndex == winds.Count - 1)
                {
                    randomIndex = 0;
                }
                else
                {
                    randomIndex += 1;
                }

                _players[i].Wind = winds[randomIndex];
            }

            // Set up the active player
            for (var i = 0; i < _playerCount; i++)
            {
                // The East player will be active at the start of the game
                if (_players[i].Wind == "east")
                {
                    _players[i].IsActive = true;
                }
            }

            return _players;
        }

        public Deck BuildDeck()
        {
            _deck = new Deck();
            _log.Info("Building the deck");

            var tileDuplicateCount = 4;
            var suits = new List<string> { "bamboo", "circles", "characters" };
            var winds = new List<string> { "east", "south", "west", "north" };
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

        public Deck GetDeck()
        {
            return _deck;
        }
        
        public DiscardPile GetDiscardPile()
        {
            return _discardPile;
        }

        public List<Player> GetPlayers()
        {
            return _players;
        }

        public Tile DrawTile(Deck deck, Player activePlayer, [Optional] string location, [Optional] bool playerWantsTile)
        {
            Tile selectedTile;
            _log.Debug($"Player {activePlayer} is drawing a tile");
            if (location == "discardPile")
            {
                _log.Debug($"(from the discard pile)");
                int selectedTileIndex = _random.Next(_discardPile.Tiles.Count - 1);
                _discardPile.Tiles.RemoveAt(selectedTileIndex);
                selectedTile = _discardPile.Tiles[selectedTileIndex];
            }
            else
            {
                _log.Debug($"(from the wall)");
                // ToDo: This shouldn't be randomised -- but does it really matter?
                int selectedTileIndex = _random.Next(deck.Tiles.Count - 1);
                selectedTile = deck.Tiles[selectedTileIndex];

                // Remove from the deck
                deck.Tiles.RemoveAt(selectedTileIndex);

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
            }

            return selectedTile;
        }

        // Set up the starting hand for each player
        public Hand DrawStartingHand(Deck deck, Player player)
        {
            var hand = new Hand(new List<Tile>());
            for (var i = 0; i < 13; i++)
            {
                hand.Add(DrawTile(deck, player, "wall", true));
            }
            return hand;
        }

        // Set up the entire game
        public Game SetupFullGame(List<Player> _players, Deck _deck, DiscardPile _discardPile)
        {
            var game = new Game(_players, _deck, _discardPile);
            return game;
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

        // Called with only four tiles to check whether or not they form a valid kang
        public static bool CalculateKang(Hand hand) // hand.Tiles.Count = 4
        {
            // Sets are only scored when they're all of the same suit
            var firstTile = hand.Tiles.First();
            return hand.Tiles.All(tile => tile.Suit == firstTile.Suit && tile.Number == firstTile.Number);
        }
    }
}