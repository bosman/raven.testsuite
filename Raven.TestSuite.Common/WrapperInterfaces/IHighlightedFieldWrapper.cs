namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IHighlightedFieldWrapper
    {
        string Field { get; }
        int FragmentLength { get; }
        int FragmentCount { get; }
        string FragmentsField { get; }
    }
}