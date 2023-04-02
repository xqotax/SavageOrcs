namespace SavageOrcs.DataTransferObjects._Constants
{
    public record ByteContentAndBooIsVisible
    {
        public byte[] Content { get; set; } = new byte[0];
        public bool IsVisible { get; set; }
    }
}
