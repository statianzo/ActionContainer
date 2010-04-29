using Moq;

namespace ActionContainer.Test
{
    public abstract class MockedTest
    {
        private MockFactory _mockFactory;
        private AutoMockContainer _mockContainer;

        protected MockFactory MockFactory
        {
            get { return _mockFactory ?? (_mockFactory = new MockFactory(MockBehavior.Default)); }
            set { _mockFactory = value; }
        }

        protected AutoMockContainer MockContainer
        {
            get { return _mockContainer ?? (_mockContainer = new AutoMockContainer(MockFactory)); }
            set { _mockContainer = value; }
        }
    }
}