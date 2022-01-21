namespace Mc.Rcon.Api.Interfaces
{
    public interface IRconService
    {
        /// <summary>
        /// Provides the Authentication for communication
        /// </summary>
        /// <param name="password">The password to manage the server</param>
        /// <returns>True if login successful</returns>
        bool Authenticate(string password);

        /// <summary>
        /// Closes the network connection and cleans up resources
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Sends the command as a message returning the server Message response
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="response">The servers response to the command, this maybe <c>string.Empty</c></param>
        /// <returns>True if the command executed and was responseded to</returns>
        bool SendCommand(string command, out string response);
    }
}
