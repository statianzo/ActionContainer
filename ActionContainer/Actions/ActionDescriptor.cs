using System;
using System.Linq;
using System.Reflection;

namespace ActionContainer.Actions
{
	public abstract class MethodDescriptor
	{
		protected MethodDescriptor(MethodInfo methodInfo, string key)
		{
			MethodInfo = methodInfo;
			RegisteredTypeKey = key;
			ParameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

		}

		public MethodInfo MethodInfo { get; set; }

		public string RegisteredTypeKey { get; set; }

		public Type[] ParameterTypes { get; private set; }

		public string Name { get { return MethodInfo.Name; } }
	}

	class ActionDescriptor : MethodDescriptor
	{
		public ActionDescriptor(MethodInfo methodInfo, string key) : base(methodInfo, key) { }

		public Action<object, object[]> Action { get; set; }
	}

	class FuncDescriptor : MethodDescriptor
	{
		public FuncDescriptor(MethodInfo methodInfo, string key) : base(methodInfo, key) { }
		public Type ReturnType { get { return MethodInfo.ReturnType; } }
		public Func<object, object[],object> Func { get; set; }
	}
}