namespace OAuthApi.Database
{
	public class User
	{
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public List<string> Regions { get; set; } = new List<string>();
	}
}
