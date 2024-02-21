using System;
using System.Text.RegularExpressions;

namespace Insig.Infrastructure.FileProcessing.Helpers;

public class FileProcessingHelper
{
    public static string GetSanitizedBase64(string imageAsBase64) => GetEncodedBase64Regex(imageAsBase64).Groups["data"].Value;

    public static string GetDecodedBase64String(string contentType, byte[] imageBytes) =>
        $"data:{contentType};base64,{Convert.ToBase64String(imageBytes)}";

    public static string GetSanitizedBase64FileType(string imageAsBase64) => GetEncodedBase64Regex(imageAsBase64).Groups["type"].Value;

    private static Match GetEncodedBase64Regex(string imageAsBase64) =>
        Regex.Match(imageAsBase64, @"data:image/(?<type>.+?);base64,(?<data>.+)");
}
