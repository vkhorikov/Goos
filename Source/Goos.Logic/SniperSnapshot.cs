namespace Goos.Logic
{
    public class SniperSnapshot
    {
        public int LastPrice { get; }
        public int LastBid { get; }
        public SniperState State { get; private set; }


        private SniperSnapshot(int lastPrice, int lastBid, SniperState state)
        {
            LastPrice = lastPrice;
            LastBid = lastBid;
            State = state;
        }


        public static SniperSnapshot Joining()
        {
            return new SniperSnapshot(0, 0, SniperState.Joining);
        }


        public SniperSnapshot Bidding(int newLastPrice, int newLastBid)
        {
            return new SniperSnapshot(newLastPrice, newLastBid, SniperState.Bidding);
        }


        public SniperSnapshot Winning(int newLastPrice)
        {
            return new SniperSnapshot(newLastPrice, LastBid, SniperState.Winning);
        }


        public SniperSnapshot Losing(int newLastPrice)
        {
            return new SniperSnapshot(newLastPrice, LastBid, SniperState.Losing);
        }


        public SniperSnapshot Lost()
        {
            return new SniperSnapshot(LastPrice, LastBid, SniperState.Lost);
        }


        public SniperSnapshot Won()
        {
            return new SniperSnapshot(LastPrice, LastBid, SniperState.Won);
        }


        public SniperSnapshot Failed()
        {
            return new SniperSnapshot(0, 0, SniperState.Failed);
        }
    }
}
