using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Shared.Application.Cqrs;
using CleanArchitecture.Shared.Core.Result;
using CleanArchitecture.Shopping.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Shopping.Application.User.Queries
{
    public class AuthenticateUserQuery : IQuery<Result<AuthenticationResponseDto>>
    {
        public AuthenticateUserQuery(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public string Username { get; }

        public string Password { get; }
    }

    internal class AuthenticateUserQueryHandler : IQueryHandler<AuthenticateUserQuery, Result<AuthenticationResponseDto>>
    {
        private readonly IIdentityUserRepository identityUserRepository;
        private readonly AuthenticationConfiguration configuration;

        public AuthenticateUserQueryHandler(
            IIdentityUserRepository identityUserRepository,
            IOptions<AuthenticationConfiguration> configuration)
        {
            this.identityUserRepository = identityUserRepository ?? throw new ArgumentNullException(nameof(identityUserRepository));
            this.configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Result<AuthenticationResponseDto>> Handle(
            AuthenticateUserQuery request,
            CancellationToken cancellationToken)
        {
            var user = await this.identityUserRepository.GetByNameAsync(request.Username);
            if (user == null)
            {
                return Result<AuthenticationResponseDto>.Unauthorized($"User {request.Username} not found.");
            }

            if (!await this.identityUserRepository.CheckPasswordAsync(user, request.Password))
            {
                return Result<AuthenticationResponseDto>.Unauthorized("Invalid login credentials.");
            }

            var expires = DateTime.UtcNow.AddDays(7);

            var principal = await this.identityUserRepository.CreatePrincipalAsync(user);
            var userRoles = await this.identityUserRepository.GetRolesForUserAsync(user);

            string token = this.GenerateToken(principal, userRoles, expires);

            var authUserModel = new AuthenticationResponseDto
            {
                Id = new Guid(user.Id),
                Username = user.UserName,
                EmailAddress = user.Email,
                Token = token,
                TokenExpiryDate = expires
            };

            return authUserModel;
        }

        private string GenerateToken(
            ClaimsPrincipal principal,
            IEnumerable<string> roles,
            DateTime expires)
        {
            var claims = new List<Claim>();

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(principal.Identity, claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = expires,
                Issuer = "https://localhost/clean-architecture",
                Audience = "clean-architecture",
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.Secret)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
