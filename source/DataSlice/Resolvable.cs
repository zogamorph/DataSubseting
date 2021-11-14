using DataSlice.Core.Factory;
using StructureMap;

namespace DataSlice
{
    public class Resolvable : IResolvable
    {
        private readonly IContainer _container;

        public Resolvable(IContainer container)
        {
            _container = container;
        }

        public T Resolve<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}
