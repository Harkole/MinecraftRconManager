using System.ComponentModel.DataAnnotations;

namespace Mc.Rcon.Api.Models
{
    public class CommandRequest
    {
        /// <summary>
        /// The password to authenticate against RCON with
        /// </summary>
        [Required]
        public string? Password { get; set; }

        /// <summary>
        /// The server instruction to execute
        /// </summary>
        [Required]
        public string? Instruction { get; set; }
    }
}
