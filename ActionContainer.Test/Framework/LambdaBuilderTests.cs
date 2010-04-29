using System.Reflection;
using ActionContainer.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActionContainer.Test.Framework
{
	[TestClass]
	public class LambdaBuilderTests
	{
		private TesterClass _tester;

		[TestInitialize]
		public void TestInit()
		{
			_tester = new TesterClass();
		}

		[TestMethod]
		public void CanCreateAction()
		{
			var meth = typeof(TesterClass).GetMethod("TestVoid");

			var action = LambdaBuilder.CreateAction(meth);

			action(_tester, null);

			Assert.IsTrue(_tester.TestVoidHit);

		}

		[TestMethod]
		public void CanCreatePropertyAccessor()
		{
			var prop = typeof(TesterClass).GetProperty("TestProp",BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
			var func = LambdaBuilder.CreatePropertyFunction(prop);
			var z = (string)func(_tester, null);

			Assert.IsTrue(_tester.TestPropHit);
			Assert.AreEqual("Hello", z);

		}

		[TestMethod]
		public void CanCreateFunc()
		{
			var meth = typeof(TesterClass).GetMethod("TestInt");
			var func = LambdaBuilder.CreateFunction(meth);
			const int input = 5;
			var z = (int)func(_tester, new object[] { input });

			Assert.IsTrue(_tester.TestIntHit);
			Assert.AreEqual(input * 100, z);

		}

		private class TesterClass
		{
			public bool TestVoidHit { get; private set; }
			public void TestVoid()
			{
				TestVoidHit = true;
			}

			public int TestInt(int z)
			{
				TestIntHit = true;
				return z * 100;
			}

			public bool TestIntHit { get; set; }

			public bool TestPropHit{ get; set;}
			private string TestProp { get
			{
				TestPropHit = true;
				return "Hello";
			} }
		}
	}
}