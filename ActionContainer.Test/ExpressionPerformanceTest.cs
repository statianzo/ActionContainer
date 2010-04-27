using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
			MethodInfo meth = typeof(SampleClass).GetMethod("Update");
			var p1 = new object[] { 100, "hello" };

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
			MethodInfo meth = typeof(SampleClass).GetMethod("Update");
			var p1 = new object[] { 100, "hello" };

			var action = CreateAction(meth);

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
			MethodInfo meth = typeof(SampleClass).GetMethod("GiveAValue");
			var p1 = new object[] { 100, "hello" };

			var action = CreateFunction(meth);

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
			MethodInfo meth = typeof(SampleClass).GetMethod("GiveAValue");
			var p1 = new object[] { 100, "hello" };

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < Iterations; i++)
			{
				meth.Invoke(sample, p1);
			}
			stopwatch.Stop();

			Console.WriteLine("Iterations: {0}\tElapsed: {1}", Iterations, stopwatch.ElapsedMilliseconds);
		}

		private Action<object, object[]> CreateAction(MethodInfo meth)
		{
			int z = 0;
			var p1 = Expression.Parameter(typeof(object), "instance");
			var p2 = Expression.Parameter(typeof(object[]), "parameters");
			IEnumerable<UnaryExpression> parameters = from p in meth.GetParameters()
													  select Expression.Convert(
														Expression.ArrayAccess(p2,
																			   Expression.Constant(z++)),
														p.ParameterType);
			var act = Expression.Call(Expression.Convert(p1, meth.DeclaringType), meth, parameters);

			var lambda = Expression.Lambda<Action<object, object[]>>(act, p1, p2);
			return lambda.Compile();

		}

		private Func<object, object[], object > CreateFunction(MethodInfo meth)
		{
			int z = 0;
			var p1 = Expression.Parameter(typeof(object), "instance");
			var p2 = Expression.Parameter(typeof(object[]), "parameters");
			IEnumerable<UnaryExpression> parameters = from p in meth.GetParameters()
													  select Expression.Convert(
														Expression.ArrayAccess(p2,
																			   Expression.Constant(z++)),
														p.ParameterType);
			var act = Expression.Convert(Expression.Call(Expression.Convert(p1, meth.DeclaringType), meth, parameters),typeof(object));

			var lambda = Expression.Lambda<Func<object, object[], object>>(act, p1, p2);
			return lambda.Compile();

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