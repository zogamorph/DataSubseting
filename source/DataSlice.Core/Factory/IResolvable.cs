namespace DataSlice.Core.Factory
{
    public interface IResolvable
    {
        T Resolve<T>();
    }
}