using System;
using System.Collections.Generic;

namespace LabelPrinter.Models;

public partial class versioninfo
{
    public long Version { get; set; }

    public DateTime? AppliedOn { get; set; }

    public string? Description { get; set; }
}
