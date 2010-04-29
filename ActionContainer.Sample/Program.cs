using System;
using System.Reflection;
using Autofac;

namespace ActionContainer.Sample
{
	internal class Program
	{
		private static void Main()
		{
			var builder = new ContainerBuilder();
			var serviceProvider = new AutofacRegistrationService(builder);
			IContainer container = null;
			var resolver = new AutofacResolvingService(() => container);
			new ActionContainerBootstrapper(x =>
				{
					x.IgnoreMethodsDeclaredBy<object>();
					x.IgnoreMethodsDeclaredBy<MarshalByRefObject>();
					x.IgnoreMethodsDeclaredBy<IDisposable>();
				})
				.InitializeContainer(serviceProvider, Assembly.GetExecutingAssembly());

			builder.RegisterAssemblyTypes(typeof(Program).Assembly)
				.AssignableTo<IDependOnSomething>()
				.WithProperty("ServiceAgent", new ServiceAgent(resolver))
				.AsImplementedInterfaces();


			container = builder.Build();
			var depender = container.Resolve<IDependOnSomething>();
			depender.DoYourThing();
		}
	}

	public class Needy : IDependOnSomething
	{
		public dynamic ServiceAgent { get; set; }

		public void DoYourThing()
		{
			//Void call
			ServiceAgent.SayHello("Jason");

			ServiceAgent.SayHello();

			//Receive an int
			int rand = ServiceAgent.GenerateRandom();
			Console.WriteLine("Random of {0}", rand);
		}
	}

	public interface IDependOnSomething
	{
		dynamic ServiceAgent { get; set; }
		void DoYourThing();
	}
}