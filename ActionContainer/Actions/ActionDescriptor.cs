using System;

namespace ActionContainer.Actions
{
	public class ActionDescriptor : MethodDescriptor
	{
		public Action<object, object[]> Action
		{
			get { return (Action<object, object[]>) Delegate; }
		}
	}
}