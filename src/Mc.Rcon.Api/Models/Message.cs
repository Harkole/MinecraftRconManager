namespace Mc.Rcon.Api.Models
{
    public class Message
    {
        public int Length { get; set; }

        public int Id { get; set; }

        public MessageTypes MessageType { get; set; }

        public string Body { get; set; } = string.Empty;
    }
}
