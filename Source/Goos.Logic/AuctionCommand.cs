namespace Goos.Logic
{
    public struct AuctionCommand
    {
        private readonly string _content;


        private AuctionCommand(string content)
        {
            _content = content;
        }


        public override string ToString()
        {
            return _content;
        }


        public static AuctionCommand Join()
        {
            return new AuctionCommand("SOLVersion: 1.1; Command: JOIN;");
        }


        public static AuctionCommand Bid(int price)
        {
            return new AuctionCommand($"SOLVersion: 1.1; Command: BID; Price: {price};");
        }


        public static AuctionCommand None()
        {
            return new AuctionCommand(string.Empty);
        }


        public static bool operator ==(AuctionCommand left, AuctionCommand right)
        {
            return left.Equals(right);
        }


        public static bool operator !=(AuctionCommand left, AuctionCommand right)
        {
            return !(left == right);
        }
    }
}
