using System;
using System.Dynamic;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace ActionContainer.Framework.Extensions
{
	public static class InvokeMemberBinderExtensions
	{
		private static readonly Func<object, object[], object> ResultDiscardedFunc;

		static InvokeMemberBinderExtensions()
		{
			Type type = typeof(CSharpBinderFlags)
				.Assembly
				.GetType("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");
			PropertyInfo prop = type
				.GetProperty("ResultDiscarded");
			ResultDiscardedFunc = LambdaBuilder.CreatePropertyFunction(prop);
		}

		public static bool ResultDiscarded(this InvokeMemberBinder source)
		{
			return (bool)ResultDiscardedFunc(source, null);
		}
	}
}