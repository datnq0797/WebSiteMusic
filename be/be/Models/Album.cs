using System;
using System.Collections.Generic;

namespace be.Models;

public partial class Album
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Singer { get; set; }

    public string? Image { get; set; }

    public Guid? IdGenre { get; set; }

    public DateTime? CreateAt { get; set; }
}
