using admin.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Services.Implementations
{
    public class UploadFile : IUploadFile
    {
        //1-Properties:
        readonly IWebHostEnvironment _webHostEnvironment;

        //2-Constructor:
        //with dependency injection for the IWebHostEnvironment
        public UploadFile(IWebHostEnvironment webHostEnvironment) => _webHostEnvironment = webHostEnvironment;

        //3-implement the IUploadFile interface method:
        async Task<string> IUploadFile.UploadFile(IFormFile file, string Folder)
        {
            try
            {
                //if there is no file has been selected, or the selected file is not an image:
                if (file is null || !file.ContentType.Contains("image"))
                    return null;

                //now create the destination folder path:
                //UploadFolder = F:\3-Courses And Books\8-Web Developement\4-C# ASP .NetCore MVC\Lecture 7\MyProject Embed a Template\MyLecture5\MyLecture5-Project1\wwwroot\DataContent\{Folder}
                //where "Folder" is the BlogImages and will be passed through the parameter above when we call this function.
                //string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"DataContent/{Folder}");
                //but we are uploading the pictures to another directory in another project "skinet" app, so:
                //(before real deployment, I will change the path to /var/.... where the my "skinet" app is deployed)
                string uploadFolder = Path.Combine("F:\\3-Courses And Books\\8-Web Developement\\4-ASP .NetCore MVC\\3-Project 3 - eCommerce with Angular and APIs\\skinet\\API\\Content\\", $"{Folder}"); ;

                //C# windows App uses \, but unfortunately C# ASP uses / so we should replace it in the above path:
                uploadFolder = uploadFolder.Replace(@"\", "/");

                //if any of the folders in the previous path is not existed, create it and create its sub directories:
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                //get the uploaded file name.extenstion
                var fileExtention = Path.GetExtension(file.FileName);

                //assign a random Id as a suffix for the name of the file, so we can protect duplicated files from getting replaced
                var fileName = $"{Guid.NewGuid().ToString("N")}{fileExtention}";

                //create the final path/name.extension: 
                //F:/3-Courses And Books/8-Web Developement/4-C# ASP .NetCore MVC/Lecture 7/MyProject Embed a Template/MyLecture5/MyLecture5-Project1/wwwroot/DataContent/{Folder}/<filename>.extension
                string filePath = Path.Combine(uploadFolder, fileName);

                //if we do not want to use the full path, we can use a relevant path so we can move the project easily to anywhere:
                //filePath = filePath.Replace(_webHostEnvironment.WebRootPath, "~/");
                //the path will be: ~/DataContent/{Folder}/<filename>.extension where ~/ will take us to the wwwroot folder

                //now upload the file using the concept of FileStream (dividing the files into multiple packets):
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                
                //after the upload is done, now return a simple path for the picture to be stored in a DB table:
                string filepath_toDB = Path.Combine($"{Folder}/", fileName);
                //now return the filepath to store later in the DB table 
                filepath_toDB = filepath_toDB.Replace(@"\", "/");
                return filepath_toDB;
            }
            catch
            {
                return null;
            }
        }

        

    }
}
