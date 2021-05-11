using Mahjong;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class BaseGameplayMechanics
    {
        private MainLogic _mainLogic;
        private Deck _deck;
        private DiscardPile _discardPile;
        private List<Player> _players;

        [SetUp]
        public void Setup()
        {
            _mainLogic = new MainLogic();
            _deck = _mainLogic.GetDeck();
            _discardPile = _mainLogic.GetDiscardPile();
            _players = _mainLogic.GetPlayers();
        }

        [Test]
        public void ChooseStartingPlayer() {
            Player player = _mainLogic.ChooseStartingPlayer(_players);
            Assert.AreEqual(true, player.IsActive);
            // We cannot realiably check for East wind here as the method will be called twice due to the invocation above
            // ChooseStartingPlayer() checks based on .WonLastGame, which will not be set yet, so .IsActive will be set twice
        }

        [Test]
        public void GetActivePlayer()
        {
            Player player = _mainLogic.GetActivePlayer();
            Assert.AreEqual(true, player.IsActive);
            Assert.AreEqual(Constants.Winds.East, player.Wind);
        }

        [Test]
        public void SwapPlayer()
        {
            _mainLogic.SwapPlayer();
            for (var i = 0; i < Constants.PlayerCount; i++)
            {
                // The East player will be active at the start of the game
                if (_players[i].Wind == Constants.Winds.East)
                {
                    Assert.AreEqual(false, _players[i].IsActive);
                    if (i + 1 == Constants.PlayerCount)
                    {
                        Assert.AreEqual(true, _players[0].IsActive);
                    }
                    else
                    {
                        Assert.AreEqual(true, _players[i + 1].IsActive);
                    }
                }
            }

        }

        [Test]
        public void DrawTileAndKeep()
        {
            int theoreticalRemainingTileCount = _deck.Tiles.Count - 1;
            var pickUpResult = _mainLogic.DrawTile(_deck, _players[0], "wall", true);
            Assert.IsInstanceOf(typeof(Tile), pickUpResult);
            Assert.AreEqual(theoreticalRemainingTileCount, _deck.Tiles.Count);
            Assert.AreEqual(14, _players[0].Hand.Count);
        }

        [Test]
        public void DrawTileAndDiscard()
        {
            int theoreticalRemainingTileCount = _deck.Tiles.Count - 1;
            var discardResult = _mainLogic.DrawTile(_deck, _players[0], "wall", false);
            Assert.IsInstanceOf(typeof(Tile), discardResult);
            Assert.AreEqual(theoreticalRemainingTileCount, _deck.Tiles.Count);
            Assert.AreEqual(1, _discardPile.Tiles.Count);
        }

        [Test]
        public void PickUpDiscard()
        {
            Player _player1 = _mainLogic.GetActivePlayer();
            _mainLogic.DrawTile(_deck, _player1, "wall", false); // Get a tile into the discard pile
            Assert.AreEqual(1, _discardPile.Tiles.Count);
            _mainLogic.SwapPlayer();
            Player _player2 = _mainLogic.GetActivePlayer();
            int theoreticalPlayerTwoTileCount = _player2.Hand.Count + 1;
            _mainLogic.DrawTile(_deck, _mainLogic.GetActivePlayer(), "discardPile");
            Assert.AreEqual(theoreticalPlayerTwoTileCount, _player2.Hand.Count);
            Assert.AreEqual(0, _discardPile.Tiles.Count);
        }

        [Test]
        public void HandGeneration()
        {
            var result = _mainLogic.DrawStartingHand(_deck, _players[0]);
            Assert.IsInstanceOf(typeof(Hand), result);
            Assert.AreEqual(13, result.Count);
        }

        [Test]
        public void FullNewGameGeneration()
        {
            Game game = _mainLogic.SetupFullGame(_players, _deck, _discardPile);
            Assert.IsInstanceOf(typeof(Game), game);
            // Check that the 144 tiles have been distributed as 13 tiles to each player
            Assert.AreEqual(144 - (13 * _players.Count), game.Deck.Tiles.Count);
            for (var i = 0; i < _players.Count; i++)
            {
                Assert.AreEqual(13, game.Players[i].Hand.Count);
                Assert.IsNotNull(game.Players[i].Wind);
                Assert.IsNotNull(game.Players[i].IsActive);
                Assert.IsNotNull(game.Players[i].IsHuman);
            }
            Assert.AreEqual(Constants.PlayerCount, game.Players.Count);
            Assert.AreEqual(true, game.Players[0].IsHuman);
            // Set up the active player
            for (var i = 0; i < _players.Count; i++)
            {
                // The East player will be active at the start of the game
                if (_players[i].Wind == Constants.Winds.East)
                {
                    Assert.AreEqual(true, game.Players[i].IsActive);
                }
                else
                {
                    Assert.AreEqual(false, game.Players[i].IsActive);
                }
            }
        }
    }

    public class ComboTiles
    {
        [TestFixture]
        public class ChiCalcuation
        {
            private static readonly List<Hand> _GoodChiTestData = new List<Hand>()
            {
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "bamboo"),
                        new Tile(3, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(4, "bamboo"),
                        new Tile(5, "bamboo"),
                        new Tile(6, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(7, "bamboo"),
                        new Tile(8, "bamboo"),
                        new Tile(9, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(2, "characters"),
                        new Tile(3, "characters"),
                        new Tile(4, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(5, "characters"),
                        new Tile(6, "characters"),
                        new Tile(7, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(3, "circles"),
                        new Tile(4, "circles"),
                        new Tile(5, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(6, "circles"),
                        new Tile(7, "circles"),
                        new Tile(8, "circles")
                    }
                )
            };

            [TestCaseSource(nameof(_GoodChiTestData))]
            public void TestHasChi(Hand testHand)
            {
                var player = new Player
                {
                    Hand = testHand
                };
                var result = MainLogic.CalculateChi(player);
                Assert.IsTrue(result);
                Assert.AreEqual(1, player.Hand.Chis.Count);
                Assert.AreEqual(3, player.Hand.Chis[0].Tiles.Count);
                Assert.AreEqual(testHand.Tiles, player.Hand.Chis[0].Tiles);
            }

            private static readonly List<Hand> _BadChiTestData = new List<Hand>()
            {
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "characters"),
                        new Tile(1, "characters"),
                        new Tile(1, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "circles"),
                        new Tile(1, "circles"),
                        new Tile(1, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "characters"),
                        new Tile(3, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "bamboo"),
                        new Tile(3, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "characters"),
                        new Tile(3, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "characters"),
                        new Tile(3, "bamboo")
                    }
                ),
            };

            [TestCaseSource(nameof(_BadChiTestData))]
            public void TestHasNoChi(Hand testHand)
            {
                var player = new Player
                {
                    Hand = testHand
                };
                var result = MainLogic.CalculateChi(player);
                Assert.IsFalse(result);
                Assert.AreEqual(0, player.Hand.Chis.Count);
            }
        }

        [TestFixture]
        public class PongCalcuation
        {
            private static readonly List<Hand> _GoodPongTestData = new List<Hand>()
            {
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "characters"),
                        new Tile(1, "characters"),
                        new Tile(1, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "circles"),
                        new Tile(1, "circles"),
                        new Tile(1, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(0, "wind", "east"),
                        new Tile(0, "wind", "east"),
                        new Tile(0, "wind", "east")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(0, "dragon", "red"),
                        new Tile(0, "dragon", "red"),
                        new Tile(0, "dragon", "red")
                    }
                )
            };

            [TestCaseSource(nameof(_GoodPongTestData))]
            public void TestHasPong(Hand testHand)
            {
                var player = new Player
                {
                    Hand = testHand
                };
                var result = MainLogic.CalculatePong(player);
                Assert.IsTrue(result);
                Assert.AreEqual(1, player.Hand.Pongs.Count);
                Assert.AreEqual(3, player.Hand.Pongs[0].Tiles.Count);
                Assert.AreEqual(testHand.Tiles, player.Hand.Pongs[0].Tiles);
            }

            private static readonly List<Hand> _BadPongTestData = new List<Hand>()
            {
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "bamboo"),
                        new Tile(3, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(4, "bamboo"),
                        new Tile(5, "bamboo"),
                        new Tile(6, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(7, "bamboo"),
                        new Tile(8, "bamboo"),
                        new Tile(9, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(2, "characters"),
                        new Tile(3, "characters"),
                        new Tile(4, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(5, "characters"),
                        new Tile(6, "characters"),
                        new Tile(7, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(3, "circles"),
                        new Tile(4, "circles"),
                        new Tile(5, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(6, "circles"),
                        new Tile(7, "circles"),
                        new Tile(8, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "characters"),
                        new Tile(1, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "characters"),
                        new Tile(1, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "characters"),
                        new Tile(1, "bamboo")
                    }
                ),
            };

            [TestCaseSource(nameof(_BadPongTestData))]
            public void TestHasNoPong(Hand testHand)
            {
                var player = new Player
                {
                    Hand = testHand
                };
                var result = MainLogic.CalculatePong(player);
                Assert.IsFalse(result);
                Assert.AreEqual(0, player.Hand.Pongs.Count);
            }
        }

        [TestFixture]
        public class KangCalcuation
        {
            private static readonly List<Hand> _GoodKangTestData = new List<Hand>()
            {
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "characters"),
                        new Tile(1, "characters"),
                        new Tile(1, "characters"),
                        new Tile(1, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "circles"),
                        new Tile(1, "circles"),
                        new Tile(1, "circles"),
                        new Tile(1, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(0, "wind", "east"),
                        new Tile(0, "wind", "east"),
                        new Tile(0, "wind", "east"),
                        new Tile(0, "wind", "east")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(0, "dragon", "red"),
                        new Tile(0, "dragon", "red"),
                        new Tile(0, "dragon", "red"),
                        new Tile(0, "dragon", "red")
                    }
                )
            };

            [TestCaseSource(nameof(_GoodKangTestData))]
            public void TestHasKang(Hand testHand)
            {
                var player = new Player
                {
                    Hand = testHand
                };
                var result = MainLogic.CalculateKang(player);
                Assert.IsTrue(result);
                Assert.AreEqual(1, player.Hand.Kangs.Count);
                Assert.AreEqual(4, player.Hand.Kangs[0].Tiles.Count);
                Assert.AreEqual(testHand.Tiles, player.Hand.Kangs[0].Tiles);
            }

            private static readonly List<Hand> _BadKangTestData = new List<Hand>()
            {
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(2, "bamboo"),
                        new Tile(3, "bamboo"),
                        new Tile(4, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(5, "bamboo"),
                        new Tile(6, "bamboo"),
                        new Tile(7, "bamboo"),
                        new Tile(8, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(2, "characters"),
                        new Tile(3, "characters"),
                        new Tile(4, "characters"),
                        new Tile(5, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(6, "characters"),
                        new Tile(7, "characters"),
                        new Tile(8, "characters"),
                        new Tile(9, "characters")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(3, "circles"),
                        new Tile(4, "circles"),
                        new Tile(5, "circles"),
                        new Tile(6, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "circles")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "circles"),
                        new Tile(1, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "characters"),
                        new Tile(1, "characters"),
                        new Tile(1, "bamboo")
                    }
                ),
                new Hand(new List<Tile>()
                    {
                        new Tile(1, "bamboo"),
                        new Tile(1, "characters"),
                        new Tile(1, "bamboo"),
                        new Tile(1, "bamboo")
                    }
                ),
            };

            [TestCaseSource(nameof(_BadKangTestData))]
            public void TestHasNoKang(Hand testHand)
            {
                var player = new Player
                {
                    Hand = testHand
                };
                var result = MainLogic.CalculateKang(player);
                Assert.IsFalse(result);
                Assert.AreEqual(0, player.Hand.Kangs.Count);
            }
        }
    }
}