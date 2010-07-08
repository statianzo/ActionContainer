using System;

namespace ActionContainer.Actions
{
	public class FuncDescriptor : MethodDescriptor
	{
		public Type ReturnType
		{
			get { return MethodInfo.ReturnType; }
		}

		public Func<object, object[], object> Func
		{
			get { return (Func<object, object[], object>) Delegate; }
		}
	}
}