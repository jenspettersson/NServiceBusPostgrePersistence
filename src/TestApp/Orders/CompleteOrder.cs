using NServiceBus;

namespace TestApp.Orders
{
    public class CompleteOrder : IMessage
    {
        public string OrderId { get; set; }
    }
}