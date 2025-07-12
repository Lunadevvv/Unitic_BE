
namespace Unitic_BE.Exceptions;

public class InvalidQuantityException() : Exception($"Quantity must greater than 0 and lower than total seat available.");