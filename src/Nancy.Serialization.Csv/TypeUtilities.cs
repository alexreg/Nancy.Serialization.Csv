using System;
using System.Collections.Generic;

namespace Nancy.Serialization.Csv
{
	public static class TypeUtilities
	{
		public static Type GetCollectionElementTypeFromConstructor(this Type type)
		{
			foreach (var constructorInfo in type.GetConstructors())
			{
				var parametersArray = constructorInfo.GetParameters();
				if (parametersArray.Length == 1)
				{
					var parameterType = parametersArray[0].GetType();
					if (parameterType == typeof(IEnumerable<>))
					{
						return parameterType.GenericTypeArguments[0];
					}
				}
			}
			return null;
		}

		public static Type GetCollectionElementTypeFromInterface(this Type type)
		{
			foreach (var interfaceType in type.GetInterfaces())
			{
				if (interfaceType == typeof(ICollection<>))
				{
					return interfaceType.GenericTypeArguments[0];
				}
			}
			return null;
		}

		public static Type GetEnumeraleElementTypeFromInterface(this Type type)
		{
			foreach (var interfaceType in type.GetInterfaces())
			{
				if (interfaceType == typeof(IEnumerable<>))
				{
					return interfaceType.GenericTypeArguments[0];
				}
			}
			return null;
		}
	}
}
