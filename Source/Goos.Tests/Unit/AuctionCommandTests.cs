using Goos.Logic;
using Should;
using Xunit;


namespace Goos.Tests.Unit
{
    public class AuctionCommandTests
    {
        [Fact]
        public void Join_command_is_of_appropriate_content()
        {
            AuctionCommand command = AuctionCommand.Join();

            command.ToString().ShouldEqual("SOLVersion: 1.1; Command: JOIN;");
        }

        [Fact]
        public void Bid_command_is_of_appropriate_content()
        {
            AuctionCommand command = AuctionCommand.Bid(123);

            command.ToString().ShouldEqual("SOLVersion: 1.1; Command: BID; Price: 123;");
        }

        [Fact]
        public void Two_commands_are_equal_if_their_contents_match()
        {
            AuctionCommand command1 = AuctionCommand.Bid(123);
            AuctionCommand command2 = AuctionCommand.Bid(123);

            command1.ShouldEqual(command2);
            command1.GetHashCode().ShouldEqual(command2.GetHashCode());
        }
    }
}
