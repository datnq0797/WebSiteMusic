using System;
using System.Collections.Generic;

namespace be.Models;

public partial class Genre
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? Status { get; set; }

    public DateTime CreateAt { get; set; }
}
