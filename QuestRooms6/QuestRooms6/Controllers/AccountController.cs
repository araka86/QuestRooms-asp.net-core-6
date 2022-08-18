using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestRooms6.Model.ViewModels;
using QuestRooms6_Model;
namespace QuestRooms6.Controllers
{
    [AllowAnonymous]
    //  [Authorize(Roles = $"{WebConstanta.AdminRole},{WebConstanta.CustomerRole}")]
    public partial class AccountController : Controller
    {
        private readonly UserManager<AplicationUser> _userManager;
        private readonly SignInManager<AplicationUser> _signInManager;
        public AccountController(UserManager<AplicationUser> userManager, SignInManager<AplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [BindProperty]
        public LoginRegistrViewModel Input { get; set; }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterMenu()
        {
            if (ModelState.IsValid)
            {
                string returnUrl = Url.Content("~/");
                var user = new AplicationUser
                {
                    UserName = Input.FullName,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    FullName = Input.FullName,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded) //When the administrator registers, add role admin
                {
                    if (User.IsInRole(WebConstanta.AdminRole))
                    {
                        await _userManager.AddToRoleAsync(user, WebConstanta.AdminRole);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, WebConstanta.CustomerRole);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginRegistrViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login()
        {
            //  ModelState.SetModelValue("ConfirmPassword", new ValueProviderResult(Output.Password,  CultureInfo.InvariantCulture)); //change modelstate

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.FullName, Input.Password, Input.RememberMe, false);
                    //  var usertest = await _userManager.FindByNameAsync("testUser");
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(Input.ReturnUrl) && Url.IsLocalUrl(Input.ReturnUrl))
                        {
                            return Redirect(Input.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect login and/or password");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "A user with this address is not registered");
                }
            }
            return View(Input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
