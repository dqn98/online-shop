using Contracts.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Identity;

namespace OcelotApiGateway.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    public TokenController(ITokenService tokenService)
    {
        ArgumentNullException.ThrowIfNull(tokenService);
        
        _tokenService = tokenService;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetToken()
    {
        var token = _tokenService.GetToken(new TokenRequest());
        return Ok(token);
    }
}