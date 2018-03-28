namespace WordsSearch.Core.Parsing.Interfaces
{
    public interface IFormatter<in TParam, out TResult>
    {
        TResult Format(TParam parameter);
    }
}