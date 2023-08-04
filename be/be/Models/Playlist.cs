using System;
using System.Collections.Generic;

namespace be.Models;

public partial class Playlist
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public Guid? IdUser { get; set; }

    public DateTime CreateAt { get; set; }
}
