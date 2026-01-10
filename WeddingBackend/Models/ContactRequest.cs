using System.ComponentModel.DataAnnotations;

namespace WeddingBackend.Models
{
    // Matches the payload posted by the Blazor client, with validation attributes.
    public class ContactRequest
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name must be 100 characters or less.")]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(256)]
        public string Email { get; set; } = "";

        [StringLength(200)]
        public string Subject { get; set; } = "Message from wedding site";

        [Required]
        [StringLength(4000, ErrorMessage = "Message is too long.")]
        public string Message { get; set; } = "";

        // Honeypot (should be empty). No validation attribute — checked server side.
        public string Hp { get; set; } = "";
    }
}