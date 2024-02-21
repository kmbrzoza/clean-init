using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Services;
using Insig.PublishedLanguage.Commands.Files;
using Insig.PublishedLanguage.Dtos.Files;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Files;

public class AddFileHandler : IRequestHandler<AddFileCommand>
{
    private readonly IExampleFilesService _exampleFilesService;

    public AddFileHandler(IExampleFilesService exampleFilesService)
    {
        _exampleFilesService = exampleFilesService;
    }

    public async Task Handle(AddFileCommand request, CancellationToken cancellationToken)
    {
        request.Result = new FileDTO();

        await _exampleFilesService.AddFile(request.FileToAdd, request.Result);
    }
}
