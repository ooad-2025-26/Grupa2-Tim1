using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InterTrips___Turistička_Agencija.Controllers;


[Route("Account")]
[Route("accounts")]
public class AccountController : Controller
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
   
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? await _userManager.FindByNameAsync(dto.Email);

        if (user == null)
            return Unauthorized(new { success = false, message = "Pogrešan email ili lozinka." });

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, dto.Password, false, false);
        if (!result.Succeeded)
            return Unauthorized(new { success = false, message = "Pogrešan email ili lozinka." });

        var roles = await _userManager.GetRolesAsync(user);

        var redirectUrl = roles.Contains("Admin") ? "/Administrator"
                : roles.Contains("Agent") ? "/Agent"
                : "/";

        return Ok(new { success = true, redirectUrl });
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
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Me")]
    public IActionResult Me()
    {
        if (User?.Identity?.IsAuthenticated != true)
            return Unauthorized(new { isAuthenticated = false });

        return Ok(new
        {
            isAuthenticated = true,
            userName = User.Identity!.Name
        });
    }
}