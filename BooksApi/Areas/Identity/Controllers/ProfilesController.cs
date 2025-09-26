using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Numerics;
using  BooksApi.DTOs.Response;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc;

namespace  BooksApi.Areas.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var updateUser = user.Adapt<UpdatePersonalInfoResponse>();

            return Ok(updateUser);  
        }

        [HttpPost("UpdateInfo")]
        public async Task<IActionResult> UpdateInfo(UpdatePersonalInfoDTO updatePersonalInfoDTO)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            user.Name = updatePersonalInfoDTO.Name;
            user.Email = updatePersonalInfoDTO.Email;
            user.PhoneNumber = updatePersonalInfoDTO.PhoneNumber;
            user.Street = updatePersonalInfoDTO.Street;
            user.State = updatePersonalInfoDTO.State;
            user.City = updatePersonalInfoDTO.City;
            user.Zipcode = updatePersonalInfoDTO.Zipcode;
            await _userManager.UpdateAsync(user); //=save changes
                                                  

            return NoContent();
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(); 

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
              return BadRequest(result.Errors.Select(e => e.Description));


            }
            
            return NoContent();
        }

        [HttpPost("UpdateProfileImage")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile ProfileImage)
        {
            if (ProfileImage == null || ProfileImage.Length == 0)
            {
                return BadRequest(new
                {
                    msg= "Please select a valid image.",
                });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Profile");

            var fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(ProfileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }

            user.ProfileImage = $"{fileName}";
            await _userManager.UpdateAsync(user);


            return NoContent();

        }
    }

}
