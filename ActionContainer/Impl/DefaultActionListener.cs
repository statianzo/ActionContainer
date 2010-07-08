using System;
using System.Collections.Generic;
using System.Linq;
using ActionContainer.Actions;
using ActionContainer.Framework;
using ActionContainer.Framework.Extensions;
using ActionContainer.Services;

namespace ActionContainer.Impl
{
	public class DefaultActionListener : IActionListener
	{
		private readonly IActionContainerResolvingService _resolvingService;

		public DefaultActionListener(IActionContainerResolvingService resolvingService)
		{
			_resolvingService = resolvingService;
		}

		public bool CanHandle(ActionCallInfo callInfo)
		{
			return true;
		}

		public bool Handle(ActionListenerContext context)
		{
			var comparer = new TypeIsOfBaseComparer();

			IEnumerable<MethodDescriptor> filtered = GetDescriptors()
				.Where(x => x.Name == context.CallInfo.MethodName
							&& x.ParameterTypes.SequenceEqual(context.CallInfo.UnnamedArguments.ToTypes(), comparer))
				.ToList();

			var returnType = context.CallInfo.ReturnType;
			if (returnType != null)
			{
				var funcDescriptor = filtered.OfType<FuncDescriptor>().Where(f => f.ReturnType == returnType).FirstOrDefault();
				if (funcDescriptor == null)
					throw new MissingMethodException("Method not found");
				var instance = GetInstanceFromMethodDescriptor(funcDescriptor);
				context.Result = funcDescriptor.Func(instance, context.CallInfo.UnnamedArguments.ToArray());
			}
			else
			{
				var actionDescriptor = filtered.OfType<ActionDescriptor>().FirstOrDefault();
				if(actionDescriptor == null)
					throw new MissingMethodException("Method not found");
				var instance = GetInstanceFromMethodDescriptor(actionDescriptor);
				actionDescriptor.Action(instance, context.CallInfo.UnnamedArguments.ToArray());
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

		private object GetInstanceFromMethodDescriptor(MethodDescriptor descriptor)
		{
			return _resolvingService.Resolve(typeof(IActionProvider), descriptor.RegisteredTypeKey);
		}
	}
}