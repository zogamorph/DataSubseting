using System;

namespace DataSlice.Core.Factory
{
    public class ServiceLocator : IServiceLocator
    {
        private static IResolvable _internalContainer;

        public  void RegisterContainer(IResolvable container)
        {
            _internalContainer = container;
            Console.WriteLine("Container set");
        }

        public  T Resolve<T>()
        {
            return _internalContainer.Resolve<T>();
        }

    }
}


