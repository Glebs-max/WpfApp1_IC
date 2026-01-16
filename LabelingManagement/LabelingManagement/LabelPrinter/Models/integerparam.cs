using System;
using System.Collections.Generic;

namespace LabelPrinter.Models;

public partial class integerparam
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ulong Value { get; set; }
}
