namespace Contracts.Http
{
    public struct SubscribeRequest
    {        
        public string Webhook_url { get; init; }
    }

    public struct SubscribeResponse
    {        
        public string Webhook_url { get; init; }
    }
}