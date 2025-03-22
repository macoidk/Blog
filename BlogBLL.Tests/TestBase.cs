using Ninject;
using NUnit.Framework;
using BlogSystem.Infrastructure;

namespace BlogSystem.Tests
{
    public abstract class TestBase
    {
        protected IKernel Kernel;

        [SetUp]
        public void SetUp()
        {
            Kernel = new StandardKernel(new ServiceModule());
        }

        [TearDown]
        public void TearDown()
        {
            Kernel.Dispose();
        }
    }
}