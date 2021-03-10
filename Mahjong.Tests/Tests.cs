using NUnit.Framework;
using System.Collections.Generic;

namespace Mahjong.Tests
{
    public class Tests
    {
        private MainLogic _mainLogic;
        private List<Tile> _tileList;
        private Deck _deck;
        private DiscardPile _discardPile;
        private Player _player;

        [SetUp]
        public void Setup()
        {
            _mainLogic = new MainLogic();
            _tileList = new List<Tile>();
            _deck = _mainLogic.BuildDeck();
            _discardPile = new DiscardPile();
            _player = _mainLogic.AddPlayer();
        }

        [Test]
        public void DrawTileAndKeep()
        {
            int theoreticalRemainingTileCount = _deck.Tiles.Count - 1;
            var pickUpResult = _mainLogic.DrawTile(_deck, _player, true);
            Assert.IsInstanceOf(typeof(Tile), pickUpResult);
            Assert.AreEqual(theoreticalRemainingTileCount, _deck.Tiles.Count);
            Assert.AreEqual(1, _player.Hand.Count);
        }

        public void DrawTileAndDiscard()
        {
            int theoreticalRemainingTileCount = _deck.Tiles.Count - 1;
            var discardResult = _mainLogic.DrawTile(_deck, _player, true);
            Assert.IsInstanceOf(typeof(Tile), discardResult);
            Assert.AreEqual(theoreticalRemainingTileCount, _deck.Tiles.Count);
            Assert.AreEqual(1, _discardPile.Tiles.Count);
        }

        [Test]
        public void HandGeneration()
        {
            var result = _mainLogic.DrawStartingHand(_deck, _player);
            Assert.IsInstanceOf(typeof(Hand), result);
            Assert.AreEqual(13, result.Count);
        }

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
            public void TestIsChi(Hand hand)
            {
                var result = MainLogic.CalculateChi(hand);
                Assert.IsTrue(result);
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
            public void TestIsNotChi(Hand hand)
            {
                var result = MainLogic.CalculateChi(hand);
                Assert.IsFalse(result);
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
            public void TestIsPong(Hand hand)
            {
                var result = MainLogic.CalculatePong(hand);
                Assert.IsTrue(result);
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
            public void TestIsNotPong(Hand hand)
            {
                var result = MainLogic.CalculatePong(hand);
                Assert.IsFalse(result);
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
            public void TestIsKang(Hand hand)
            {
                var result = MainLogic.CalculateKang(hand);
                Assert.IsTrue(result);
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
            public void TestIsNotKang(Hand hand)
            {
                var result = MainLogic.CalculateKang(hand);
                Assert.IsFalse(result);
            }
        }
    }
}