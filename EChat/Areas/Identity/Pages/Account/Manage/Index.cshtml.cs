using EChat.Helpers;
using EChat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EChat.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<IndexModel> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IFileValidator _fileValidator;
    public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<IndexModel> logger, IWebHostEnvironment env, IFileValidator fileValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _env = env;
        _fileValidator = fileValidator;
    }

    public string Username { get; set; }
    public string FullName { get; set; }
    public string Avatar { get; set; }
    public string UploadAvatarErrorMessage { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    [BindProperty]
    public IFormFile AvatarFile { get; set; }

    public class InputModel
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        Username = userName;
        FullName = user.FullName;
        Avatar = user.Avatar;

        Input = new InputModel
        {
            PhoneNumber = phoneNumber
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAvatarAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        if (!_fileValidator.IsValid(AvatarFile))
        {
            UploadAvatarErrorMessage = "Invalid file.";
            await LoadAsync(user);
            return Page();
        }

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(AvatarFile.FileName);
        var uploaded = await UploadAvatarAsync(AvatarFile, fileName);
        if (uploaded)
        {
            if (!string.IsNullOrEmpty(user.Avatar))
                DeleteAvatar(user.Avatar);

            user.Avatar = fileName;
            await _userManager.UpdateAsync(user);
        }

        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAvatarAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var deleted = DeleteAvatar(user.Avatar);
        if (deleted)
        {
            user.Avatar = null;
            await _userManager.UpdateAsync(user);
        }

        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    private async Task<bool> UploadAvatarAsync(IFormFile file, string fileName)
    {
        try
        {
            var folderPath = Path.Combine(_env.WebRootPath, "avatars");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, Path.GetFileName(fileName));
            await using var fs = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fs);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading avatar: {file.FileName}.", ex.Message);
        }

        return false;
    }

    private bool DeleteAvatar(string fileName)
    {
        try
        {
            var folderPath = Path.Combine(_env.WebRootPath, "avatars");
            var filePath = Path.Combine(folderPath, Path.GetFileName(fileName));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting avatar: {fileName}.", ex.Message);
        }

        return false;
    }
}