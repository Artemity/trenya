using System;
using System.Collections.Generic;

namespace экзамка.DB;

public partial class Language
{
    public int LanguageCode { get; set; }

    public string LanguageName { get; set; } = null!;

    public string LanguageGroup { get; set; } = null!;

    public string WritingSystem { get; set; } = null!;

    public virtual ICollection<EthnicComposition> EthnicCompositions { get; set; } = new List<EthnicComposition>();
}
