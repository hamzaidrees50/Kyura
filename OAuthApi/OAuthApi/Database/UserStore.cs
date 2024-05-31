namespace OAuthApi.Database
{
	public static class UserStore
	{
		public static List<User> Users = new List<User>
		{
			new User { Username = "player1", Password = "password", Role = "player", Regions = new List<string> { "b_game" } },
			new User { Username = "vipuser", Password = "password", Role = "player", Regions = new List<string> { "b_game", "vip_chararacter_personalize" } },
			new User { Username = "adminuser", Password = "password", Role = "admin", Regions = new List<string> { "b_game", "vip_chararacter_personalize" } }
		};
	}
}
