using Xunit;

namespace Goos.Tests.EndToEnd
{
    public class AuctionSniperTests : Tests
    {
        private const int Auction = 12345;
        private const int Auction2 = 23456;
        private readonly FakeAuction _auction = new FakeAuction(Auction);
        private readonly FakeAuction _auction2 = new FakeAuction(Auction2);


        [Fact]
        public void Sniper_joins_auction_until_auction_closes()
        {
            _auction.StartSellingItem(_connection);
            _application.StartBiddingIn(Auction);
            _auction.HasReceivedJoinRequest();
            _auction.AnnounceClosed();
            _application.HasShownSniperHasLostAuction(_auction.AuctionId, 0, 0);
        }


        [Fact]
        public void Sniper_makes_a_higher_bid_but_loses()
        {
            _auction.StartSellingItem(_connection);

            _application.StartBiddingIn(Auction);
            _auction.HasReceivedJoinRequest();
            _auction.ReportPrice(1000, 98, "other bidder");
            _application.HasShownSniperIsBidding(_auction.AuctionId, 1000, 1098);

            _auction.HasReceivedBid(1098);

            _auction.AnnounceClosed();
            _application.HasShownSniperHasLostAuction(_auction.AuctionId, 1000, 1098);
        }


        [Fact]
        public void Sniper_wins_an_auction_by_bidding_higher()
        {
            _auction.StartSellingItem(_connection);

            _application.StartBiddingIn(Auction);
            _auction.HasReceivedJoinRequest();
            _auction.ReportPrice(1000, 98, "other bidder");
            _application.HasShownSniperIsBidding(_auction.AuctionId, 1000, 1098);

            _auction.HasReceivedBid(1098);

            _auction.ReportPrice(1098, 97, "Test user");
            _application.HasShownSniperIsWinning(_auction.AuctionId, 1098);

            _auction.AnnounceClosed();
            _application.HasShownSniperHasWonAuction(_auction.AuctionId, 1098);
        }


        [Fact]
        public void Sniper_bids_for_multiple_items()
        {
            _auction.StartSellingItem(_connection);
            _auction2.StartSellingItem(_connection);

            _application.StartBiddingIn(Auction);
            _application.StartBiddingIn(Auction2);
            _auction.HasReceivedJoinRequest();
            _auction2.HasReceivedJoinRequest();

            _auction.ReportPrice(1000, 98, "other bidder");
            _auction.HasReceivedBid(1098);

            _auction2.ReportPrice(500, 21, "other bidder");
            _auction2.HasReceivedBid(521);

            _auction.ReportPrice(1098, 97, "Test user");
            _auction2.ReportPrice(521, 22, "Test user");

            _application.HasShownSniperIsWinning(_auction.AuctionId, 1098);
            _application.HasShownSniperIsWinning(_auction2.AuctionId, 521);

            _auction.AnnounceClosed();
            _auction2.AnnounceClosed();

            _application.HasShownSniperHasWonAuction(_auction.AuctionId, 1098);
            _application.HasShownSniperHasWonAuction(_auction2.AuctionId, 521);
        }


        [Fact]
        public void Sniper_loses_an_auction_when_the_price_is_too_high()
        {
            _auction.StartSellingItem(_connection);

            _application.StartBiddingInWithStopPrice(Auction, 1100);
            _auction.HasReceivedJoinRequest();
            _auction.ReportPrice(1000, 98, "other bidder");
            _application.HasShownSniperIsBidding(Auction, 1000, 1098);

            _auction.HasReceivedBid(1098);

            _auction.ReportPrice(1197, 10, "third party");
            _application.HasShownSniperIsLosing(Auction, 1197, 1098);

            _auction.ReportPrice(1207, 10, "fourth party");
            _application.HasShownSniperIsLosing(Auction, 1207, 1098);
            _auction.AnnounceClosed();
            _application.HasShownSniperHasLostAuction(Auction, 1207, 1098);
        }


        [Fact]
        public void Sniper_reports_invalid_auction_message_and_stops_responding_to_events()
        {
            string brokenMessage = "a broken message";
            _auction.StartSellingItem(_connection);
            _auction2.StartSellingItem(_connection);

            _application.StartBiddingIn(Auction);
            _application.StartBiddingIn(Auction2);
            _auction.HasReceivedJoinRequest();

            _auction.ReportPrice(500, 20, "other bidder");
            _auction.HasReceivedBid(520);

            _auction.SendInvalidMessageContaining(brokenMessage);
            _application.HasShownSniperHasFailed(Auction);

            _auction.ReportPrice(520, 21, "other bidder");
            WaitForAnotherAuctionEvent();

            _application.ReportsInvalidMessage(Auction, brokenMessage);
            _application.HasShownSniperHasFailed(Auction);
        }


        private void WaitForAnotherAuctionEvent()
        {
            _auction2.HasReceivedJoinRequest();
            _auction2.ReportPrice(600, 6, "other bidder");
            _application.HasShownSniperIsBidding(Auction2, 600, 606);
        }
    }
}
