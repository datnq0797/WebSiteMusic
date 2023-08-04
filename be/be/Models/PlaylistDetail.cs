using System;
using System.Collections.Generic;

namespace be.Models;

public partial class PlaylistDetail
{
    public Guid Id { get; set; }

    public Guid IdPlaylist { get; set; }

    public Guid IdSong { get; set; }
}
