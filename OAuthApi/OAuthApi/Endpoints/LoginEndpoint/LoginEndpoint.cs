using Carter;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using OAuthApi.Database;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace OAuthApi.Endpoints.LoginEndpoint;

public class LoginEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("/login", async (LoginRequest request, ISender sender) =>
		{
			var command = new UserLogin.Command
			{
				Username = request.Username,
				Password = request.Password
			};
			var result = await sender.Send(command);
			if (result is null)
			{
				return Results.Unauthorized();
			}
			return Results.Ok(result);
		}).WithOpenApi()
		.Produces<LoginResponse>((int)HttpStatusCode.OK)
		.Produces((int)HttpStatusCode.Unauthorized);
	}
}

public class UserLogin
{
	public class Command : IRequest<LoginResponse?>
	{
		public required string Username { get; set; }
		public required string Password { get; set; }
	}


	internal sealed class Handler(IConfiguration configuration)
		: IRequestHandler<Command, LoginResponse?>
	{
		public Task<LoginResponse?> Handle(Command request, CancellationToken cancellationToken)
		{
			var user = UserStore.Users
				.SingleOrDefault(u => u.Username == request.Username && u.Password == request.Password);
			if (user == null)
			{
				return Task.FromResult<LoginResponse?>(null);
			}
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, user.Username),
					new Claim(ClaimTypes.Role, user.Role),
					new Claim("regions", string.Join(",", user.Regions))
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
										 SecurityAlgorithms.HmacSha256Signature),
				Issuer = configuration["Jwt:Issuer"]!,
				Audience = configuration["Jwt:Audience"]!
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);
			return Task.FromResult<LoginResponse?>(new LoginResponse
			{
				Token = tokenString,
				Role = user.Role,
				Regions = user.Regions
			});

		}
	}
}
