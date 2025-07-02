﻿namespace Unitic_BE.DTOs.Requests;

public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}