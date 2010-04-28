using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ActionContainer.Actions;
using ActionContainer.Framework;
using ActionContainer.Services;
using Microsoft.CSharp.RuntimeBinder;

namespace ActionContainer
{
	public class ServiceAgent : DynamicObject
	{
		private static readonly Func<object, object[], object> ResultDiscarded;

		private readonly IActionContainerResolvingService _resolvingService;

		static ServiceAgent()
		{
			Type type = typeof (CSharpBinderFlags)
				.Assembly
				.GetType("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");
			PropertyInfo prop = type
				.GetProperty("ResultDiscarded");
			ResultDiscarded = LambdaBuilder.CreatePropertyFunction(prop);
		}

		public ServiceAgent(IActionContainerResolvingService resolvingService)
		{
			_resolvingService = resolvingService;
		}

		private IEnumerable<FuncDescriptor> PossibleMethods { get; set; }
		private object[] Args { get; set; }

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			IEnumerable<FuncDescriptor> funcs = PossibleMethods.Where(x => binder.ReturnType.IsAssignableFrom(x.ReturnType));
			var funcDescriptor = GetMethod<FuncDescriptor>(funcs);
			object instance = GetInstanceFromMethodDescriptor(funcDescriptor);
			result = funcDescriptor.Func(instance, Args);
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var descriptors =
				_resolvingService
					.Resolve(typeof (IEnumerable<MethodDescriptor>), "MethodDescriptors") as IEnumerable<MethodDescriptor>;
			if (descriptors == null || !descriptors.Any())
				throw new InvalidOperationException("No MethodDescriptors have been registered");

			var isDiscarded = (bool) ResultDiscarded(binder, null);
			IEnumerable<Type> parameterTypes = (args != null && args.Length > 0) ? args.Select(x => x.GetType()) : new Type[0];
			IEnumerable<MethodDescriptor> filtered = descriptors
				.Where(x => x.Name == binder.Name
				            && parameterTypes.SequenceEqual(x.ParameterTypes));
			if (isDiscarded)
			{
				var action = GetMethod<ActionDescriptor>(filtered);
				object instance = GetInstanceFromMethodDescriptor(action);

				action.Action(instance, args);
				result = null;
			}
			else
			{
				IEnumerable<FuncDescriptor> funcs = filtered.OfType<FuncDescriptor>();
				result = new ServiceAgent(_resolvingService) {PossibleMethods = funcs, Args = args};
			}
			return true;
		}

		private static T GetMethod<T>(IEnumerable<MethodDescriptor> filtered) where T : MethodDescriptor
		{
			IEnumerable<T> actions = filtered.OfType<T>();
			int count = actions.Count();
			if (count > 1)
				throw new AmbiguousMatchException("Ambiguous Method Match");
			if (count == 0)
				throw new MissingMethodException("Method not found");

			return actions.Single();
		}

		private object GetInstanceFromMethodDescriptor(MethodDescriptor descriptor)
		{
			return _resolvingService.Resolve(typeof (IActionProvider), descriptor.RegisteredTypeKey);
		}
	}
}