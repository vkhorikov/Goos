using System;
using Goos.Logic;
using Should;
using Xunit;

namespace Goos.Tests.Unit
{
    public class AuctionSniperTests
    {
        [Fact]
        public void New_auction_is_in_joining_state()
        {
            var sniper = new AuctionSniper("me", 34);

            sniper.Bidder.ShouldEqual("me");
            sniper.StopPrice.ShouldEqual(34);
            sniper.StateShouldBe(SniperState.Joining, 0, 0);
        }


        [Fact]
        public void Joining_sniper_loses_when_auction_closes()
        {
            var sniper = new AuctionSniper("", 200);

            AuctionCommand command = sniper.Process(AuctionEvent.Close());

            command.ShouldEqual(AuctionCommand.None());
            sniper.StateShouldBe(SniperState.Lost, 0, 0);
        }


        [Fact]
        public void Sniper_bids_when_price_event_with_a_different_bidder_arrives()
        {
            var sniper = new AuctionSniper("", 200);

            AuctionCommand command = sniper.Process(AuctionEvent.Price(1, 2, "some bidder"));

            command.ShouldEqual(AuctionCommand.Bid(3));
            sniper.StateShouldBe(SniperState.Bidding, 1, 3);
        }


        [Fact]
        public void Bidding_sniper_loses_when_auction_closes()
        {
            AuctionSniper sniper = CreateBiddingSniper();

            AuctionCommand command = sniper.Process(AuctionEvent.Close());

            command.ShouldEqual(AuctionCommand.None());
            sniper.StateShouldBe(SniperState.Lost, 1, 3);
        }


        [Fact]
        public void Bidding_sniper_is_winning_when_price_event_with_the_same_bidder_arrives()
        {
            AuctionSniper sniper = CreateBiddingSniper("bidder");

            AuctionCommand command = sniper.Process(AuctionEvent.Price(3, 2, "bidder"));

            command.ShouldEqual(AuctionCommand.None());
            sniper.StateShouldBe(SniperState.Winning, 3, 3);
        }


        [Fact]
        public void Sniper_is_losing_when_it_cannot_beat_last_bid()
        {
            var sniper = new AuctionSniper("bidder", 20);

            AuctionCommand command = sniper.Process(AuctionEvent.Price(15, 10, "other bidder"));

            command.ShouldEqual(AuctionCommand.None());
            sniper.StateShouldBe(SniperState.Losing, 15, 0);
        }


        [Fact]
        public void Winning_sniper_wins_when_auction_closes()
        {
            AuctionSniper sniper = CreateWinningSniper();

            AuctionCommand command = sniper.Process(AuctionEvent.Close());

            command.ShouldEqual(AuctionCommand.None());
            sniper.Snapshot.State.ShouldEqual(SniperState.Won);
        }


        [Fact]
        public void Losing_sniper_loses_when_auction_closes()
        {
            AuctionSniper sniper = CreateLosingSniper();

            AuctionCommand command = sniper.Process(AuctionEvent.Close());

            command.ShouldEqual(AuctionCommand.None());
            sniper.Snapshot.State.ShouldEqual(SniperState.Lost);
        }


        [Fact]
        public void Sniper_fails_when_auction_sends_unknown_event()
        {
            AuctionSniper sniper = CreateLosingSniper();

            AuctionCommand command = sniper.Process(AuctionEvent.From("Some corrupted message"));

            command.ShouldEqual(AuctionCommand.None());
            sniper.StateShouldBe(SniperState.Failed, 0, 0);
        }


        [Fact]
        public void Failed_sniper_does_not_react_on_further_messages()
        {
            AuctionSniper sniper = CreateFailedSniper();

            AuctionCommand command = sniper.Process(AuctionEvent.Price(10, 5, "some bidder"));

            command.ShouldEqual(AuctionCommand.None());
            sniper.StateShouldBe(SniperState.Failed, 0, 0);
        }


        private AuctionSniper CreateBiddingSniper(string bidder = "some bidder")
        {
            var sniper = new AuctionSniper(bidder, 200);
            sniper.Process(AuctionEvent.Price(1, 2, Guid.NewGuid().ToString()));
            return sniper;
        }


        private AuctionSniper CreateWinningSniper()
        {
            var sniper = new AuctionSniper("bidder", 200);
            sniper.Process(AuctionEvent.Price(1, 2, "bidder"));
            return sniper;
        }


        private AuctionSniper CreateLosingSniper()
        {
            var sniper = new AuctionSniper("bidder", 20);
            sniper.Process(AuctionEvent.Price(20, 5, "other bidder"));
            return sniper;
        }


        private AuctionSniper CreateFailedSniper()
        {
            var sniper = new AuctionSniper("bidder", 20);
            sniper.Process(AuctionEvent.From("Some corrupted message"));
            return sniper;
        }
    }


    internal static class AuctionSniperExtentions
    {
        public static void StateShouldBe(this AuctionSniper sniper, SniperState state, int lastPrice, int lastBid)
        {
            sniper.Snapshot.State.ShouldEqual(state);
            sniper.Snapshot.LastPrice.ShouldEqual(lastPrice);
            sniper.Snapshot.LastBid.ShouldEqual(lastBid);
        }
    }
}
