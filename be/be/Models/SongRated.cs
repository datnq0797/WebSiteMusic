using System;
using System.Collections.Generic;

namespace be.Models;

public partial class SongRated
{
    public Guid Id { get; set; }

    public Guid IdSong { get; set; }

    public Guid IdUser { get; set; }

    public int? Rate { get; set; }

    public DateTime CreateAt { get; set; }
}
