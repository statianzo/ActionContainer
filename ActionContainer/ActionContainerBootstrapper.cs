using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActionContainer.Actions;
using ActionContainer.Framework;
using ActionContainer.Services;

namespace ActionContainer
{
	public class ActionContainerBootstrapper
	{

		public void InitializeContainer(IActionContainerRegistrationService registrationService, Assembly assembly)
		{
			RegisterProvidersAndDescriptors(registrationService, assembly);
		}

		private static void RegisterProvidersAndDescriptors(IActionContainerRegistrationService registrationService, Assembly assembly)
		{
			var types = assembly
				.GetTypes()
				.Where(t => typeof(IActionProvider).IsAssignableFrom(t));
			foreach (var t in types)
			{
				var key = Guid.NewGuid().ToString();
				registrationService.Register(t, typeof(IActionProvider), key);
				var actionDescriptors = DescriptorsFromType(t, key);
				registrationService.RegisterInstance(actionDescriptors, typeof(IEnumerable<MethodDescriptor>), "MethodDescriptors");
			}
		}

		private static IEnumerable<MethodDescriptor> DescriptorsFromType(Type type, string key)
		{
			var descriptors = new List<MethodDescriptor>();
			foreach (var m in type.GetMethods())
			{
				if (m.ReturnType == typeof(void))
					descriptors.Add(new ActionDescriptor(m, key) { Action = LambdaBuilder.CreateAction(m) });
				else
					descriptors.Add(new FuncDescriptor(m, key) { Func = LambdaBuilder.CreateFunction(m) });
			}
			return descriptors;
				   

		}
	}
}