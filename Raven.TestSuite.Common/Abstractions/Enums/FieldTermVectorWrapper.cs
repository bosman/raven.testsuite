namespace Raven.TestSuite.Common.Abstractions.Enums
{
    public enum FieldTermVectorWrapper
    {
        /// <summary>
        /// Do not store term vectors
        /// </summary>
        No,
        /// <summary>
        /// Store the term vectors of each document. A term vector is a list of the document's
        /// terms and their number of occurrences in that document.
        /// </summary>
        Yes,
        /// <summary>
        /// Store the term vector + tokenWrapper position information
        /// </summary>
        WithPositions,
        /// <summary>
        /// Store the term vector + tokenWrapper offset information
        /// </summary>
        WithOffsets,
        /// <summary>
        /// Store the term vector + tokenWrapper position and offset information
        /// </summary>
        WithPositionsAndOffsets
    }
}