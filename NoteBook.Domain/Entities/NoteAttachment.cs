namespace NoteBook.Domain.Entities;

/// <summary>
/// Represents an attachment (file) on a note.
/// Supports images, documents, and other file types.
/// </summary>
public class NoteAttachment : BaseEntity
{
    /// <summary>
    /// The note this attachment belongs to
    /// </summary>
    public Guid NoteId { get; set; }
    
    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// File MIME type (image/png, application/pdf, etc.)
    /// </summary>
    public string MimeType { get; set; } = string.Empty;
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }
    
    /// <summary>
    /// Storage path or URL where file is stored
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional description of the attachment
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// For images: width in pixels
    /// </summary>
    public int? ImageWidth { get; set; }
    
    /// <summary>
    /// For images: height in pixels
    /// </summary>
    public int? ImageHeight { get; set; }
    
    /// <summary>
    /// Thumbnail path for images
    /// </summary>
    public string? ThumbnailPath { get; set; }
    
    /// <summary>
    /// Number of times this attachment was downloaded
    /// </summary>
    public int DownloadCount { get; set; }
    
    /// <summary>
    /// Is this attachment marked for deletion
    /// </summary>
    public bool IsMarkedForDeletion { get; set; }
    
    /// <summary>
    /// Check if attachment is an image
    /// </summary>
    public bool IsImage => MimeType.StartsWith("image/");
    
    /// <summary>
    /// Check if attachment is a document
    /// </summary>
    public bool IsDocument => MimeType.StartsWith("application/pdf") || 
                              MimeType.StartsWith("application/msword") ||
                              MimeType.Contains("spreadsheet");
    
    /// <summary>
    /// Increment download count
    /// </summary>
    public void IncrementDownloadCount()
    {
        DownloadCount++;
    }
    
    /// <summary>
    /// Mark attachment for deletion
    /// </summary>
    public void MarkForDeletion(Guid markedBy)
    {
        IsMarkedForDeletion = true;
        UpdateAudit(markedBy);
    }
}
