using System.Linq;
using System.Reflection;
using ActionContainer.Framework;

namespace ActionContainer.Actions
{
	public class MethodDescriptorFactory
	{
		public static MethodDescriptor Create(MethodInfo methodInfo, string key)
		{
			var descriptor = methodInfo.ReturnType == typeof(void)
											? (MethodDescriptor)new ActionDescriptor { Delegate = LambdaBuilder.CreateAction(methodInfo) }
											: new FuncDescriptor { Delegate = LambdaBuilder.CreateFunction(methodInfo) };
			descriptor.MethodInfo = methodInfo;
			descriptor.RegisteredTypeKey = key;
			descriptor.ParameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

			return descriptor;
		}
	}
}