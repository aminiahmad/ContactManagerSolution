using ContactManager.Core.Domain.IdentityEntities;
using ContactManager.Core.DTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.Enums;

namespace ContactManager.UI.Controllers
{
    //all person see this page , not required login - 
    //[AllowAnonymous]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        //create update delete find user with UserMANAGER

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole>  _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }



        //اگر کاربر لاگین کرد دوباره به صفحه لاگین و ثبت نام نتونه دسترسی داشته باشه
        [Authorize("NotAuthorized")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        

        [HttpPost]
        //اگر کاربر لاگین کرد دوباره به صفحه لاگین و ثبت نام نتونه دسترسی داشته باشه
        [Authorize("NotAuthorized")] 
        //اگر کاربر روی لینک های مخرب کلیک کرد و درخواست به سرور داد برای هک یا خالی کردن حساب
        // یک توکن AntiForgery (cookie token + form token) form token == <input type="hidden" va .... />
        //این توکن تو هر رفرش اپدیت میشه و با قبلی فرق میکنه
        // در اخر توکن صادر شده با توکن کاربر بر اساس (setionId + salt) == که برای انتی هست دوباره چک میشه
        // اگر از یه سایت دیگه بود که form cookie رو نداره و درخواست رد میشه ارور400 
        //[ValidateAntiForgeryToken]
        //  به جای این برای تمام درخواست های پست یه فیلتر اضافه میکنیم تو program => AddControllersWithViews سرویس های     
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(error=>error.ErrorMessage);
                return View(registerDto);
            }

            ApplicationUser user = new ApplicationUser(){Email = registerDto.Email,UserName = registerDto.Email
                ,PhoneNumber = registerDto.Phone,PersonName = registerDto.PersonName};

            // create automate password to password hash in DB , create automate Repository , create automate sql code 
            IdentityResult result =  await _userManager.CreateAsync(user,registerDto.Password!);

            if (result.Succeeded)
            {
                //Check status of Radio Button 
                if (registerDto.Roles == UserRoles.Admin)
                {
                    //create admin role
                    if (await _roleManager.FindByNameAsync(UserRoles.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UserRoles.Admin.ToString() };
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
                }

                else
                {
                    //create user role
                    if (await _roleManager.FindByNameAsync(UserRoles.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UserRoles.User.ToString() };
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());
                }

                // for check sign in user and create cookie for user (isPersistent if equal to true , cookie don't delete and save then close browser but equals to false when close browser when go to page create new cookie )
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(PersonsController.Index),"Persons");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register",error.Description);
                }

                return View(registerDto);
            }
        }


        [HttpGet]
        //اگر کاربر لاگین کرد دوباره به صفحه لاگین و ثبت نام نتونه دسترسی داشته باشه
        [Authorize("NotAuthorized")]
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        //اگر کاربر لاگین کرد دوباره به صفحه لاگین و ثبت نام نتونه دسترسی داشته باشه
        [Authorize("NotAuthorized")]
        public async Task<IActionResult> Login(LoginDTO loginDto,string? ReturnUrl )
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors=ModelState.Values.SelectMany(val => val.Errors).Select(error => error.ErrorMessage);
                return View(loginDto);
            }

            if (loginDto is { Password: not null, Email: not null })
            {
                //lockoutOnFailure => If user only mistake to enter parameter for login , if lockoutOnFailure is true
                //, this user is locked and not again to enter parameter to login

                var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: false,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    ApplicationUser? user = await _userManager.FindByEmailAsync(loginDto.Email);
                    if (user != null)
                    {
                        if (await _userManager.IsInRoleAsync(user, UserRoles.Admin.ToString()))
                        {
                           return RedirectToAction("Index", "Home", new { area="Admin"});
                        }
                    }
                    // isLocalUrl and LocalRedirect be security of hacks and don't send any post request to other url
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    return RedirectToAction(nameof(PersonsController.Index), "Persons");
                }
            }

            ModelState.AddModelError("Login",errorMessage:"user name or password is incorrect");
            return View(loginDto);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string? email)
        {
            var user= await _userManager.FindByEmailAsync(email!);
            return Json(user == null);
        }
    }
}
