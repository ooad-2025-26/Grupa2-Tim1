using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

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
                : roles.Contains("Agent") ? $"/Agent?agentId={user.Id}"
                : "/";

        return Ok(new { success = true, redirectUrl });
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest(new { success = false, message = "Sva polja su obavezna!" });
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
           
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var kreiraniKorisnik = await _userManager.FindByNameAsync(user.UserName);
            if (kreiraniKorisnik != null)
            {
                await _userManager.AddToRoleAsync(kreiraniKorisnik, "Client");
            }
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Json(new { success = true, redirectUrl = "/" });
        }

        var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
        return BadRequest(new { success = false, message = errorMessage });
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
    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied()
    { return Content("Pristup odbijen. Nemate administratorske ili agentske ovlasti za ovaj dio stranice.");
    }
}