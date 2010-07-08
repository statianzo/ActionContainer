using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ActionContainer.Actions
{
	public abstract class MethodDescriptor
	{
		public MethodInfo MethodInfo { get; set; }

		public string RegisteredTypeKey { get; set; }

		public Type[] ParameterTypes { get; set; }

		public string Name { get { return MethodInfo.Name; } }

		public Delegate Delegate { get; set; }
	}
}