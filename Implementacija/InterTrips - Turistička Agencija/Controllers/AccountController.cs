using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public record LoginDto(string Email, string Password);
    public record RegisterDto(string Name, string Email, string Password);

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized(new { success = false, message = "Pogrešan email ili lozinka." });

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, dto.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded) return Unauthorized(new { success = false, message = "Pogrešan email ili lozinka." });

        return Ok(new { success = true });
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null) return BadRequest(new { success = false, message = "Email je već registrovan." });

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Ime = dto.Name
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { success = false, message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        await _signInManager.SignInAsync(user, isPersistent: false);
        return Ok(new { success = true });
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { success = true });
    }
}