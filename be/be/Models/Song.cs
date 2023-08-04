using System;
using System.Collections.Generic;

namespace be.Models;

public partial class Song
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Singer { get; set; }

    public Guid? IdGenre { get; set; }

    public string? Lyrics { get; set; }

    public string? Image { get; set; }

    public string Path { get; set; } = null!;

    public int? Duration { get; set; }

    public int? Quantity { get; set; }

    public bool? Status { get; set; }

    public DateTime CreateAt { get; set; }
}
