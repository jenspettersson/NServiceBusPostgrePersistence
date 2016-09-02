using NServiceBus;

namespace TestApp.Orders
{
    public class StartOrder : IMessage
    {
        public string OrderId { get; set; }
    }
}