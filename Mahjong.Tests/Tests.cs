using NUnit.Framework;
using System.Collections.Generic;

namespace Mahjong.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DeckGeneration()
        {
            var result = MainLogic.BuildDeck();
            Assert.IsInstanceOf(typeof(Deck), result);
            Assert.AreEqual(144, result.Tiles.Count);
        }

        [Test]
        public void DrawTile()
        {
            Player player = MainLogic.AddPlayer();
            Deck deck = MainLogic.BuildDeck();
            int theoreticalRemainingTileCount = deck.Tiles.Count - 1;
            var result = MainLogic.DrawTile(deck, player);
            Assert.IsInstanceOf(typeof(Tile), result);
            Assert.AreEqual(theoreticalRemainingTileCount, deck.Tiles.Count);
        }
        
        //[Test]
        //public void HandGeneration()
        //{
        //    var result = MainLogic.DrawHand();

        //    Assert.IsInstanceOf(typeof(List<Tile>), result);
        //    Assert.AreEqual(13, result.Count);
        //}

        [TestFixture]
        public class ChiCalcuation
        {
            private static readonly object[] _GoodChiTestData =
            {
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(2, "bamboo"),
                    new Tile(3, "bamboo")
                ),
                new Hand(
                    new Tile(4, "bamboo"),
                    new Tile(5, "bamboo"),
                    new Tile(6, "bamboo")
                ),
                new Hand(
                    new Tile(7, "bamboo"),
                    new Tile(8, "bamboo"),
                    new Tile(9, "bamboo")
                ),
                new Hand(
                    new Tile(2, "characters"),
                    new Tile(3, "characters"),
                    new Tile(4, "characters")
                ),
                new Hand(
                    new Tile(5, "characters"),
                    new Tile(6, "characters"),
                    new Tile(7, "characters")
                ),
                new Hand(
                    new Tile(3, "circles"),
                    new Tile(4, "circles"),
                    new Tile(5, "circles")
                ),
                new Hand(
                    new Tile(6, "circles"),
                    new Tile(7, "circles"),
                    new Tile(8, "circles")
                )
            };

            [TestCaseSource(nameof(_GoodChiTestData))]
            public void TestIsChi(Hand hand)
            {
                var result = MainLogic.CalculateChi(hand);
                Assert.IsTrue(result);
            }

            private static readonly object[] _BadChiTestData =
            {
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo")
                ),
                new Hand(
                    new Tile(1, "characters"),
                    new Tile(1, "characters"),
                    new Tile(1, "characters")
                ),
                new Hand(
                    new Tile(1, "circles"),
                    new Tile(1, "circles"),
                    new Tile(1, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(2, "characters"),
                    new Tile(3, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(2, "bamboo"),
                    new Tile(3, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(2, "characters"),
                    new Tile(3, "characters")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(2, "characters"),
                    new Tile(3, "bamboo")
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
            private static readonly object[] _GoodPongTestData =
            {
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo")
                ),
                new Hand(
                    new Tile(1, "characters"),
                    new Tile(1, "characters"),
                    new Tile(1, "characters")
                ),
                new Hand(
                    new Tile(1, "circles"),
                    new Tile(1, "circles"),
                    new Tile(1, "circles")
                )
            };

            [TestCaseSource(nameof(_GoodPongTestData))]
            public void TestIsPong(Hand hand)
            {
                var result = MainLogic.CalculatePong(hand);
                Assert.IsTrue(result);
            }

            private static readonly object[] _BadPongTestData =
            {
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(2, "bamboo"),
                    new Tile(3, "bamboo")
                ),
                new Hand(
                    new Tile(4, "bamboo"),
                    new Tile(5, "bamboo"),
                    new Tile(6, "bamboo")
                ),
                new Hand(
                    new Tile(7, "bamboo"),
                    new Tile(8, "bamboo"),
                    new Tile(9, "bamboo")
                ),
                new Hand(
                    new Tile(2, "characters"),
                    new Tile(3, "characters"),
                    new Tile(4, "characters")
                ),
                new Hand(
                    new Tile(5, "characters"),
                    new Tile(6, "characters"),
                    new Tile(7, "characters")
                ),
                new Hand(
                    new Tile(3, "circles"),
                    new Tile(4, "circles"),
                    new Tile(5, "circles")
                ),
                new Hand(
                    new Tile(6, "circles"),
                    new Tile(7, "circles"),
                    new Tile(8, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "characters"),
                    new Tile(1, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "characters"),
                    new Tile(1, "characters")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "characters"),
                    new Tile(1, "bamboo")
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
            private static readonly object[] _GoodKangTestData =
            {
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo")
                ),
                new Hand(
                    new Tile(1, "characters"),
                    new Tile(1, "characters"),
                    new Tile(1, "characters"),
                    new Tile(1, "characters")
                ),
                new Hand(
                    new Tile(1, "circles"),
                    new Tile(1, "circles"),
                    new Tile(1, "circles"),
                    new Tile(1, "circles")
                )
            };

            [TestCaseSource(nameof(_GoodKangTestData))]
            public void TestIsKang(Hand hand)
            {
                var result = MainLogic.CalculateKang(hand);
                Assert.IsTrue(result);
            }

            private static readonly object[] _BadKangTestData =
            {
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(2, "bamboo"),
                    new Tile(3, "bamboo"),
                    new Tile(4, "bamboo")
                ),
                new Hand(
                    new Tile(5, "bamboo"),
                    new Tile(6, "bamboo"),
                    new Tile(7, "bamboo"),
                    new Tile(8, "bamboo")
                ),
                new Hand(
                    new Tile(2, "characters"),
                    new Tile(3, "characters"),
                    new Tile(4, "characters"),
                    new Tile(5, "characters")
                ),
                new Hand(
                    new Tile(6, "characters"),
                    new Tile(7, "characters"),
                    new Tile(8, "characters"),
                    new Tile(9, "characters")
                ),
                new Hand(
                    new Tile(3, "circles"),
                    new Tile(4, "circles"),
                    new Tile(5, "circles"),
                    new Tile(6, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "circles")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "circles"),
                    new Tile(1, "bamboo")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "characters"),
                    new Tile(1, "characters"),
                    new Tile(1, "bamboo")
                ),
                new Hand(
                    new Tile(1, "bamboo"),
                    new Tile(1, "characters"),
                    new Tile(1, "bamboo"),
                    new Tile(1, "bamboo")
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