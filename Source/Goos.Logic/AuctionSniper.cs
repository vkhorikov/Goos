using System;

namespace Goos.Logic
{
    public class AuctionSniper
    {
        public string Bidder { get; }
        public int StopPrice { get; }
        public SniperSnapshot Snapshot { get; private set; }


        public AuctionSniper(string bidder, int stopPrice)
        {
            Bidder = bidder;
            StopPrice = stopPrice;
            Snapshot = SniperSnapshot.Joining();
        }


        public AuctionCommand Process(AuctionEvent auctionEvent)
        {
            if (Snapshot.State == SniperState.Failed)
                return AuctionCommand.None();

            switch (auctionEvent.Type)
            {
                case AuctionEventType.Price:
                    return ProcessPriceEvent(auctionEvent.CurrentPrice, auctionEvent.Increment, auctionEvent.Bidder);

                case AuctionEventType.Close:
                    return ProcessCloseEvent();

                case AuctionEventType.Unknown:
                    return ProcessUnknownEvent();

                default:
                    throw new InvalidOperationException();
            }
        }


        private AuctionCommand ProcessUnknownEvent()
        {
            Snapshot = Snapshot.Failed();
            return AuctionCommand.None();
        }


        private AuctionCommand ProcessCloseEvent()
        {
            if (Snapshot.State == SniperState.Winning)
            {
                Snapshot = Snapshot.Won();
            }
            else
            {
                Snapshot = Snapshot.Lost();
            }

            return AuctionCommand.None();
        }


        private AuctionCommand ProcessPriceEvent(int currentPrice, int increment, string bidder)
        {
            if (Bidder == bidder)
            {
                Snapshot = Snapshot.Winning(currentPrice);
                return AuctionCommand.None();
            }

            int newBid = currentPrice + increment;
            if (newBid > StopPrice)
            {
                Snapshot = Snapshot.Losing(currentPrice);
                return AuctionCommand.None();
            }
            else
            {
                Snapshot = Snapshot.Bidding(currentPrice, newBid);
                return AuctionCommand.Bid(newBid);
            }
        }
    }
}
