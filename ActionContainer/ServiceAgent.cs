using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ActionContainer.Actions;
using ActionContainer.Framework;
using ActionContainer.Framework.Extensions;
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
			IEnumerable<FuncDescriptor> funcs = PossibleMethods.Where(x => binder.ReturnType.IsAssignableFrom(x.ReturnType));
			var funcDescriptor = GetMethod<FuncDescriptor>(funcs);
			object instance = GetInstanceFromMethodDescriptor(funcDescriptor);
			result = funcDescriptor.Func(instance, Args);
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var comparer = new TypeIsOfBaseComparer();

			IEnumerable<MethodDescriptor> filtered = GetDescriptors()
				.Where(x => x.Name == binder.Name
							&& x.ParameterTypes.SequenceEqual(args.ToTypes(), comparer))
				.ToList();

			if (binder.ResultDiscarded())
			{
				var action = GetMethod<ActionDescriptor>(filtered);
				object instance = GetInstanceFromMethodDescriptor(action);

				action.Action(instance, args);
				result = null;
			}
			else
			{
				IEnumerable<FuncDescriptor> funcs = filtered.OfType<FuncDescriptor>();
				result = new ServiceAgent(_resolvingService) { PossibleMethods = funcs, Args = args };
			}
			return true;
		}


		private IEnumerable<MethodDescriptor> GetDescriptors()
		{
			var descriptors =
				_resolvingService
					.Resolve(typeof(IEnumerable<MethodDescriptor>), "MethodDescriptors") as IEnumerable<MethodDescriptor>;
			if (descriptors == null || !descriptors.Any())
				throw new InvalidOperationException("No MethodDescriptors have been registered");
			return descriptors;
		}

		private static T GetMethod<T>(IEnumerable<MethodDescriptor> filtered) where T : MethodDescriptor
		{
			IEnumerable<T> methods = filtered.OfType<T>();
			int count = methods.Count();
			if (count > 1)
				throw new AmbiguousMatchException("Ambiguous Method Match");
			if (count == 0)
				throw new MissingMethodException("Method not found");

			return methods.Single();
		}

		private object GetInstanceFromMethodDescriptor(MethodDescriptor descriptor)
		{
			return _resolvingService.Resolve(typeof(IActionProvider), descriptor.RegisteredTypeKey);
		}
	}
}