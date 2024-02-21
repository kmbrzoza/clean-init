using System;
using System.Threading.Tasks;
using Insig.ApplicationServices.Services;
using Insig.Infrastructure.FileProcessing.Containers;
using Insig.Infrastructure.FileProcessing.Models;
using Insig.Infrastructure.FileProcessing.Services;
using Insig.PublishedLanguage.Dtos.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Insig.Infrastructure.Services.FileServices;

public class ExampleFilesService : BaseFileService, IExampleFilesService
{
    // For this example, we skip the format validation, while the correct formats should be defined like this
    // protected override IReadOnlyList<string> AvailableFormats => new List<string>() { ".pdf", ".dot", ".doc", ".docx", ".xls", ".xlsx" }.AsReadOnly();

    public ExampleFilesService(
        IBlobService blobService,
        IConfiguration configuration
    ) : base(blobService, configuration) { }

    public async Task AddFile(IFormFile file, FileDTO resultFile)
    {
        if (file is null)
        {
            throw new ArgumentException("File is null.");
        }

        // For this example, we skip the format validation, while it should be done like this
        // EnsureCorrectFormat(file.FileName);

        var blobFile = new StreamBlobFile(file.FileName, file.OpenReadStream(), file.ContentType, file.Length);

        await Store<ExampleContainer>(blobFile);

        resultFile.FilePath = await GetAuthBlobUrl<ExampleContainer>(blobFile.FilePath);
    }
}
