using System.Threading.Tasks;
using Insig.PublishedLanguage.Dtos.Files;
using Microsoft.AspNetCore.Http;

namespace Insig.ApplicationServices.Services;

public interface IExampleFilesService
{
    Task AddFile(IFormFile file, FileDTO resultFile);
}
