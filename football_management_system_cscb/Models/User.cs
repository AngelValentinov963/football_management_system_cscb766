public class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public string Role { get; set; } = "";

    public int? TeamId { get; set; }

    public DateTime CreatedAt { get; set; }
}