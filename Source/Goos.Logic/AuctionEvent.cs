using System;
using System.Collections.Generic;
using System.Linq;


namespace Goos.Logic
{
    public class AuctionEvent
    {
        public AuctionEventType Type { get; }
        private readonly Dictionary<string, string> _fields;

        public int CurrentPrice => GetInt("CurrentPrice");
        public int Increment => GetInt("Increment");
        public string Bidder => Get("Bidder");


        private AuctionEvent(AuctionEventType type, Dictionary<string, string> fields)
        {
            Type = type;
            _fields = fields;
        }


        private int GetInt(string fieldName)
        {
            return int.Parse(Get(fieldName));
        }


        private string Get(string fieldName)
        {
            string value = _fields[fieldName];

            if (value == null)
                throw new InvalidOperationException(fieldName);

            return value;
        }


        public override string ToString()
        {
            return string.Join(" ", _fields.Select(x => x.Key + ": " + x.Value + ";"));
        }


        public static AuctionEvent From(string message)
        {
            if (!message.Contains(":") || !message.Contains(";"))
            {
                return new AuctionEvent(AuctionEventType.Unknown, new Dictionary<string, string>());
            }

            Dictionary<string, string> fields = GetFields(message);
            AuctionEventType eventType = GetEventType(fields);

            return new AuctionEvent(eventType, fields);
        }


        private static AuctionEventType GetEventType(Dictionary<string, string> fields)
        {
            if (!fields.ContainsKey("Event"))
                return AuctionEventType.Unknown;

            AuctionEventType type;
            if (!Enum.TryParse(fields["Event"], true, out type))
                return AuctionEventType.Unknown;

            return type;
        }


        private static Dictionary<string, string> GetFields(string message)
        {
            var fields = new Dictionary<string, string>();

            string[] pairs = message.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pair in pairs)
            {
                string[] data = pair.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                fields.Add(data[0].Trim(), data[1].Trim());
            }

            return fields;
        }


        public static AuctionEvent Close()
        {
            return From("SOLVersion: 1.1; Event: CLOSE;");
        }


        public static AuctionEvent Price(int currentPrice, int increment, string bidder)
        {
            return From($"SOLVersion: 1.1; Event: PRICE; CurrentPrice: {currentPrice}; Increment: {increment}; Bidder: {bidder};");
        }
    }


    public enum AuctionEventType
    {
        Close,
        Price,
        Unknown
    }
}
