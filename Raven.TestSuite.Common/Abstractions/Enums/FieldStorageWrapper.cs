namespace Raven.TestSuite.Common.Abstractions.Enums
{
    public enum FieldStorageWrapper
    {
        /// <summary>
        /// Store the original field value in the index. This is useful for short texts like a document's title which should be displayed with the results. 
        /// The value is stored in its original form, i.e. no analyzer is used before it is stored.
        /// </summary>
        Yes,
        /// <summary>
        /// Do not store the field value in the index.
        /// </summary>
        No
    }
}