//-----------------------------------------------------------------------
// <copyright file="JsonExtensions.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Bson;
using Raven.Imports.Newtonsoft.Json.Serialization;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common.Abstractions.Extensions
{
	/// <summary>
	/// Json extensions 
	/// </summary>
	public static class JsonExtensions
	{
	    public static RavenJObjectWrapper ToJObject(object result)
		{
			var dynamicJsonObject = result as Linq.IDynamicJsonObject;
			if (dynamicJsonObject != null)
				return dynamicJsonObject.Inner;
			if (result is string || result is ValueType)
				return new RavenJObjectWrapper { { "Value", new RavenJValueWrapper(result) } };
			return RavenJObjectWrapper.FromObject(result, CreateDefaultJsonSerializer());
		}

		/// <summary>
		/// Convert a byte array to a RavenJObjectWrapper
		/// </summary>
		public static RavenJObjectWrapper ToJObject(this byte [] self)
		{
			return RavenJObjectWrapper.Load(new BsonReader(new MemoryStream(self))
			{
				DateTimeKindHandling = DateTimeKind.Utc,
			});
		}

		/// <summary>
		/// Convert a byte array to a RavenJObjectWrapper
		/// </summary>
		public static RavenJObjectWrapper ToJObject(this Stream self)
		{
			return RavenJObjectWrapper.Load(new BsonReader(self)
			{
				DateTimeKindHandling = DateTimeKind.Utc,
			});
		}

		/// <summary>
		/// Convert a RavenJTokenWrapper to a byte array
		/// </summary>
		public static void WriteTo(this RavenJTokenWrapper self, Stream stream)
		{
			self.WriteTo(new BsonWriter(stream)
			{
				DateTimeKindHandling = DateTimeKind.Unspecified
			}, Default.Converters);
		}


	    /// <summary>
		/// Deserialize a <param name="self"/> to an instance of <typeparam name="T"/>
		/// </summary>
		public static T JsonDeserialization<T>(this byte [] self)
		{
			return (T)CreateDefaultJsonSerializer().Deserialize(new BsonReader(new MemoryStream(self)), typeof(T));
		}

		/// <summary>
		/// Deserialize a <param name="self"/> to an instance of<typeparam name="T"/>
		/// </summary>
		public static T JsonDeserialization<T>(this RavenJObjectWrapper self)
		{
			return (T)CreateDefaultJsonSerializer().Deserialize(new RavenJTokenReader(self), typeof(T));
		}
		
		/// <summary>
		/// Deserialize a <param name="self"/> to an instance of<typeparam name="T"/>
		/// </summary>
		public static T JsonDeserialization<T>(this StreamReader self)
		{
			return CreateDefaultJsonSerializer().Deserialize<T>(self);
		}
		
		/// <summary>
		/// Deserialize a <param name="self"/> to an instance of<typeparam name="T"/>
		/// </summary>
		public static T JsonDeserialization<T>(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				return reader.JsonDeserialization<T>();
			}
		}

		public static T Deserialize<T>(this JsonSerializer self, TextReader reader)
		{
			return (T)self.Deserialize(reader, typeof(T));
		}

		private static readonly IContractResolver contractResolver = new DefaultServerContractResolver(shareCache: true)
		{
#if !NETFX_CORE
			DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
#endif
		};

		private class DefaultServerContractResolver : DefaultContractResolver
		{
			public DefaultServerContractResolver(bool shareCache) : base(shareCache)
			{
			}

			protected override System.Collections.Generic.List<MemberInfo> GetSerializableMembers(Type objectType)
			{
				var serializableMembers = base.GetSerializableMembers(objectType);
				foreach (var toRemove in serializableMembers
					.Where(MembersToFilterOut)
					.ToArray())
				{
					serializableMembers.Remove(toRemove);
				}
				return serializableMembers;
			}

			private static bool MembersToFilterOut(MemberInfo info)
			{
				if (info is EventInfo)
					return true;
				var fieldInfo = info as FieldInfo;
				if (fieldInfo != null && !fieldInfo.IsPublic)
					return true;
				return info.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();
			} 
		}

		public static JsonSerializer CreateDefaultJsonSerializer()
		{
			var jsonSerializer = new JsonSerializer
			{
				DateParseHandling = DateParseHandling.None,
				ContractResolver = contractResolver
			};
			foreach (var defaultJsonConverter in Default.Converters)
			{
				jsonSerializer.Converters.Add(defaultJsonConverter);
			}
			return jsonSerializer;
		}
	}
}