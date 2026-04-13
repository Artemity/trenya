using System;
using System.Collections.Generic;

namespace экзамка.DB;

public partial class Country
{
    public int CountryCode { get; set; }

    public string CountryName { get; set; } = null!;

    public string Continent { get; set; } = null!;

    public string Capital { get; set; } = null!;

    public int Population { get; set; }

    public virtual ICollection<EthnicComposition> EthnicCompositions { get; set; } = new List<EthnicComposition>();
}
