namespace Unitic_BE.Exceptions;

public class UpdateAddFailedException(IEnumerable<string> errorDescriptions)
    : Exception($"Update/Add failed with following errors: {string.Join("",errorDescriptions)}"); 