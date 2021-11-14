namespace DataSlice.Core.Factory
{
    public interface IServiceLocator
    {
        void RegisterContainer(IResolvable container);
        T Resolve<T>();
    }
}