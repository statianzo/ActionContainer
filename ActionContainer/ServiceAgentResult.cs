using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ActionContainer.Actions;
using ActionContainer.Services;

namespace ActionContainer
{
	public class ServiceAgentResult : DynamicObject
	{
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

	}
}