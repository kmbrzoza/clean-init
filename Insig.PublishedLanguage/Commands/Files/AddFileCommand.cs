using Insig.PublishedLanguage.Dtos.Files;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Insig.PublishedLanguage.Commands.Files;

public class AddFileCommand : IRequest
{
    public IFormFile FileToAdd { get; set; }

    // out
    public FileDTO Result { get; set; }
}
