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

        public Player ChooseStartingPlayer(List<Player> players)
        {
            Player startingPlayer = players.Find(player => Equals(player.WonLastGame, true));
            int newStarterIndex = players.FindIndex(player => Equals(player.WonLastGame, true));
            
            // The first game
            if (startingPlayer == null)
            {
                // ToDo: Tie to a die function
                startingPlayer = players[_random.Next(_players.Count - 1)];
            }
            else
            {
                // Only shift player if the winner wasn't East
                if (startingPlayer.Wind != Constants.Winds.East)
                {
                    // Update old player activity
                    startingPlayer.IsActive = false;
                    
                    if (newStarterIndex == Constants.PlayerCount - 1)
                    {
                        newStarterIndex = 0;
                    }
                    else
                    {
                        newStarterIndex += 1;
                    }

                    startingPlayer = players[newStarterIndex];
                }
            }

            // Update new player activity
            startingPlayer.IsActive = true;

            return startingPlayer;
        }

        public List<Player> SetUpPlayers()
        {
            _players = new List<Player>();

            // Set up the AI players
            for (var i = 0; i < Constants.PlayerCount; i++)
            {
                Player player = AddPlayer();
                DrawStartingHand(_deck, player);
                _players.Add(player);
            }

            // Set up the human player
            _players[0].IsHuman = true;

            var startingPlayer = ChooseStartingPlayer(_players);
            int playerIndex = _players.FindIndex(player => player == startingPlayer);
            
            // Assign winds
            var winds = new List<string>
                {
                    Constants.Winds.North,
                    Constants.Winds.East,
                    Constants.Winds.South,
                    Constants.Winds.West
                };

            // Start by giving out the East wind
            var windIndex = 1;

            for (var i = 0; i < Constants.PlayerCount; i++)
            {
                if (playerIndex == Constants.PlayerCount - 1)
                {
                    playerIndex = 0;
                }
                else
                {
                    playerIndex += 1;
                }

                if (windIndex == winds.Count - 1)
                {
                    windIndex = 0;
                }
                else
                {
                    windIndex += 1;
                }

                _players[playerIndex].Wind = winds[windIndex];
            }

            return _players;
        }

        public Deck BuildDeck()
        {
            _deck = new Deck();
            _log.Info("Building the deck");

            var tileDuplicateCount = 4;
            var suits = new List<string> { "bamboo", "circles", "characters" };
            var winds = new List<string>
                {
                    Constants.Winds.North,
                    Constants.Winds.East,
                    Constants.Winds.West,
                    Constants.Winds.South
                };
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

        public Player GetActivePlayer()
        {
            for (var i = 0; i < Constants.PlayerCount; i++)
            {
                if (_players[i].IsActive)
                {
                    return _players[i];
                }
            }

            throw new Exception("No active player could be found");
        }

        public List<Player> SwapPlayer()
        {
            for (var i = 0; i < Constants.PlayerCount; i++) {
                if (_players[i].IsActive)
                {
                    _players[i].IsActive = false;
                    if (i + 1 == Constants.PlayerCount)
                    {
                        _players[0].IsActive = true;
                        break;
                    }
                    else
                    {
                        _players[i + 1].IsActive = true;
                        break;
                    }
                }  
            }

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
                selectedTile = _discardPile.Tiles[selectedTileIndex];
                _discardPile.Tiles.RemoveAt(selectedTileIndex);
                activePlayer.Hand.Add(selectedTile); // The player must take the tile in this logic path
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

        // Play the game
        public void PlayGame(Game _game)
        {
            // ToDo: Handle turns, AI and game finishing
            
            //while (!_game.Finished)
            //{
            //    foreach (Player _player in _players)
            //    {
            //        if (_player.IsActive)
            //        {
            //            if (_player.IsHuman)
            //            {
            //                // User input
            //            }
            //            else
            //            {
            //                // Bot logic
            //            }
            //        }
            //    }
            //}

            // ToDo: Tie this to score
            if (_game.Finished)
            {
                _players[0].WonLastGame = true;
            }
        }

        public static bool CalculateChi(Player player)
        {
            var orderedTiles = player.Hand.Tiles.OrderBy(tile => tile.Number);
            int count = 1; // We always have at least one tile
            Tile firstTile = player.Hand.Tiles.First();
            int firstNumber = 0;
            List<Tile> validTiles = new List<Tile>
            {
                firstTile
            };
            foreach (Tile tile in orderedTiles)
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
                    validTiles.Add(tile);
                }
                // End of one sequence, start of another
                else
                {
                    if (count >= 3)
                    {
                        //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                        player.Hand.Chis.Add(new TileSet(validTiles));
                        return true;
                    }
                    count = 1;
                    firstNumber = tile.Number;
                }
            }
            if (count >= 3)
            {
                //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                player.Hand.Chis.Add(new TileSet(validTiles));
                return true;
            }
            validTiles = new List<Tile>();

            return false;
        }

        public static bool CalculatePong(Player player)
        {
            var orderedTiles = player.Hand.Tiles.OrderBy(tile => tile.Number);
            int count = 0;
            Tile firstTile = player.Hand.Tiles.First();
            List<Tile> validTiles = new List<Tile>();
            foreach (var tile in orderedTiles)
            {
                // New value contributes to sequence
                // Note sets are only scored when all tiles are of the same suit and are the same number
                if (tile.Number == firstTile.Number && tile.Suit == firstTile.Suit)
                {
                    count++;
                    validTiles.Add(tile);
                }
                // End of one sequence, start of another
                else
                {
                    if (count >= 3)
                    {
                        //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                        player.Hand.Pongs.Add(new TileSet(validTiles));
                        return true;
                    }
                    count = 0;
                }
            }
            if (count >= 3)
            {
                //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                player.Hand.Pongs.Add(new TileSet(validTiles));
                return true;
            }
            validTiles = new List<Tile>();

            return false;
        }

        public static bool CalculateKang(Player player)
        {
            var orderedTiles = player.Hand.Tiles.OrderBy(tile => tile.Number);
            int count = 0;
            Tile firstTile = player.Hand.Tiles.First();
            List<Tile> validTiles = new List<Tile>();
            foreach (var tile in orderedTiles)
            {
                // New value contributes to sequence
                // Note sets are only scored when all tiles are of the same suit and are the same number
                if (tile.Number == firstTile.Number && tile.Suit == firstTile.Suit)
                {
                    count++;
                    validTiles.Add(tile);
                }
                // End of one sequence, start of another
                else
                {
                    if (count >= 4)
                    {
                        //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                        player.Hand.Kangs.Add(new TileSet(validTiles));
                        return true;
                    }
                    count = 0;
                }
            }
            if (count >= 4)
            {
                //_log.Info($"Found sequence of length {count}, starting at {firstNumber}");
                player.Hand.Kangs.Add(new TileSet(validTiles));
                return true;
            }
            validTiles = new List<Tile>();

            return false;
        }

        public static SpecialHand CalculateSpecialHand(Player player)
        {
            // Create all special hands
            List<SpecialHand> specialHands = new List<SpecialHand>
            {
                new SpecialHand
                {
                    Name = "Bamboo Forest",
                    Score = 50,
                    Tiles = new List<Tile>
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "bamboo"),
                        new Tile(3, "bamboo"),
                        new Tile(4, "bamboo"),
                        new Tile(5, "bamboo"),
                        new Tile(6, "bamboo"),
                        new Tile(7, "bamboo"),
                        new Tile(8, "bamboo"),
                        new Tile(9, "bamboo")
                    }
                }
            };

            foreach (SpecialHand specialHand in specialHands)
            {
                if (player.Hand.Tiles.ContainsSubsequence(specialHand.Tiles))
                {
                    return specialHand;
                }
            }

            return null;
        }

        public static int CalculateScore(Player player)
        {
            int score = 0;

            // Check for special hands first, as they override the score
            SpecialHand specialHand = CalculateSpecialHand(player);
            if (specialHand != null)
            {
                return specialHand.Score;
            }

            // Check pretties
            foreach (Tile tile in player.Hand)
            {
                if (tile.Suit == "pretty")
                {
                    score += 4;
                }
            }

            // Check pairs
            foreach (TileSet pair in player.Hand.Pairs)
            {
                if (!pair.Declared)
                {
                    if (pair.All(tile => tile.Suit == "dragon")) {
                        score += 2;
                    }
                    if (pair.All(tile => tile.Suit == "wind" &&
                        (tile.SpecialName == Constants.Winds.East || tile.SpecialName == player.Wind)
                       ))
                    {
                        score += 2;
                    }
                }
            }

            // Check chis - non-scoring outside special hands

            // Check pongs
            foreach (TileSet pong in player.Hand.Pongs)
            {
                if (pong.Declared)
                {
                    score += 2;
                }
                else
                {
                    score += 4;
                }
            }

            // Check kangs
            foreach (TileSet kang in player.Hand.Kangs)
            {
                if (kang.Declared)
                {
                    score += 8;
                }
                else
                {
                    score += 16;
                }
            }

            // Check doubles (after previous 'summed' score has been calculated)

            // Pongs
            foreach (TileSet pong in player.Hand.Pongs)
            {
                if (!pong.Declared)
                {
                    if (pong.All(tile => tile.Suit == "dragon"))
                    {
                        score *= 2;
                    }
                    if (pong.All(tile => tile.Suit == "wind" &&
                        (tile.SpecialName == Constants.Winds.East || tile.SpecialName == player.Wind)
                       ))
                    {
                        score *= 2;
                    }
                }
            }

            // Kangs
            foreach (TileSet kang in player.Hand.Kangs)
            {
                if (!kang.Declared)
                {
                    if (kang.All(tile => tile.Suit == "dragon"))
                    {
                        score *= 2;
                    }
                    if (kang.All(tile => tile.Suit == "wind" &&
                        (tile.SpecialName == Constants.Winds.East || tile.SpecialName == player.Wind)
                       ))
                    {
                        score *= 2;
                    }
                }
            }

            // Pretty collection
            if (player.Hand.All(tile => tile.Suit == "pretty" &&
                (tile.SpecialName == "flowers" || tile.SpecialName == "seasons")
                ))
            {
                score *= 2;
            }

            // 1 to 9
            if (player.Hand.OrderBy(a => a)
                .Zip(player.Hand.Skip(1), (a, b) => (a.Number + 1) == b.Number)
                .All(x => x)
                )
            {
                score *= 2;
            }

            player.Score += score;

            return score;
        }
    }
}