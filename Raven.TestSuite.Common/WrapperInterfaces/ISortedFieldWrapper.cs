namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface ISortedFieldWrapper
    {
        string Field { get; set; }
        bool Descending { get; set; }
    }
}