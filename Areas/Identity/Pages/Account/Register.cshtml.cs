using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shops.Data;
using Shops.Models;
using Shops.Utility;

namespace Shops.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
       

        public RegisterModel(
           ApplicationDbContext applicationDbContext ,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _dbContext = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [Display(Name = "Roles")]
        [BindProperty]
        public string RoleName { get; set; }
        public IList<IdentityRole> roles { get; set; }

        [Display(Name = "Human Class")]
        [BindProperty]
        public int HumanClassId { get; set; }
        public IList<HumanClass> HumanClass { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            [StringLength(60)]
            public string FirstName { get; set; }
            [Required]
            [StringLength(60)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [StringLength(11,ErrorMessage ="Phone Number must have 11 number")]
            public string PhoneNumber { get; set; }
            public byte[]? Image { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            await GetDataPage();
             ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
           
            var file = Request.Form.Files;
            if (!file.Any())
            {
                await GetDataPage();
                ModelState.AddModelError("Image", "No Image Select");
                return Page();
            }

            List<string> extentions = new List<string> { ".png", ".jpg" };
            string ExtentionFile = Path.GetExtension(file[0].FileName.ToLower());

            if (!extentions.Contains(ExtentionFile))
            {
                await GetDataPage();
                ModelState.AddModelError("Image", "Image must have only extention of png / jpg");
                return Page();
            }
            
            MemoryStream stream = new MemoryStream();
            file[0].CopyTo(stream);
            if (ModelState.IsValid)
            {
                if (await _userManager.Users.AnyAsync(u => u.Email == Input.Email))
                {
                    await GetDataPage();
                    ModelState.AddModelError("Email", "Email Exist . .!");
                    return Page();
                };
                
                if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == Input.PhoneNumber))
                {
                    await GetDataPage();
                    ModelState.AddModelError("PhoneNumber", "PhoneNumber Exist . .!");
                    return Page();
                }
              
                var user = new ApplicationUser {
                    UserName = new MailAddress(Input.Email).User,
                    Email = Input.Email.Trim(),
                    FirstName = Input.FirstName.Trim(),
                    LastName = Input.LastName.Trim(),
                    Image = stream.ToArray(),
                    PhoneNumber =Input.PhoneNumber,
                   HumanClassId =HumanClassId,
                 };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                      
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                   
                    var appUser = await _dbContext.ApplicationUsers.SingleOrDefaultAsync(a => a.Email == user.Email);

                    if (!_signInManager.IsSignedIn(User) && _dbContext.ApplicationUsers.Count()==1)
                    {
                        if (!await _dbContext.Roles.AnyAsync(r => r.Name == RoleUtilty.Admin))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(RoleUtilty.Admin));
                        }
                        var roleDefualt = await _userManager.AddToRoleAsync(appUser, RoleUtilty.Admin);
                        if (!roleDefualt.Succeeded)
                        {
                            await GetDataPage();
                            ModelState.AddModelError("RoleId", "user already has this role! ");
                            return Page();
                        }
                       
                    }
                    if (!_signInManager.IsSignedIn(User) && _dbContext.ApplicationUsers.Count() > 1)
                    {
                        if(! await _dbContext.Roles.AnyAsync(r => r.Name == RoleUtilty.Customer))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(RoleUtilty.Customer));
                        }
                        var roleDefualt = await _userManager.AddToRoleAsync(appUser, RoleUtilty.Customer);
                        if (!roleDefualt.Succeeded)
                        {
                            await GetDataPage();
                            ModelState.AddModelError("RoleId", "user already has this role! ");
                            return Page();
                        }
                        
                    }
                  
                    if (_signInManager.IsSignedIn(User) && User.IsInRole(RoleUtilty.Admin))
                    {

                        var role = await _userManager.AddToRoleAsync(user, RoleName);
                        if (!role.Succeeded)
                        {
                            await GetDataPage();
                            ModelState.AddModelError("RoleId", "user already has this role! ");
                            return Page();
                        }
                        return LocalRedirect("~/Customer");
                    }
                    
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if(!_signInManager.IsSignedIn(User)&& !User.IsInRole(RoleUtilty.Admin))
                        {
                         await _signInManager.SignInAsync(user, isPersistent: false);
                        }
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    await GetDataPage();
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        private async Task GetDataPage()
        {
            roles = await _roleManager.Roles.ToListAsync();
            HumanClass = await _dbContext.HumanClass.ToListAsync();
        }
    }
  
}
