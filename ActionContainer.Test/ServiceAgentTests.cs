using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActionContainer.Test
{
	[TestClass]
	public class ServiceAgentTests
	{
		private ServiceAgent _serviceAgent;

		[TestInitialize]
		public void TestInit()
		{
			_serviceAgent = (ServiceAgent) new object();
		}

		[TestMethod]
		public void HandlesInvokeMember()
		{
			dynamic agent = _serviceAgent;
			agent.Hello(3);
		}
	}
}