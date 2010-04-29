using System;
using System.Reflection;
using ActionContainer.Framework;

namespace ActionContainer.Actions
{
	public class FuncDescriptor : MethodDescriptor
	{
		public FuncDescriptor(MethodInfo methodInfo, string key) : base(methodInfo, key)
		{
			Func = LambdaBuilder.CreateFunction(methodInfo);
		}
		public Type ReturnType { get { return MethodInfo.ReturnType; } }
		public Func<object, object[],object> Func { get; private set; }
	}
}