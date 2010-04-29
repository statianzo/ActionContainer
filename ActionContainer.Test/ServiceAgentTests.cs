using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ActionContainer.Actions;
using ActionContainer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ActionContainer.Test
{
	[TestClass]
	public class ServiceAgentTests : MockedTest
	{
		private IEnumerable<MethodDescriptor> _methodDescriptors;
		private ServiceAgent _serviceAgent;
		private EmptyProvider _provider;

		[TestInitialize]
		public void TestInit()
		{
			_provider = new EmptyProvider();
			MockContainer
				.GetMock<IActionContainerResolvingService>()
				.Setup(x => x.Resolve(typeof(IEnumerable<MethodDescriptor>), It.IsAny<string>()))
				.Returns(() => _methodDescriptors);

			MockContainer
				.GetMock<IActionContainerResolvingService>()
				.Setup(x => x.Resolve(typeof(IActionProvider), It.IsAny<string>()))
				.Returns(() => _provider);


			_methodDescriptors = CreateMethodDescriptors();

			_serviceAgent = MockContainer.Create<ServiceAgent>();
		}

		public IEnumerable<MethodDescriptor> CreateMethodDescriptors()
		{
			MethodInfo voidMeth = typeof(EmptyProvider).GetMethod("VoidMethod");
			MethodInfo funcMeth = typeof(EmptyProvider).GetMethod("FuncMethod");
			var funcDescriptor = new FuncDescriptor(funcMeth, "1");
			var actionDescriptor = new ActionDescriptor(voidMeth, "2");
			return new MethodDescriptor[] { funcDescriptor, actionDescriptor };
		}

		[TestMethod]
		public void CanInvokeVoidMethod()
		{
			dynamic dynoAgent = _serviceAgent;
			dynoAgent.VoidMethod("Hello");

			Assert.IsTrue(_provider.VoidInvoked);
		}

		[TestMethod]
		public void CanInvokeFuncMethod()
		{
			string input = "hello";
			dynamic dynoAgent = _serviceAgent;
			string result = dynoAgent.FuncMethod(input);

			Assert.IsTrue(_provider.FuncInvoked);
			Assert.IsNotNull(result);
			Assert.AreEqual(_provider.FuncMethod(input), result);
		}

		[TestMethod]
		[ExpectedException(typeof(MissingMethodException))]
		public void ThrowsWhenInvokingMissingMethod()
		{
			dynamic dynoAgent = _serviceAgent;
			dynoAgent.ThisMethodDoesntExist();
		}

		[TestMethod]
		public void ServiceAgentVsDirectCallBenchmark()
		{
			string input = "hello";
			dynamic dynoAgent = _serviceAgent;

			//Initial calls
			dynoAgent.VoidMethod(input);
			_provider.VoidMethod(input);

			var stopwatch = new Stopwatch();
			int iterations = 100000;

			stopwatch.Start();
			for (int i = 0; i < iterations; i++)
				new EmptyProvider().VoidMethod(input);
			stopwatch.Stop();
			Console.WriteLine("Direct Call\tIterations: {0} Time: {1} ms", iterations, stopwatch.ElapsedMilliseconds);

			stopwatch.Restart();
			for (int i = 0; i < iterations; i++)
				dynoAgent.VoidMethod(input);
			stopwatch.Stop();
			Console.WriteLine("Dynamic Call\tIterations: {0} Time: {1} ms", iterations, stopwatch.ElapsedMilliseconds);
		}


		private class EmptyProvider : IActionProvider
		{
			public bool VoidInvoked { get; private set; }
			public bool FuncInvoked { get; private set; }

			readonly Random _rand = new Random();
			public void VoidMethod(string s)
			{
				_rand.Next();
				VoidInvoked = true;
			}

			public string FuncMethod(string s)
			{
				FuncInvoked = true;
				return s;
			}
		}

	}
}