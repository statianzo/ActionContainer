using System;
using ActionContainer.Services;

namespace ActionContainer.Sample
{
	public class HelloProvider : IActionProvider
	{
		public void SayHello()
		{
			Console.WriteLine("Hello, no name");
		}
		public void SayHello(string name)
		{
			Console.WriteLine("Hello, {0}", name);
		}

		public void SayHello(string name, int age)
		{
			Console.WriteLine("Your name is {0} and your age is {1}", name, age);
		}

	}
}