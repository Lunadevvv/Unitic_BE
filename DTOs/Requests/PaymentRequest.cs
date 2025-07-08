using Unitic_BE.Enums;

namespace Unitic_BE.DTOs.Requests;

public class PaymentRequest

{
    public required int Money { get; init; }
    public string Description { get; init; } = string.Empty;
}