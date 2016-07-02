using Goos.Logic;
using Goos.XmppSimulator;

namespace Goos.UI
{
    public class AuctionSniperViewModel : ViewModel
    {
        private readonly AuctionSniper _auctionSniper;
        private readonly XmppChat _chat;
        private readonly int _auctionId;

        public string ItemId => _auctionId.ToString();
        public string LastPrice => _auctionSniper.Snapshot.LastPrice.ToString();
        public string LastBid => _auctionSniper.Snapshot.LastBid.ToString();
        public string State => _auctionSniper.Snapshot.State.ToString();


        public AuctionSniperViewModel(int auctionId, int stopPrice, string bidder, XmppChat chat)
        {
            _auctionId = auctionId;
            _auctionSniper = new AuctionSniper(bidder, stopPrice);
            _chat = chat;
            _chat.MessageReceived += ChatMessageRecieved;
            _chat.SendMessage(AuctionCommand.Join().ToString());
        }


        private void ChatMessageRecieved(string message)
        {
            AuctionEvent ev = AuctionEvent.From(message);
            AuctionCommand command = _auctionSniper.Process(ev);
            if (command != AuctionCommand.None())
            {
                _chat.SendMessage(command.ToString());
            }

            Notify(nameof(LastPrice));
            Notify(nameof(LastBid));
            Notify(nameof(State));
        }
    }
}
