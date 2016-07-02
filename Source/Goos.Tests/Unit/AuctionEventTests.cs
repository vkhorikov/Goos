using Goos.Logic;
using Should;
using Xunit;

namespace Goos.Tests.Unit
{
    public class AuctionEventTests
    {
        [Fact]
        public void Parses_close_events()
        {
            string message = "SOLVersion: 1.1; Event: CLOSE;";

            AuctionEvent auctionEvent = AuctionEvent.From(message);
            string serializedEvent = auctionEvent.ToString();

            auctionEvent.Type.ShouldEqual(AuctionEventType.Close);
            serializedEvent.ShouldEqual(message);
        }


        [Fact]
        public void Parses_price_events()
        {
            string message = "SOLVersion: 1.1; Event: PRICE; CurrentPrice: 12; Increment: 34; Bidder: sniper-56;";

            AuctionEvent auctionEvent = AuctionEvent.From(message);
            string serializedEvent = auctionEvent.ToString();

            auctionEvent.Type.ShouldEqual(AuctionEventType.Price);
            auctionEvent.CurrentPrice.ShouldEqual(12);
            auctionEvent.Increment.ShouldEqual(34);
            auctionEvent.Bidder.ShouldEqual("sniper-56");
            serializedEvent.ShouldEqual(message);
        }


        [Fact]
        public void Does_not_parse_events_with_incorrect_type()
        {
            string message = "SOLVersion: 1.1; Event: SOME_NONEXISTING_TYPE;";

            AuctionEvent auctionEvent = AuctionEvent.From(message);

            auctionEvent.Type.ShouldEqual(AuctionEventType.Unknown);
        }


        [Fact]
        public void Does_not_parse_events_with_incorrect_format()
        {
            string message = "Incorrectly formatted message";

            AuctionEvent auctionEvent = AuctionEvent.From(message);

            auctionEvent.Type.ShouldEqual(AuctionEventType.Unknown);
        }


        [Fact]
        public void Close_method_returns_a_close_event()
        {
            AuctionEvent auctionEvent = AuctionEvent.Close();

            auctionEvent.Type.ShouldEqual(AuctionEventType.Close);
            auctionEvent.ToString().ShouldEqual("SOLVersion: 1.1; Event: CLOSE;");
        }


        [Fact]
        public void Price_method_returns_a_price_event()
        {
            AuctionEvent auctionEvent = AuctionEvent.Price(1, 2, "bidder");

            auctionEvent.Type.ShouldEqual(AuctionEventType.Price);
            auctionEvent.CurrentPrice.ShouldEqual(1);
            auctionEvent.Increment.ShouldEqual(2);
            auctionEvent.Bidder.ShouldEqual("bidder");
            auctionEvent.ToString().ShouldEqual("SOLVersion: 1.1; Event: PRICE; CurrentPrice: 1; Increment: 2; Bidder: bidder;");
        }
    }
}
