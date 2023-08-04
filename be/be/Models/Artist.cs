using System;
using System.Collections.Generic;

namespace be.Models;

public partial class Artist
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public string? ShortDescription { get; set; }

    public string? Image { get; set; }

    public bool? Status { get; set; }

    public DateTime CreateAt { get; set; }
}
