namespace Unitic_BE.Exceptions;

public class RegistrationFailedException(IEnumerable<string> errorDescriptions)
    : Exception($"Registration failed with following errors: {string.Join(", ", errorDescriptions)}"); 