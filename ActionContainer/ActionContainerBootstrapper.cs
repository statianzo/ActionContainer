using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ActionContainer.Actions;
using ActionContainer.Framework;
using ActionContainer.Impl;
using ActionContainer.Services;

namespace ActionContainer
{
	public interface IActionContainerConfiguration
	{
		IActionContainerConfiguration ExcludeMethods(Expression<Func<MethodInfo, bool>> filter);
		IActionContainerConfiguration IgnoreMethodsDeclaredBy<T>();
	}

	public class ActionContainerBootstrapper : IActionContainerConfiguration
	{
		private readonly CompositeFilter<MethodInfo> _methodFilters = new CompositeFilter<MethodInfo>();

		public ActionContainerBootstrapper(Action<ActionContainerBootstrapper> configure)
			: this()
		{
			configure(this);
		}

		public ActionContainerBootstrapper()
		{
			_methodFilters.Excludes += m => m.DeclaringType == typeof(object);
			_methodFilters.Excludes += m => m.ContainsGenericParameters;
			_methodFilters.Excludes += m => m.IsSpecialName;
		}

		public void InitializeContainer(IActionContainerRegistrationService registrationService, Assembly assembly)
		{
			RegisterProvidersAndDescriptors(registrationService, assembly);
			RegisterListeners(registrationService, assembly);
		}
		private static void RegisterListeners(IActionContainerRegistrationService registrationService, Assembly assembly)
		{

			registrationService.Register(typeof(DefaultActionListener), typeof(IActionListener));
			var types = assembly
				.GetTypes()
				.Where(t => typeof(IActionListener).IsAssignableFrom(t));
			foreach (var type in types)
				registrationService.Register(type, typeof(IActionListener));
		}
		private void RegisterProvidersAndDescriptors(IActionContainerRegistrationService registrationService, Assembly assembly)
		{
			var types = assembly
				.GetTypes()
				.Where(t => typeof(IActionProvider).IsAssignableFrom(t));

			var methodDescriptors = types.SelectMany(t =>
				{
					var key = Guid.NewGuid().ToString();
					registrationService.Register(t, typeof(IActionProvider), key);
					var actionDescriptors = DescriptorsFromType(t, key);
					return actionDescriptors;
				}).ToList();

			registrationService.RegisterInstance(methodDescriptors, typeof(IEnumerable<MethodDescriptor>), "MethodDescriptors");
		}

		private IEnumerable<MethodDescriptor> DescriptorsFromType(Type type, string key)
		{
			return type.GetMethods()
				.Where(_methodFilters.Matches)
				.Select(m => MethodDescriptorFactory.Create(m, key))
				.ToList();
		}

		public IActionContainerConfiguration IgnoreMethodsDeclaredBy<T>()
		{
			ExcludeMethods(m => m.DeclaringType == typeof(T));
			return this;
		}

		public IActionContainerConfiguration ExcludeMethods(Expression<Func<MethodInfo, bool>> filter)
		{
			_methodFilters.Excludes += filter;
			return this;
		}
	}
}