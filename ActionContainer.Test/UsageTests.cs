using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using ActionContainer.Actions;
using ActionContainer.Impl;
using ActionContainer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ActionContainer.Test
{
	[TestClass]
	public class UsageTests : MockedTest
	{
		private EmptyProvider _provider;
		private ServiceAgent _serviceAgent;
		private IEnumerable<MethodDescriptor> _methodDescriptors;

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

			MockContainer
				.GetMock<IActionContainerResolvingService>()
				.Setup(x => x.ResolveAll(typeof(IActionListener)))
				.Returns(() => new[] { new DefaultActionListener(MockContainer.GetMock<IActionContainerResolvingService>().Object) });


			_methodDescriptors = CreateMethodDescriptors();

			_serviceAgent = MockContainer.Create<ServiceAgent>();
		}

		public IEnumerable<MethodDescriptor> CreateMethodDescriptors()
		{
			MethodInfo voidMeth = typeof(EmptyProvider).GetMethod("VoidMethod");
			MethodInfo funcMeth = typeof(EmptyProvider).GetMethod("FuncMethod");
			MethodInfo randToListMeth = typeof(EmptyProvider).GetMethod("AddRandomToList");
			MethodDescriptor funcDescriptor = MethodDescriptorFactory.Create(funcMeth, "1");
			MethodDescriptor actionDescriptor = MethodDescriptorFactory.Create(voidMeth, "2");
			MethodDescriptor randToListDescriptor = MethodDescriptorFactory.Create(randToListMeth, "3");
			return new[] { funcDescriptor, actionDescriptor, randToListDescriptor };
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

		[TestMethod]
		public void ThreadedCalling()
		{
			int iterations = 10000;
			dynamic dynoAgent = _serviceAgent;
			Queue nums = Queue.Synchronized(new Queue());
			Action a = () =>
				{
					for (int i = 0; i < iterations; i++)
						dynoAgent.AddRandomToList(nums);
				};

			var asyncResults = new List<IAsyncResult>();
			for (int t = 0; t < 64; t++)
			{
				asyncResults.Add(a.BeginInvoke(null, a));
			}

			while (asyncResults.Any(x => !x.IsCompleted))
				Thread.Sleep(1000);

			Assert.AreEqual(640000, nums.Count);
		}

		#region Nested type: EmptyProvider

		private class EmptyProvider : IActionProvider
		{
			private readonly Random _rand = new Random();
			public bool VoidInvoked { get; private set; }
			public bool FuncInvoked { get; private set; }

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

			public void AddRandomToList(Queue nums)
			{
				nums.Enqueue(_rand.Next());
			}
		}

		#endregion
	}
}