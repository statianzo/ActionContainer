using System;
using System.Reflection;
using ActionContainer.Framework;

namespace ActionContainer.Actions
{
	public class ActionDescriptor : MethodDescriptor
	{
		public ActionDescriptor(MethodInfo methodInfo, string key) : base(methodInfo, key)
		{
			Action = LambdaBuilder.CreateAction(methodInfo);
		}

		public Action<object, object[]> Action { get; private set; }
	}
}