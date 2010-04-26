using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ActionContainer
{
	public class ActionContainer : DynamicObject
	{
		private IList<MethodInfo> PossibleMethods { get; set; }
		private object[] Args { get; set; }

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			MethodInfo method = PossibleMethods.FirstOrDefault(x => x.ReturnType == binder.ReturnType);
			if (method == null)
				throw new MissingMethodException("No method matching provided signature found");
			result = method.Invoke(null, Args);
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var meth1 = new Func<int, string>(x => x.ToString());
			var meth2 = new Func<int, int>(x => x + 1000);
			var returnnothing = new Action<int>(x => Console.WriteLine("Void call with {0} provided as an argument", x));

			var isDiscarded =
				(bool)
				binder.GetType().GetProperty("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder.ResultDiscarded",
				                             BindingFlags.NonPublic |
				                             BindingFlags.Instance |
				                             BindingFlags.FlattenHierarchy).GetValue(
				                             	binder, null);
			if (isDiscarded)
			{
				returnnothing.DynamicInvoke(args);
				result = null;
			}
			else
				result = new ActionContainer {PossibleMethods = new[] {meth1.Method, meth2.Method}, Args = args};
			return true;
		}
	}
}