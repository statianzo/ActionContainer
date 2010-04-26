using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace ActionContainer.Sample
{
	internal class Program
	{
		private static void Main()
		{
			var builder = new ContainerBuilder();
			builder.RegisterAssemblyTypes(typeof(Program).Assembly)
				.AssignableTo<IDependOnSomething>()
				.WithProperty("ActionContainer", new ActionContainer())
				.AsImplementedInterfaces();


			IContainer container = builder.Build();
			var depender = container.Resolve<IDependOnSomething>();
			depender.DoYourThing();
		}
	}

	public class Needy : IDependOnSomething
	{
		public dynamic ActionContainer { get; set; }

		public void DoYourThing()
		{
			//Void call
			ActionContainer.VoidMethod(23);
			//Call that returns an integer
			int i = ActionContainer.GiveMeAnInt(40);
			Console.WriteLine("{0} - {1}", i, i.GetType());
			//Call that returns a string
			string s = ActionContainer.WhatAboutAString(938);
			Console.WriteLine("{0} - {1}", s, s.GetType());

			//Missing method
			try
			{
				bool q = ActionContainer.CanYouBoolMe("Hello");
			}
			catch (MissingMethodException e)
			{
				Console.WriteLine(e.Message);
			}


		}
	}

	public interface IDependOnSomething
	{
		dynamic ActionContainer { get; set; }
		void DoYourThing();
	}
}
