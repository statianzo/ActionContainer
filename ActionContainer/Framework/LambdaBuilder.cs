using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ActionContainer.Framework
{
	public class LambdaBuilder
	{
		public static Action<object, object[]> CreateAction(MethodInfo meth)
		{
			int z = 0;
			ParameterExpression p1 = Expression.Parameter(typeof (object), "instance");
			ParameterExpression p2 = Expression.Parameter(typeof (object[]), "parameters");
			IEnumerable<UnaryExpression> parameters = from p in meth.GetParameters()
			                                          select Expression.Convert(
			                                          	Expression.ArrayAccess(p2,
			                                          	                       Expression.Constant(z++)),
			                                          	p.ParameterType);
			MethodCallExpression act = Expression.Call(Expression.Convert(p1, meth.DeclaringType), meth, parameters);

			Expression<Action<object, object[]>> lambda = Expression.Lambda<Action<object, object[]>>(act, p1, p2);
			return lambda.Compile();
		}

		public static Func<object, object[], object> CreateFunction(MethodInfo meth)
		{
			int z = 0;
			ParameterExpression p1 = Expression.Parameter(typeof (object), "instance");
			ParameterExpression p2 = Expression.Parameter(typeof (object[]), "parameters");
			IEnumerable<UnaryExpression> parameters = from p in meth.GetParameters()
			                                          select Expression.Convert(
			                                          	Expression.ArrayAccess(p2,
			                                          	                       Expression.Constant(z++)),
			                                          	p.ParameterType);
			UnaryExpression act =
				Expression.Convert(Expression.Call(Expression.Convert(p1, meth.DeclaringType), meth, parameters), typeof (object));

			Expression<Func<object, object[], object>> lambda = Expression.Lambda<Func<object, object[], object>>(act, p1, p2);
			return lambda.Compile();
		}
	}
}