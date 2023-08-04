using System;
using System.Collections.Generic;

namespace be.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public bool? Gender { get; set; }

    public string? Image { get; set; }

    public string? Phone { get; set; }

    public Guid? IdRole { get; set; }

    public bool Status { get; set; }

    public DateTime CreateAt { get; set; }
}
