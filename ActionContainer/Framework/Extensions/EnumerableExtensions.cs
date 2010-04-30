using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionContainer.Framework.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<Type> ToTypes(this IEnumerable<object> args)
		{
			return (args != null && args.Count() > 0) ? args.Select(x => x.GetType()) : new Type[0];
		}
	}
}