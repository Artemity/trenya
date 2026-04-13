using System;
using System.Collections.Generic;

namespace экзамка.DB;

public partial class EthnicComposition
{
    public int CompositionId { get; set; }

    public int CountryCode { get; set; }

    public int LanguageCode { get; set; }

    public int SpeakersCount { get; set; }

    public virtual Country CountryCodeNavigation { get; set; } = null!;

    public virtual Language LanguageCodeNavigation { get; set; } = null!;
}
