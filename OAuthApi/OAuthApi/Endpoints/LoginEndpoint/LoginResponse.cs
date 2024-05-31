namespace OAuthApi.Endpoints.LoginEndpoint
{
	public class LoginResponse
	{
		public string Token { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public List<string> Regions { get; set; } = new List<string>();
	}
}
