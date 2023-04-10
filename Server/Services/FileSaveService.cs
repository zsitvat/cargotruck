using Cargotruck.Server.Data;
using Cargotruck.Server.Models;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace Cargotruck.Server.Services
{
    public class FileSaveService : IFileSaveService
    {
        private readonly IWebHostEnvironment env;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<Resource> _localizer;

        public FileSaveService(IWebHostEnvironment env, UserManager<User> userManager, ApplicationDbContext context, SignInManager<User> signInManager, IStringLocalizer<Resource> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.env = env;
            _context = context;
            _localizer = localizer;
        }

        public async Task<dynamic> PostFileAsync([FromForm] IEnumerable<IFormFile> files, string id, CultureInfo lang)
        {
            CultureInfo.CurrentUICulture = lang;
            string? path = null;
            var maxAllowedFiles = 1;
            long maxFileSize = 1024 * 5000;
            var filesProcessed = 0;
            //var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<UploadResult> uploadResults = new();

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        return _localizer["Empty_file"].Value + trustedFileNameForDisplay;
                    }
                    else if (file.Length > maxFileSize)
                    {

                        return _localizer["Empty_file"].Value + " " + trustedFileNameForDisplay + "." + _localizer["File_lenght_is"].Value + " " + file.Length + _localizer["File_max_lenght"].Value + " " + maxFileSize;
                    }
                    else
                    {
                        try
                        {
                            string rootpath, folder;

                            //file upload part
                            if (id == "page")
                            {
                                rootpath = env.ContentRootPath;
                                var extension = Path.GetExtension(uploadResult.FileName);
                                trustedFileNameForFileStorage = Path.GetRandomFileName();
                                trustedFileNameForFileStorage = Path.ChangeExtension(trustedFileNameForFileStorage, extension);
                                folder = "Files/";
                            }
                            // image upload part
                            else if (uploadResult.FileName.Contains(".jpg") || uploadResult.FileName.Contains(".jpeg") || uploadResult.FileName.Contains(".png") || uploadResult.FileName.Contains(".webp"))
                            {
                                rootpath = env.ContentRootPath.Replace("\\Server", "\\Client\\") + "wwwroot";
                                trustedFileNameForFileStorage = Path.GetRandomFileName();
                                trustedFileNameForFileStorage = Path.ChangeExtension(trustedFileNameForFileStorage, ".jpg");
                                folder = "img/profiles";

                            }
                            else
                            {
                                return _localizer["Not_an_image"].Value + " " + uploadResult.FileName;
                            }

                            var image = Path.Combine(folder, trustedFileNameForFileStorage);
                            path = Path.Combine(rootpath, folder, trustedFileNameForFileStorage);

                            await using FileStream fs = new(path, FileMode.Create);
                            await file.CopyToAsync(fs);

                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;

                            //image upload part
                            if (id != "page")
                            {
                                
                                User? user = new();
                                string img = "";

                                //change the image
                                if (uploadResult.Uploaded)
                                {
                                    //get the old image
                                    if (id == "NoId")
                                    {

                                        user = _context.Users.FirstOrDefault(a => a.UserName == _signInManager.Context.User.Identity!.Name);
                                        if (user != null)
                                        {
                                            img = _context.UserClaims.FirstOrDefault(c => c.UserId == user.Id && c.ClaimType == "img")?.ClaimValue ?? "";
                                        }
                                    }
                                    else
                                    {
                                        user = _context.Users.FirstOrDefault(a => a.Id == id);
                                        if (user != null)
                                        {
                                            img = _context.UserClaims.FirstOrDefault(c => c.UserId == id && c.ClaimType == "img")?.ClaimValue ?? "";
                                        }
                                    }

                                    await _userManager.ReplaceClaimAsync(user!, new Claim("img", img), new Claim("img", image));

                                    var deleteold = Path.Combine(rootpath, img);

                                    //delete the old one
                                    if (System.IO.File.Exists(deleteold) && deleteold.Contains("/profiles/"))
                                    {
                                        System.IO.File.Delete(deleteold);
                                    }
                                    if (id == "NoId")
                                    {
                                        await _signInManager.RefreshSignInAsync(user!);
                                    }
                                }
                            }

                        }
                        catch (IOException ex)
                        {
                            return _localizer["Error_on_upload"].Value + " " + ex.Message;
                        }
                    }
                    filesProcessed++;
                }
                else
                {
                    return _localizer["Max_file_number"].Value + " " + maxAllowedFiles;
                }

                uploadResults.Add(uploadResult);
            }

            return new CreatedResult(path!, uploadResults);
        }
    }
}
