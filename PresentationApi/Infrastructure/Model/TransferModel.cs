namespace PresentationApi.Infrastructure.Model
{
    public sealed class TransferModel
    {
        public string From { get; set; }

        public string To { get; set; }

        public decimal Amount { get; set; }
    }
}
