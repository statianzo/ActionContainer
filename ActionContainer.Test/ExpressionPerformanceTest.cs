using System;
using System.Diagnostics;
using System.Reflection;
using ActionContainer.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActionContainer.Test
{
	[TestClass]
	public class ExpressionPerformanceTest
	{
		private const int Iterations = 100000;

		[TestMethod]
		public void UsingReflection()
		{
			var sample = new SampleClass();
			MethodInfo meth = typeof (SampleClass).GetMethod("Update");
			var p1 = new object[] {100, "hello"};

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				meth.Invoke(sample, p1);
			}
			stopwatch.Stop();

			Console.WriteLine("Iterations: {0}\tElapsed: {1}", Iterations, stopwatch.ElapsedMilliseconds);
		}

		[TestMethod]
		public void UsingExpression()
		{
			var sample = new SampleClass();
			MethodInfo meth = typeof (SampleClass).GetMethod("Update");
			var p1 = new object[] {100, "hello"};

			Action<object, object[]> action = LambdaBuilder.CreateAction(meth);

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				action(sample, p1);
			}
			stopwatch.Stop();

			Console.WriteLine("Iterations: {0}\tElapsed: {1}", Iterations, stopwatch.ElapsedMilliseconds);
		}

		[TestMethod]
		public void FuncUsingExpression()
		{
			var sample = new SampleClass();
			MethodInfo meth = typeof (SampleClass).GetMethod("GiveAValue");
			var p1 = new object[] {100, "hello"};

			Func<object, object[], object> action = LambdaBuilder.CreateFunction(meth);

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				action(sample, p1);
			}
			stopwatch.Stop();

			Console.WriteLine("Iterations: {0}\tElapsed: {1}", Iterations, stopwatch.ElapsedMilliseconds);
		}

		[TestMethod]
		public void FuncUsingReflection()
		{
			var sample = new SampleClass();
			MethodInfo meth = typeof (SampleClass).GetMethod("GiveAValue");
			var p1 = new object[] {100, "hello"};

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				meth.Invoke(sample, p1);
			}
			stopwatch.Stop();

			Console.WriteLine("Iterations: {0}\tElapsed: {1}", Iterations, stopwatch.ElapsedMilliseconds);
		}

		[TestMethod]
		public void FuncDirectCall()
		{
			var sample = new SampleClass();

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				sample.GiveAValue(100, "hello");
			}
			stopwatch.Stop();

			Console.WriteLine("Iterations: {0}\tElapsed: {1}", Iterations, stopwatch.ElapsedMilliseconds);
		}

		[TestMethod]
		public void ActionDirectCall()
		{
			var sample = new SampleClass();

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				sample.Update(100, "hello");
			}
			stopwatch.Stop();

			Console.WriteLine("Iterations: {0}\tElapsed: {1}", Iterations, stopwatch.ElapsedMilliseconds);
		}
	}

	public class SampleClass
	{
		private static decimal ds;
		private decimal d;

		public void Update(int x, string y)
		{
			d++;
			ds++;
		}

		public decimal GiveAValue(int x, string y)
		{
			return x + d++;
		}
	}
}