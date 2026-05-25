using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace InterTrips___Turistička_Agencija.Controllers
{
    [Route("Account")]
    [Route("accounts")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterDto
        {
            public string Ime { get; set; } = string.Empty;
            public string Prezime { get; set; } = string.Empty;
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
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { success = false, message = "Popunite sva polja." });

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
        
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { success = false, message = "Podaci nisu ispravno poslani." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Model nije validan." });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Ime = dto.Ime,
                Prezime = dto.Prezime,
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                string klijentUloga = "Client";
                if (!await _roleManager.RoleExistsAsync(klijentUloga))
                {
                    await _roleManager.CreateAsync(new IdentityRole(klijentUloga));
                }
                await _userManager.AddToRoleAsync(user, klijentUloga);

                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok(new { success = true, redirectUrl = "/" });
            }

            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = errors });
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
        {
            return Content("Pristup odbijen. Nemate administratorske ili agentske ovlasti za ovaj dio stranice.");
        }
    }
}