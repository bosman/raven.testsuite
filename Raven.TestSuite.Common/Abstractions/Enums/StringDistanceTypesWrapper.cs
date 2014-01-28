namespace Raven.TestSuite.Common.Abstractions.Enums
{
    public enum StringDistanceTypesWrapper
    {
        /// <summary>
        /// Default, suggestion is not active
        /// </summary>
        None,

        /// <summary>
        /// Default, equivalent to Levenshtein
        /// </summary>
        Default,

        /// <summary>
        /// Levenshtein distance algorithm (default)
        /// </summary>
        Levenshtein,

        /// <summary>
        /// JaroWinkler distance algorithm
        /// </summary>
        JaroWinkler,

        /// <summary>
        /// NGram distance algorithm
        /// </summary>
        NGram,
    }
}