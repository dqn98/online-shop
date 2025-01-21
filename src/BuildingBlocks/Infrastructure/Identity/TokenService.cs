using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Contracts.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using Shared.DTOs.Identity;

namespace Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    
    public TokenService(JwtSettings jwtSettings)
    {
        ArgumentNullException.ThrowIfNull(jwtSettings);
        _jwtSettings = jwtSettings;
    }
    
    public TokenResponse GetToken(TokenRequest request)
    {
        var token = GenerateJwtToken();
        var result = new TokenResponse(token);
        return result;
    }
    
    private string GenerateJwtToken() => GenerateEncryptedJwtToken(GetSigningCredentials());
    
    private string GenerateEncryptedJwtToken(SigningCredentials signingCredentials)
    {
        var token = new JwtSecurityToken(
            // expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signingCredentials);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new NullReferenceException("Jwt key is not configured.");
        }
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret),
            SecurityAlgorithms.HmacSha256);
    }
}