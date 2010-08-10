using System;
using System.Linq;
using ActionContainer.Framework;
using ActionContainer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ActionContainer.Test
{
	[TestClass]
	public class ServiceAgentTests : MockedTest
	{
		private Mock<IActionListener> _listenerMock;
		private ServiceAgent _serviceAgent;

		[TestInitialize]
		public void TestInit()
		{
			_listenerMock = MockContainer
				.GetMock<IActionListener>();

			MockContainer
				.GetMock<IActionContainerResolvingService>()
				.Setup(x => x.ResolveAll(typeof (IActionListener)))
				.Returns(() => new[] {_listenerMock.Object});

			_serviceAgent = MockContainer.Create<ServiceAgent>();
		}

		[TestMethod]
		public void CanInvokeVoidMethod()
		{
			string input = "Hello";
			dynamic dynoAgent = _serviceAgent;

			Action<ActionCallInfo> assertCallInfo = info =>
				{
					Assert.IsNotNull(info);
					Assert.AreEqual("VoidMethod", info.MethodName);
					Assert.IsNull(info.ReturnType);
					Assert.AreEqual(1, info.UnnamedArguments.Count);
					Assert.AreEqual(input, info.UnnamedArguments.First());
				};
			_listenerMock.Setup(x => x.CanHandle(It.IsAny<ActionCallInfo>()))
				.Callback(assertCallInfo)
				.Returns(true);
			_listenerMock.Setup(x => x.Handle(It.IsAny<ActionListenerContext>()))
				.Callback<ActionListenerContext>(context =>
					{
						ActionCallInfo info = context.CallInfo;
						assertCallInfo(info);
					})
				.Returns(true);

			dynoAgent.VoidMethod(input);

			_listenerMock.VerifyAll();
		}

		[TestMethod]
		public void CanInvokeFuncMethod()
		{
			string input = "Testing func";
			string output = "This is output";
			dynamic dynoAgent = _serviceAgent;

			Action<ActionCallInfo> assertCallInfo = info =>
				{
					Assert.IsNotNull(info);
					Assert.AreEqual("FuncMethod", info.MethodName);
					Assert.AreEqual(typeof (string), info.ReturnType);
					Assert.AreEqual(1, info.UnnamedArguments.Count);
					Assert.AreEqual(input, info.UnnamedArguments.First());
				};
			_listenerMock.Setup(x => x.CanHandle(It.IsAny<ActionCallInfo>()))
				.Callback(assertCallInfo)
				.Returns(true);
			_listenerMock.Setup(x => x.Handle(It.IsAny<ActionListenerContext>()))
				.Callback<ActionListenerContext>(context =>
					{
						ActionCallInfo info = context.CallInfo;
						assertCallInfo(info);
						context.Result = output;
					})
				.Returns(true);

			string result = dynoAgent.FuncMethod(input);
			Assert.AreEqual(output, result);

			_listenerMock.VerifyAll();
		}

		[TestMethod]
		[ExpectedException(typeof (InvalidOperationException))]
		public void ThrowsWhenNoListeners()
		{
			dynamic dynoAgent = _serviceAgent;
			dynoAgent.NoListenersAreRegistered();
		}
	}
}