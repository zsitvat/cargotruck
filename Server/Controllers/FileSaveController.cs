using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cargotruck.Shared.Models;
using Cargotruck.Server.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Cargotruck.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Globalization;
using Cargotruck.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesaveController : ControllerBase
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<FilesaveController> logger;
    private readonly UserManager<Users> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<Users> _signInManager;
    public FilesaveController(IWebHostEnvironment env,ILogger<FilesaveController> logger, UserManager<Users> userManager, ApplicationDbContext context, SignInManager<Users> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        this.env = env;
        this.logger = logger;
        _context = context;
    }

    [HttpPost("{id}"), HttpPost]
    public async Task<ActionResult<IList<UploadResult>>> PostFile([FromForm] IEnumerable<IFormFile> files, string id)
    {
        string? path = null;
        var maxAllowedFiles = 1;
        long maxFileSize = 1024 * 3000;
        var filesProcessed = 0;
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
        List<UploadResult> uploadResults = new();

        foreach (var file in files)
        {
            var uploadResult = new UploadResult();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay =
                WebUtility.HtmlEncode(untrustedFileName);

            if (filesProcessed < maxAllowedFiles)
            {
                if (file.Length == 0)
                {
                    logger.LogInformation("{FileName} length is 0 (Err: 1)",
                        trustedFileNameForDisplay);
                    uploadResult.ErrorCode = 1;
                }
                else if (file.Length > maxFileSize)
                {
                    logger.LogInformation("{FileName} of {Length} bytes is " +
                        "larger than the limit of {Limit} bytes (Err: 2)",
                        trustedFileNameForDisplay, file.Length, maxFileSize);
                    uploadResult.ErrorCode = 2;
                }
                else
                {
                    try
                    {
                        string rootpath,folder;

                        //data upload part
                        if ( id=="page") 
                        { 
                            rootpath = env.ContentRootPath + "";
                            trustedFileNameForFileStorage = Path.GetRandomFileName();
                            trustedFileNameForFileStorage = Path.ChangeExtension(trustedFileNameForFileStorage, ".xlsx");
                            folder = "Files/";
                        }
                        // image upload part
                        else
                        {
                            rootpath = env.ContentRootPath.Replace("\\Server", "\\Client\\") + "wwwroot"; 
                            trustedFileNameForFileStorage = Path.GetRandomFileName();
                            trustedFileNameForFileStorage = Path.ChangeExtension(trustedFileNameForFileStorage, ".jpg");
                            folder = "img/profiles";
                           
                        }
                        var image = folder + "/" + trustedFileNameForFileStorage;
                        path = Path.Combine(rootpath, folder ,trustedFileNameForFileStorage);
                        await using FileStream fs = new(path, FileMode.Create);
                        await file.CopyToAsync(fs);

                        logger.LogInformation("{FileName} saved at {Path}",trustedFileNameForDisplay, path);
                        uploadResult.Uploaded = true;
                        uploadResult.StoredFileName = trustedFileNameForFileStorage;

                        //data upload part
                        if (id != "page")
                        {
                            Dictionary<string, string> claims = new Dictionary<string, string>();
                            var user = new Users();
                            string img = "";

                            //change the image
                            if (uploadResult.Uploaded)
                            {
                                if (id == "NoId")
                                {
                                    user = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
                                    claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
                                    img = claims["img"];
                                }
                                else
                                {
                                    user = _context.Users.FirstOrDefault(a => a.Id == id);
                                    claims = _context.UserClaims.ToDictionary(c => c.UserId, c => c.ClaimValue);
                                    img = claims[id];
                                }

                                await _userManager.ReplaceClaimAsync(user, new Claim("img", img), new Claim("img", image));

                                var deleteold = rootpath + img;

                                //delete the old one
                                if (System.IO.File.Exists(deleteold))
                                {
                                    System.IO.File.Delete(deleteold);
                                }
                                if (id == "NoId")
                                {
                                    await _signInManager.RefreshSignInAsync(user);
                                }
                            }
                        }
                       
                    }
                    catch (IOException ex)
                    {
                        logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                            trustedFileNameForDisplay, ex.Message);
                        uploadResult.ErrorCode = 3;
                    }
                }
                filesProcessed++;
            }
            else
            {
                logger.LogInformation("{FileName} not uploaded because the " +
                    "request exceeded the allowed {Count} of files (Err: 4)",
                    trustedFileNameForDisplay, maxAllowedFiles);
                uploadResult.ErrorCode = 4;
            }

            uploadResults.Add(uploadResult);
        }

        return new CreatedResult(resourcePath, uploadResults);
    }


}