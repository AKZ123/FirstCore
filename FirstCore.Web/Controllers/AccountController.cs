using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FirstCore.Web.Models;
using FirstCore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstCore.Web.Controllers
{
    //Part: 66.2
    public class AccountController : Controller
    {
        //Part: 67.1,  77.2.3
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Part: 75.1
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailIsUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already use");
            }
        }

        //Part: 67.2
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                //Part:        77.2.1,                                                       77.4.3
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, City=model.City };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //Part:84.4    //for Staying signedIn User on signIn after create a New User  
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    await signInManager.SignInAsync(user, isPersistent: false); //isPersistent: false for Session Cooce(Sign in Lost after browser close) & isPersistent: true for Permanent Cooce(Sign in also stay after browser close)
                    return RedirectToAction("index", "Home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //Part: 69.4
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }

        //Part: 70.2,  106.3
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            UserLoginViewModel model = new UserLoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        //Part: 70.4,  72
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,model.RememberMe, false); //(RememberMe) isPersistent: false for Session Cooce(Sign in Lost after browser close) & (RememberMe)isPersistent: true for Permanent Cooce(Sign in also stay after browser close)

                if (result.Succeeded)
                {
                    //Part: 72,  73
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                        //return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");                
            }
            return View(model);
        }


        //Part: 106.5
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallbak", "Account", new { ReturnUrl = returnUrl });

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        //Part: 107.1
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallbak(string returnUrl =null, string remoteError =null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            UserLoginViewModel userLoginViewModel = new UserLoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins=(await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", userLoginViewModel);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error from loading external login information");

                return View("Login", userLoginViewModel);
            }

            var signinResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);  //Check AspNetUserLogins table 

            if (signinResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else                   
            {
                var email = info.Principal.FindFirst(ClaimTypes.Email).ToString();

                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);     //search local account of this email from aspNetUsers table

                    if (user ==null)
                    {
                        user = new ApplicationUser
                        {
                            UserName =info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);        //create a Local Account
                    }
                    await userManager.AddLoginAsync(user, info);                //on AspNetUserLogins table Add a user 
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = $"Please contract support on akader080@gmail.com";

                return View("Error");
            }
        }

        //Part: 83.2
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
