using System;
using System.Linq;
using Should;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.Utility;

namespace Goos.Tests.EndToEnd
{
    public class ApplicationRunner
    {
        private readonly Application _application;


        public ApplicationRunner(Application application)
        {
            _application = application;
        }


        public void StartBiddingInWithStopPrice(int auctionId, int stopPrice)
        {
            Window window = GetMainWindow();

            var itemIdTextBox = window.Get<TextBox>(SearchCriteria.Indexed(0));
            var stopPriceTextBox = window.Get<TextBox>(SearchCriteria.Indexed(1));

            itemIdTextBox.Text = auctionId.ToString();
            stopPriceTextBox.Text = stopPrice.ToString();

            var button = window.Get<Button>(SearchCriteria.ByText("Join Auction"));
            button.Click();
        }


        public void StartBiddingIn(int auctionId)
        {
            StartBiddingInWithStopPrice(auctionId, int.MaxValue);
        }


        private Window GetMainWindow()
        {
            return Retry.For(
                () => _application.GetWindows().First(x => x.Title.Contains("Auction Sniper")),
                TimeSpan.FromSeconds(5));
        }


        public void HasShownSniperHasLostAuction(int auctionId, int lastPrice, int lastBid)
        {
            ContainsRow(auctionId, lastPrice, lastBid, "Lost").ShouldBeTrue();
        }


        public void HasShownSniperIsBidding(int auctionId, int lastPrice, int lastBid)
        {
            ContainsRow(auctionId, lastPrice, lastBid, "Bidding").ShouldBeTrue();
        }


        public void HasShownSniperIsWinning(int auctionId, int winningBid)
        {
            ContainsRow(auctionId, winningBid, winningBid, "Winning").ShouldBeTrue();
        }


        public void HasShownSniperHasWonAuction(int auctionId, int lastPrice)
        {
            ContainsRow(auctionId, lastPrice, lastPrice, "Won").ShouldBeTrue();
        }


        public void HasShownSniperIsLosing(int auctionId, int lastPrice, int lastBid)
        {
            ContainsRow(auctionId, lastPrice, lastBid, "Losing").ShouldBeTrue();
        }


        public void HasShownSniperHasFailed(int auctionId)
        {
            ContainsRow(auctionId, 0, 0, "Failed").ShouldBeTrue();
        }


        private bool ContainsRow(int auctionId, int lastPrice, int lastBid, string status)
        {
            Window window = GetMainWindow();
            var dataGrid = window.Get<ListView>();

            string[] data = { auctionId.ToString(), lastPrice.ToString(), lastBid.ToString(), status };
            return Retry.For(() => ContainsRow(dataGrid, data), TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(500));
        }


        private bool ContainsRow(ListView listView, string[] data)
        {
            foreach (ListViewRow row in listView.Rows)
            {
                if (DataMatches(row, data))
                    return true;
            }

            return false;
        }


        private bool DataMatches(ListViewRow row, string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (row.Cells[i].Text != data[i])
                    return false;
            }
            return true;
        }


        public void ReportsInvalidMessage(int auctionId, string brokenMessage)
        {
            // Look in the log file here
        }
    }
}
