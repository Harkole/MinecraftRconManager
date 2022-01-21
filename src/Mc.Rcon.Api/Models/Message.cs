namespace Mc.Rcon.Api.Models
{
    public class Message
    {
        /// <summary>
        /// Message length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Identity of the message
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The type of message this is
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// The message payload/response body
        /// </summary>
        public string Body { get; set; } = string.Empty;
    }
}
