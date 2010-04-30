using System;
using System.Collections.Generic;

namespace ActionContainer.Framework
{
	public class TypeIsOfBaseComparer : IEqualityComparer<Type>
	{
		public bool Equals(Type x, Type y)
		{
			return x.IsAssignableFrom(y);
		}

		public int GetHashCode(Type obj)
		{
			return obj.GetHashCode();
		}
	}
}