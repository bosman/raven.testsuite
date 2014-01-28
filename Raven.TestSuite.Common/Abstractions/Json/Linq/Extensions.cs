using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raven.Imports.Newtonsoft.Json.Utilities;

namespace Raven.TestSuite.Common.Abstractions.Json.Linq
{
	public static class Extensions
	{
		/// <summary>
		/// Converts the valueWrapper.
		/// </summary>
		/// <typeparam name="U">The type to convert the valueWrapper to.</typeparam>
		/// <param name="value">A <see cref="RavenJTokenWrapper"/> cast as a <see cref="IEnumerable{T}"/> of <see cref="RavenJTokenWrapper"/>.</param>
		/// <returns>A converted valueWrapper.</returns>
		public static U Value<U>(this IEnumerable<RavenJTokenWrapper> value)
		{
			return value.Value<RavenJTokenWrapper, U>();
		}

		/// <summary>
		/// Converts the valueWrapper.
		/// </summary>
		/// <typeparam name="T">The source collection type.</typeparam>
		/// <typeparam name="U">The type to convert the valueWrapper to.</typeparam>
		/// <param name="value">A <see cref="RavenJTokenWrapper"/> cast as a <see cref="IEnumerable{T}"/> of <see cref="RavenJTokenWrapper"/>.</param>
		/// <returns>A converted valueWrapper.</returns>
		public static U Value<T, U>(this IEnumerable<T> value) where T : RavenJTokenWrapper
		{
			var token = value as RavenJTokenWrapper;
			if (token == null)
				throw new ArgumentException("Source valueWrapper must be a RavenJTokenWrapper.");

			return token.Convert<U>();
		}

		/// <summary>
		/// Returns a collection of converted child values of every object in the source collection.
		/// </summary>
		/// <typeparam name="U">The type to convert the values to.</typeparam>
		/// <param name="source">An <see cref="IEnumerable{T}"/> of <see cref="RavenJTokenWrapper"/> that contains the source collection.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains the converted values of every node in the source collection.</returns>
		public static IEnumerable<U> Values<U>(this IEnumerable<RavenJTokenWrapper> source)
		{
			return Values<U>(source, null);
		}

		/// <summary>
		/// Returns a collection of child values of every object in the source collection with the given key.
		/// </summary>
		/// <param name="source">An <see cref="IEnumerable{T}"/> of <see cref="RavenJTokenWrapper"/> that contains the source collection.</param>
		/// <param name="key">The tokenWrapper key.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="RavenJTokenWrapper"/> that contains the values of every node in the source collection with the given key.</returns>
		public static IEnumerable<RavenJTokenWrapper> Values(this IEnumerable<RavenJTokenWrapper> source, string key)
		{
			return Values<RavenJTokenWrapper>(source, key);
		}

		/// <summary>
		/// Returns a collection of child values of every object in the source collection.
		/// </summary>
		/// <param name="source">An <see cref="IEnumerable{T}"/> of <see cref="RavenJTokenWrapper"/> that contains the source collection.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="RavenJTokenWrapper"/> that contains the values of every node in the source collection.</returns>
		public static IEnumerable<RavenJTokenWrapper> Values(this IEnumerable<RavenJTokenWrapper> source)
		{
			return source.Values(null);
		}

		internal static IEnumerable<U> Values<U>(this IEnumerable<RavenJTokenWrapper> source, string key)
		{
			foreach (RavenJTokenWrapper token in source)
			{
				if (token is RavenJValueWrapper)
				{
					yield return Convert<U>(token);
				}
				else
				{
					foreach (var t in token.Values<U>())
					{
						yield return t;
					}
				}

				var ravenJObject = (RavenJObjectWrapper) token;

				RavenJTokenWrapper value = ravenJObject[key];
				if (value != null)
					yield return value.Convert<U>();
			}

			yield break;
		}

		internal static U Convert<U>(this RavenJTokenWrapper tokenWrapper)
		{
			if (tokenWrapper is RavenJArrayWrapper && typeof(U) == typeof(RavenJObjectWrapper))
			{
				var ar = (RavenJArrayWrapper)tokenWrapper;
				var o = new RavenJObjectWrapper();
				foreach (RavenJObjectWrapper item in ar)
				{
					o[item["Key"].Value<string>()] = item["Value"];
				}
				return (U) (object) o;
			}

			bool cast = typeof(RavenJTokenWrapper).IsAssignableFrom(typeof(U));

			return Convert<U>(tokenWrapper, cast);
		}

		internal static IEnumerable<U> Convert<U>(this IEnumerable<RavenJTokenWrapper> source)
		{
			bool cast = typeof(RavenJTokenWrapper).IsAssignableFrom(typeof(U));

			return source.Select(token => Convert<U>(token, cast));
		}

		internal static U Convert<U>(this RavenJTokenWrapper tokenWrapper, bool cast)
		{
			if (cast)
			{
				// HACK
				return (U)(object)tokenWrapper;
			}
			if (tokenWrapper == null)
				return default(U);

			var value = tokenWrapper as RavenJValueWrapper;
			if (value == null)
				throw new InvalidCastException("Cannot cast {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, tokenWrapper.GetType(), typeof(U)));

			if (value.Value is U)
				return (U)value.Value;

			Type targetType = typeof(U);

			if (targetType.IsGenericType() && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (value.Value == null)
					return default(U);

				targetType = Nullable.GetUnderlyingType(targetType);
			}
			if(targetType == typeof(Guid))
			{
				if (value.Value == null)
					return default(U);
				return (U)(object)new Guid(value.Value.ToString());
			}
			if (targetType == typeof(string))
			{
				if (value.Value == null)
					return default(U);
				return (U)(object)value.Value.ToString();
			}
			if (targetType == typeof(DateTime) && value.Value is string)
			{
				DateTime dateTime;
				if (DateTime.TryParseExact((string)value.Value, Default.DateTimeFormatsToRead, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
					return (U)(object)dateTime;
				
				dateTime = RavenJsonTextReader.ParseDateMicrosoft((string)value.Value);
				return (U)(object)dateTime;
			}
			if (targetType == typeof(DateTimeOffset) && value.Value is string)
			{
				DateTimeOffset dateTimeOffset;
				if (DateTimeOffset.TryParseExact((string)value.Value, Default.DateTimeFormatsToRead, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTimeOffset))
					return (U)(object)dateTimeOffset;

				return default(U);
			}
			return (U)System.Convert.ChangeType(value.Value, targetType, CultureInfo.InvariantCulture);
		}
	}
}
