using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ActionContainer.Actions;
using ActionContainer.Services;

namespace ActionContainer
{
	public class ServiceAgent : DynamicObject
	{
		private readonly IActionContainerResolvingService _resolvingService;

		public ServiceAgent(IActionContainerResolvingService resolvingService)
		{
			_resolvingService = resolvingService;
		}

		private IEnumerable<FuncDescriptor> PossibleMethods { get; set; }
		private object[] Args { get; set; }

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			var funcs = PossibleMethods.Where(x => binder.ReturnType.IsAssignableFrom(x.ReturnType));
			var count = funcs.Count();
			if (count > 1)
				throw new AmbiguousMatchException("Ambiguous Action Match");
			if (count == 0)
				throw new MissingMethodException("Method not found");
			var func = funcs.Single();
			var instance = GetInstanceFromMethodDescriptor(func);
			result = func.Func(instance, Args);
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var descriptors =
				_resolvingService.Resolve(typeof(IEnumerable<MethodDescriptor>), "MethodDescriptors") as
				IEnumerable<MethodDescriptor>;
			if (descriptors == null)
				throw new InvalidOperationException("MethodDescriptors have not been registered");

			var isDiscarded =
				(bool)
				binder.GetType().GetProperty("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder.ResultDiscarded",
											 BindingFlags.NonPublic |
											 BindingFlags.Instance |
											 BindingFlags.FlattenHierarchy).GetValue(
												binder, null);
			IEnumerable<Type> parameterTypes = (args != null && args.Length > 0) ? args.Select(x => x.GetType()) : new Type[0];
			IEnumerable<MethodDescriptor> filtered = descriptors
				.Where(x => x.Name == binder.Name
							&& parameterTypes.SequenceEqual(x.ParameterTypes));
			if (isDiscarded)
			{
				var actions = filtered.OfType<ActionDescriptor>();
				var count = actions.Count();
				if (count > 1)
					throw new AmbiguousMatchException("Ambiguous Action Match");
				if (count == 0)
					throw new MissingMethodException("Method not found");

				ActionDescriptor action = actions.Single();
				var instance = GetInstanceFromMethodDescriptor(action);

				action.Action(instance, args);
				result = null;
			}
			else
			{
				var funcs = filtered.OfType<FuncDescriptor>();
				result = new ServiceAgent(_resolvingService) { PossibleMethods = funcs, Args = args };
			}
			return true;
		}

		private object GetInstanceFromMethodDescriptor(MethodDescriptor descriptor)
		{
			return _resolvingService.Resolve(typeof(IActionProvider), descriptor.RegisteredTypeKey);
		}
	}
}