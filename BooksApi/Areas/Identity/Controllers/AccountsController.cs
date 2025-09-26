using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using  BooksApi.DTOs.Response;

namespace  BooksApi.Areas.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
       
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IEmailSender _emailSender;
            private readonly SignInManager<ApplicationUser> _signManager;
            private readonly IRepository<UserOTP> _userOTP;

            public AccountsController(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
                SignInManager<ApplicationUser> signManager, IRepository<UserOTP> userOTP)
            {
                _emailSender = emailSender;
                _signManager = signManager;
                _userManager = userManager;
                _userOTP = userOTP;
            }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
          
            #region way 1:

            ApplicationUser applicationUser = new()
            {
                Name = registerDTO.Name,
                Email = registerDTO.Email,
                Street = registerDTO.Street,
                City = registerDTO.City,
                State = registerDTO.State,
                Zipcode = registerDTO.Zipcode,
                UserName = registerDTO.UserName,
            };

            #endregion

            #region way 2:
            //ApplicationUser applicationUser= registerVM.Adapt<ApplicationUser>();
            #endregion

            var result = await _userManager.CreateAsync(applicationUser, registerDTO.Password);

            ModelStateDictionary keyValuePairs = new();

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    keyValuePairs.AddModelError(string.Empty, item.Description);
                }
                return BadRequest(keyValuePairs);
            }
            #region Add user to role:
            await _userManager.AddToRoleAsync(applicationUser, SD.CustomerArea);
             
            #endregion

            #region Send Confirm Msg:

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var Link = Url.Action("ConfirmEmail", "Accounts", new
            {
                area = "Identity",
                token = token
                ,
                userId = applicationUser.Id
            }, Request.Scheme);

            await _emailSender.SendEmailAsync(applicationUser.Email, "Confirm Your Email",
                $"<h1>Welcome to E-Tickets</h1>" +
                $"<p>Please confirm your email by <a href='{Link}'>Clicking here</a></p>");

            #endregion 
            
            return Ok(new
            {
                status = 200,
                message = "Registration Successful. Please confirm your email.",
                data = applicationUser
            });

        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
          
            var user = await _userManager.FindByEmailAsync(loginDTO.EmailOrUserName) ?? await _userManager.FindByNameAsync(loginDTO.EmailOrUserName);
            if (user == null)
            {
               
                return NotFound(new NotificationDTO
                {
                    Msg = "Invalid username or password",
                    TraceId= Guid.NewGuid().ToString(),
                    CreatedAT= DateTime.UtcNow
                });
            }


            var result = await _signManager.PasswordSignInAsync(user, loginDTO.Password, loginDTO.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    //ModelState.AddModelError(string.Empty, "Your account is locked out, please try again later");
                    return BadRequest(new NotificationDTO
                    {
                        Msg = "Your account is locked out, please try again later",
                        TraceId = Guid.NewGuid().ToString(),
                        CreatedAT = DateTime.UtcNow
                    });
                }
                else if (result.IsNotAllowed)
                {
                    return NotFound(new NotificationDTO
                    {
                        Msg = "You must confirm your email to login",
                        TraceId = Guid.NewGuid().ToString(),
                        CreatedAT = DateTime.UtcNow
                    });
                }
                else
                {
                   return NotFound(new NotificationDTO
                   {
                       Msg = "Invalid username or password",
                       TraceId = Guid.NewGuid().ToString(),
                       CreatedAT = DateTime.UtcNow
                   });

                };

            }
            if (!user.EmailConfirmed)
            {
                return BadRequest(new NotificationDTO
                {
                    Msg = " Confirm your Email first",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });

            }
            return Ok(new
            {
                status = 200,
                message = "Login Successful",
                data = user
            });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound(new NotificationDTO
                {
                    Msg = "Invalid username or password",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return BadRequest(new NotificationDTO
                {
                    Msg = "Link Expired!, Resend Email Confirmation",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });
        

            return Ok(new NotificationDTO
            {
                Msg = "Confirmation email sent. Please check your email.",
                TraceId = Guid.NewGuid().ToString(),
                CreatedAT = DateTime.UtcNow
            });
        }

        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(string EmailOrUserName )
        {
        
            var user = await _userManager.FindByEmailAsync(EmailOrUserName) ?? await _userManager.FindByNameAsync(EmailOrUserName);
            if (user == null)
            {
                return NotFound(new NotificationDTO
                {
                    Msg = "Invalid username or password",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });
            }

            if (user.EmailConfirmed)
            {
               BadRequest(new NotificationDTO
                {
                    Msg = "Your email is already confirmed, please login",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var Link = Url.Action("ConfirmEmail", "Account", new
            {
                area = "Identity",
                token = token
                ,
                userId = user.Id
            }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Email",
                $"<h1>Welcome to E-Tickets</h1>" +
                $"<p>Please confirm your email by <a href='{Link}'>Clicking here</a></p>");

         return Ok(new NotificationDTO
            {
                Msg = "Confirmation email sent. Please check your email.",
                TraceId = Guid.NewGuid().ToString(),
                CreatedAT = DateTime.UtcNow
            });

        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
         
            var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.EmailOrUserName) ?? await _userManager.FindByNameAsync(forgetPasswordDTO.EmailOrUserName);
            if (user == null)
            {
                return NotFound(new NotificationDTO
                {
                    Msg = "Invalid username or password",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });
            }

            var OTP = new Random().Next(1000, 9999).ToString();
            var Link = Url.Action("ResetPassword", "Account", new
            {
                area = "Identity",
                userId = user.Id
            }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, $"Reset Password!",
                $"<h1>Reset Password Using {OTP}" +
                $". Don't share it!</h1>");
            await _userOTP.CreateAsync(new()
            {
                ApplicationUserId = user.Id,
                OTPNumber = OTP,
                ValidTo = DateTime.UtcNow.AddDays(1)
            });
            await _userOTP.comitChanges();

         

            return Ok(new 
            {
                msg = "Reset Password OTP sent. Please check your email.",
                userId= user.Id
            });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        { 
            var user = await _userManager.FindByIdAsync(resetPasswordDTO.ApplicationUserId);

            if (user is null)
            {
                return NotFound(new NotificationDTO
                {
                    Msg = "Invalid username or password",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });
            }

            var userOTP = (await _userOTP.GetAllAsync(e => e.ApplicationUserId == resetPasswordDTO.ApplicationUserId)).OrderBy(e => e.Id).LastOrDefault();

            if (userOTP is null)
                return NotFound();

            if (userOTP.OTPNumber != resetPasswordDTO.OTPNumber)
            {
                return BadRequest(new NotificationDTO
                {
                    Msg = "Invalid OTP",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });

            }

            if (DateTime.UtcNow > userOTP.ValidTo)
            {
                return BadRequest(new NotificationDTO
                {
                    Msg = "Expired OTP",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });
            }

            return Ok(new
            {
                status = 200,
                message = "Success OTP",
                data = new { ApplicationUserId = user.Id }
            });
        }

        [HttpPost("NewPassword")]
        public async Task<IActionResult> NewPassword(NewPasswordDTO newPasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(newPasswordDTO.ApplicationUserId);

            if (user is null)
            {
                return NotFound(new NotificationDTO
                {
                    Msg = "Invalid username or password",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAT = DateTime.UtcNow
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, newPasswordDTO.Password);
   
            return Ok(new NotificationDTO
            {
                Msg = "Change Password Successfully!",
                TraceId = Guid.NewGuid().ToString(),
                CreatedAT = DateTime.UtcNow
            });
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            _signManager.SignOutAsync();
            return Ok();

        }




    }
}
