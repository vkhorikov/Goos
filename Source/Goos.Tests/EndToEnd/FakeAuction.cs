using System.Threading;

using Goos.XmppSimulator;

using Should;

namespace Goos.Tests.EndToEnd
{
    public class FakeAuction
    {
        private const string User = "Some fake user";

        public readonly int AuctionId;
        private XmppChat _chat;

        private string _receivedMessage = string.Empty;
        private readonly ManualResetEventSlim _canWrite = new ManualResetEventSlim(true);
        private readonly ManualResetEventSlim _canRead = new ManualResetEventSlim(false);


        public FakeAuction(int auctionId)
        {
            AuctionId = auctionId;
        }


        public void StartSellingItem(XmppConnection connection)
        {
            connection.CreateChat(AuctionId);
            _chat = connection.ConnectToChat(AuctionId, User);
            _chat.MessageReceived += MessageReceived;
        }


        private void MessageReceived(string message)
        {
            _canWrite.Wait();
            _canWrite.Reset();

            Volatile.Write(ref _receivedMessage, message);

            _canRead.Set();
        }


        public void HasReceivedJoinRequest()
        {
            HasReceivedMessage("SOLVersion: 1.1; Command: JOIN;");
        }


        public void HasReceivedBid(int bid)
        {
            HasReceivedMessage($"SOLVersion: 1.1; Command: BID; Price: {bid};");
        }


        private void HasReceivedMessage(string message)
        {
            _canRead.Wait();
            _canRead.Reset();

            string receivedMessage = Volatile.Read(ref _receivedMessage);
            receivedMessage.ShouldEqual(message);

            _canWrite.Set();
        }


        public void AnnounceClosed()
        {
            _chat.SendMessage("SOLVersion: 1.1; Event: CLOSE;");
        }


        public void ReportPrice(int currentPrice, int increment, string bidder)
        {
            _chat.SendMessage($"SOLVersion: 1.1; Event: PRICE; CurrentPrice: {currentPrice}; Increment: {increment}; Bidder: {bidder};");
        }


        public void SendInvalidMessageContaining(string brokenMessage)
        {
            _chat.SendMessage(brokenMessage);
        }
    }
}
