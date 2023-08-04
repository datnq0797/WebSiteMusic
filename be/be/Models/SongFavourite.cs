using System;
using System.Collections.Generic;

namespace be.Models;

public partial class SongFavourite
{
    public Guid Id { get; set; }

    public Guid IdSong { get; set; }

    public Guid IdUser { get; set; }

    public bool? Status { get; set; }

    public DateTime CreateAt { get; set; }
}
