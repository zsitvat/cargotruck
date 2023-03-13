using Cargotruck.Data;
using Cargotruck.Server.Models;
using Cargotruck.Shared.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Cargotruck.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesaveController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly UserManager<Users> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<Users> _signInManager;
        public FilesaveController(IWebHostEnvironment env, UserManager<Users> userManager, ApplicationDbContext context, SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.env = env;
            _context = context;
        }

        [HttpPost("{id}"), HttpPost]
        public async Task<ActionResult<IList<UploadResult>>> PostFile([FromForm] IEnumerable<IFormFile> files, string id, string? lang)
        {
            string? path = null;
            var maxAllowedFiles = 1;
            long maxFileSize = 1024 * 5000;
            var filesProcessed = 0;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
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
                        return BadRequest(lang == "hu" ? $"{trustedFileNameForDisplay} hossza 0." : $"{trustedFileNameForDisplay} length is 0.");
                    }
                    else if (file.Length > maxFileSize)
                    {

                        return BadRequest(lang == "hu" ? $"{trustedFileNameForDisplay} hossza {file.Length} bájt, mely túllépi a megengedett {maxFileSize} bájtot." : $"{trustedFileNameForDisplay} of {file.Length} bytes is larger than the limit of {maxFileSize} bytes.");
                    }
                    else
                    {
                        try
                        {
                            string rootpath, folder;

                            //file upload part
                            if (id == "page")
                            {
                                rootpath = env.ContentRootPath + "";
                                trustedFileNameForFileStorage = Path.GetRandomFileName();
                                trustedFileNameForFileStorage = Path.ChangeExtension(trustedFileNameForFileStorage, ".xlsx");
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
                                return BadRequest(lang == "hu" ? $"{uploadResult.FileName} nem egy kép." : $"{uploadResult.FileName} is not an image.");
                            }
                            var image = folder + "/" + trustedFileNameForFileStorage;
                            path = Path.Combine(rootpath, folder, trustedFileNameForFileStorage);
                            await using FileStream fs = new(path, FileMode.Create);
                            await file.CopyToAsync(fs);

                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;

                            //image upload part
                            if (id != "page")
                            {
                                Dictionary<string, string> claims = new();
                                Users? user = new();
                                string img = "";

                                //change the image
                                if (uploadResult.Uploaded)
                                {
                                    //get the old image
                                    if (id == "NoId")
                                    {

                                        user = _context.Users.FirstOrDefault(a => a.UserName == User.Identity!.Name);

                                        claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
                                        img = claims["img"];
                                    }
                                    else
                                    {
                                        user = _context.Users.FirstOrDefault(a => a.Id == id);
                                        claims = _context.UserClaims.ToDictionary(c => c.UserId, c => c.ClaimValue);
                                        img = claims[id];
                                    }

                                    await _userManager.ReplaceClaimAsync(user!, new Claim("img", img), new Claim("img", image));

                                    var deleteold = rootpath + img;

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
                            return BadRequest(lang == "hu" ? $"{trustedFileNameForDisplay} hiba a feltöltésnél: {ex.Message}" : $"{trustedFileNameForDisplay} error on upload: {ex.Message}");
                        }
                    }
                    filesProcessed++;
                }
                else
                {
                    return BadRequest(lang == "hu" ? $"{trustedFileNameForDisplay} a feltöltés nem sikerült, mert a hivás túllépte a  {maxAllowedFiles} darab fájlt." : $"{trustedFileNameForDisplay} not uploaded because the request exceeded the allowed {maxAllowedFiles} of files.");
                }

                uploadResults.Add(uploadResult);
            }

            return new CreatedResult(resourcePath, uploadResults);
        }
    }
}