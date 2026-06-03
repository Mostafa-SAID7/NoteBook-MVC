namespace NoteBook.Domain.ValueObjects;

using System.Text.RegularExpressions;

/// <summary>
/// Value object representing an email address.
/// Immutable and self-validating.
/// </summary>
public class Email : IEquatable<Email>
{
    /// <summary>
    /// The email value
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Basic email validation pattern
    /// </summary>
    private static readonly Regex EmailPattern = new(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    
    private const int MaxLength = 254; // RFC 5321
    
    private Email(string value)
    {
        Value = value.ToLowerInvariant();
    }
    
    /// <summary>
    /// Create an email value object
    /// </summary>
    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure("Email cannot be empty");
        
        value = value.Trim();
        
        if (value.Length > MaxLength)
            return Result<Email>.Failure($"Email cannot exceed {MaxLength} characters");
        
        if (!EmailPattern.IsMatch(value))
            return Result<Email>.Failure("Invalid email format");
        
        return Result<Email>.Success(new Email(value));
    }
    
    /// <summary>
    /// Get local part (before @)
    /// </summary>
    public string GetLocalPart() => Value.Split('@')[0];
    
    /// <summary>
    /// Get domain part (after @)
    /// </summary>
    public string GetDomain() => Value.Split('@')[1];
    
    public override string ToString() => Value;
    
    public override bool Equals(object? obj) => Equals(obj as Email);
    
    public bool Equals(Email? other) => other?.Value == Value;
    
    public override int GetHashCode() => Value.GetHashCode();
    
    public static bool operator ==(Email? left, Email? right) => left?.Equals(right) ?? right is null;
    
    public static bool operator !=(Email? left, Email? right) => !(left == right);
}
