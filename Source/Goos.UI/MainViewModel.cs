using System.Collections.ObjectModel;
using Goos.XmppSimulator;

namespace Goos.UI
{
    public class MainViewModel
    {
        private const string UserName = "Test user";

        private readonly XmppConnection _connection;
        public ObservableCollection<AuctionSniperViewModel> Snipers { get; }

        public int NewItemId { get; set; }
        public int NewItemStopPrice { get; set; }
        public Command JoinAuctionCommand { get; private set; }


        public MainViewModel(XmppConnection connection)
        {
            _connection = connection;
            JoinAuctionCommand = new Command(JoinAuction);
            Snipers = new ObservableCollection<AuctionSniperViewModel>();
        }


        private void JoinAuction()
        {
            XmppChat chat = _connection.ConnectToChat(NewItemId, UserName);
            var viewModel = new AuctionSniperViewModel(NewItemId, NewItemStopPrice, UserName, chat);
            Snipers.Add(viewModel);
        }
    }
}
