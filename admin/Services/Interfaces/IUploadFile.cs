
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace admin.Services.Interfaces
{
    public interface IUploadFile
    {
        Task<string> UploadFile(IFormFile file, string Folder);
    }


}
