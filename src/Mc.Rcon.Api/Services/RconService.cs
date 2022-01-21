using Mc.Rcon.Api.Interfaces;
using Mc.Rcon.Api.Models;
using System.Net.Sockets;
using System.Text;

namespace Mc.Rcon.Api.Services
{
    public class RconService : IRconService
    {
        // Messages over this size will be truncated
        private const int MaximumMessageSize = 4096;

        // Size of the header without the length data
        private const int HeaderSize = 10;

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;

        private int _lastId;

        /// <summary>
        /// Congifuration of the <c>RconService</c>
        /// </summary>
        /// <param name="host">Host address where the server is</param>
        /// <param name="port">Post number to use, defaults to Minecraft default port 25575</param>
        public RconService(string host, int port = 25575)
        {
            _tcpClient = new TcpClient(host, port);
            _stream = _tcpClient.GetStream();
        }

        /// <inheritdoc />
        public bool Authenticate(string password)
        {
            Message message = new()
            {
                MessageType = MessageType.Authenticate,
                Id = _lastId++,
                Body = password,
                Length = password.Length + HeaderSize,
            };

            return SendMessage(message, out Message _);
        }

        /// <inheritdoc />
        public void CloseConnection()
        {
            if (null != _stream)
            {
                _stream.Close();
                _stream.Dispose();
            }

            if (null != _tcpClient)
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
            }
        }

        /// <inheritdoc />
        public bool SendCommand(string command, out string response)
        {
            Message message = new()
            {
                MessageType = MessageType.Command,
                Id = _lastId++,
                Body = command,
                Length = command.Length + HeaderSize,
            };

            bool isSuccess = SendMessage(message, out Message messageResponse);
            response = messageResponse.Body;

            return isSuccess;
        }

        /// <summary>
        /// Sends and processes the server response for a <c>Message</c>
        /// </summary>
        /// <param name="request">The Message containing the command to execute</param>
        /// <param name="response">The server response to the request</param>
        /// <returns>True if the message was succesfully processed and the response recieved</returns>
        private bool SendMessage(Message request, out Message response)
        {
            bool isSuccess = false;

            // Send the request
            byte[] encodedMessage = EncodeMessage(request);
            _stream.Write(encodedMessage, 0, encodedMessage.Length);

            // Get the response
            byte[] responseBytes = new byte[MaximumMessageSize];
            int bytesRead = _stream.Read(responseBytes, 0, responseBytes.Length);
            Array.Resize(ref responseBytes, bytesRead);

            // Decode the response
            response = DecodeMessage(responseBytes);

            if (request.Id == response.Id)
            {
                // This is the response you are looking for...
                isSuccess = true;
            }

            return isSuccess;
        }

        /// <summary>
        /// Encodes the Message object in to a byte array for sending to the server
        /// </summary>
        /// <param name="message">Details of what to send to the server</param>
        /// <returns>The byte array raw payload</returns>
        private static byte[] EncodeMessage(Message message)
        {
            List<byte> bytes = new();

            bytes.AddRange(BitConverter.GetBytes(message.Id));
            bytes.AddRange(BitConverter.GetBytes((int)message.MessageType));
            bytes.AddRange(Encoding.ASCII.GetBytes(message.Body));
            bytes.AddRange(BitConverter.GetBytes(message.Length));

            // Add two bytes padding
            bytes.AddRange(new byte[] { 0, 0, });

            return bytes.ToArray();
        }

        /// <summary>
        /// Decodes the byte array that is returned in to the Message object
        /// </summary>
        /// <param name="response">The servers response</param>
        /// <returns>A constructed <c>Message</c> that holds the detailed response</returns>
        private static Message DecodeMessage(byte[] response)
        {
            // Get body length after header (10) and length data (4) are removed
            int bodyLength = response.Length - 14;

            Message message = new()
            {
                Length = BitConverter.ToInt32(response, 0),
                Id = BitConverter.ToInt32(response, 4),
            };

            // Get the message type and parse it back to the enum
            string messageType = BitConverter.ToInt32(response, 8).ToString();
            if (Enum.TryParse(messageType, out MessageType type))
            {
                message.MessageType = type;
            }

            if (0 < bodyLength)
            {
                // Create a new array for just the body and copy the body part of the response in to it
                byte[] bodyBytes = new byte[bodyLength];
                Array.Copy(response, 12, bodyBytes, 0, bodyLength);

                message.Body = Encoding.ASCII.GetString(bodyBytes);
            }
            else
            {
                // There is no body...
                message.Body = string.Empty;
            }

            return message;
        }
    }
}
