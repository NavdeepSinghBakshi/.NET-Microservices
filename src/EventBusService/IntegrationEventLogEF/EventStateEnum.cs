namespace IntegrationEventLogEF
{
    public enum EventState
    {
        NotPublished = 0,
        InProgress = 1,
        Published = 2,
        PublishedFailed = 3,
        NotSubscribe = 4,
        Subscribe = 5,
        SubscribeFailed = 6
    }
}