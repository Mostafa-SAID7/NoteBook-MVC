namespace NoteBook.Domain.ValueObjects;

using System.Text.RegularExpressions;

/// <summary>
/// Value object representing a URL-friendly slug.
/// Immutable and self-validating.
/// </summary>
public class Slug : IEquatable<Slug>
{
    /// <summary>
    /// The slug value (URL-safe string)
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Slug length limits
    /// </summary>
    private const int MinLength = 3;
    private const int MaxLength = 100;
    
    /// <summary>
    /// Valid slug pattern: lowercase letters, numbers, hyphens only
    /// </summary>
    private static readonly Regex ValidSlugPattern = new(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);
    
    private Slug(string value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Create a slug from a string (title, name, etc.)
    /// Automatically converts to lowercase and replaces spaces with hyphens
    /// </summary>
    public static Result<Slug> CreateFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result<Slug>.Failure("Slug text cannot be empty");
        
        // Convert to lowercase and replace spaces/special chars with hyphens
        var slug = ConvertToSlug(text);
        
        if (string.IsNullOrWhiteSpace(slug))
            return Result<Slug>.Failure("Text cannot be converted to valid slug");
        
        return Create(slug);
    }
    
    /// <summary>
    /// Create a slug from a pre-formatted slug value
    /// </summary>
    public static Result<Slug> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Slug>.Failure("Slug cannot be empty");
        
        if (value.Length < MinLength)
            return Result<Slug>.Failure($"Slug must be at least {MinLength} characters");
        
        if (value.Length > MaxLength)
            return Result<Slug>.Failure($"Slug cannot exceed {MaxLength} characters");
        
        if (!ValidSlugPattern.IsMatch(value))
            return Result<Slug>.Failure("Slug can only contain lowercase letters, numbers, and hyphens");
        
        return Result<Slug>.Success(new Slug(value));
    }
    
    /// <summary>
    /// Convert text to slug format
    /// Removes special characters, converts to lowercase, replaces spaces with hyphens
    /// </summary>
    private static string ConvertToSlug(string text)
    {
        // Convert to lowercase
        var slug = text.ToLowerInvariant();
        
        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");
        
        // Remove special characters except hyphens
        slug = Regex.Replace(slug, @"[^\w\-]", "");
        
        // Replace multiple hyphens with single hyphen
        slug = Regex.Replace(slug, @"\-+", "-");
        
        // Trim hyphens from start and end
        slug = slug.Trim('-');
        
        return slug;
    }
    
    /// <summary>
    /// Generate a unique slug by appending a number if needed
    /// Useful for handling slug conflicts
    /// </summary>
    public static Result<Slug> CreateUnique(string text, Func<string, bool> slugExists)
    {
        var baseResult = CreateFromText(text);
        if (!baseResult.IsSuccess)
            return baseResult;
        
        var slug = baseResult.Value.Value;
        var counter = 1;
        var originalSlug = slug;
        
        while (slugExists(slug) && counter < 1000)
        {
            slug = $"{originalSlug}-{counter}";
            counter++;
        }
        
        return Create(slug);
    }
    
    public override string ToString() => Value;
    
    public override bool Equals(object? obj) => Equals(obj as Slug);
    
    public bool Equals(Slug? other) => other?.Value == Value;
    
    public override int GetHashCode() => Value.GetHashCode();
    
    public static bool operator ==(Slug? left, Slug? right) => left?.Equals(right) ?? right is null;
    
    public static bool operator !=(Slug? left, Slug? right) => !(left == right);
}
