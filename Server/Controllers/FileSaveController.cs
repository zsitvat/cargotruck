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

[ApiController]
[Route("[controller]")]
public class FilesaveController : ControllerBase
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<FilesaveController> logger;
    private readonly UserManager<Users> _userManager;
    private readonly ApplicationDbContext _context;
    public FilesaveController(IWebHostEnvironment env,ILogger<FilesaveController> logger, UserManager<Users> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        this.env = env;
        this.logger = logger;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<IList<UploadResult>>> PostFile(
        [FromForm] IEnumerable<IFormFile> files)
    {
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
                        var rootpath = env.ContentRootPath.Replace("\\Server", "\\Client\\") + "wwwroot";
                        trustedFileNameForFileStorage = Path.GetRandomFileName();
                        trustedFileNameForFileStorage = Path.ChangeExtension(trustedFileNameForFileStorage, ".jpg");
                        var path = Path.Combine(rootpath, "img/profiles",
                            trustedFileNameForFileStorage);
                      
                        await using FileStream fs = new(path, FileMode.Create);
                        await file.CopyToAsync(fs);

                        logger.LogInformation("{FileName} saved at {Path}",
                            trustedFileNameForDisplay, path);
                        uploadResult.Uploaded = true;
                        uploadResult.StoredFileName = trustedFileNameForFileStorage;
                        if (uploadResult.Uploaded) { 
                            var user = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
                            var Claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
                            var image = "/img/profiles/" + trustedFileNameForFileStorage;
                            await _userManager.ReplaceClaimAsync(user, new Claim("img", Claims["img"]), new Claim("img",image ));
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