//-----------------------------------------------------------------------
// <copyright file="JsonDynamicConverter.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if !SILVERLIGHT
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Linq;
using Raven.Imports.Newtonsoft.Json;
using Raven.TestSuite.Common.Abstractions.Linq;

namespace Raven.TestSuite.Common.Abstractions.Json.Linq
{
	/// <summary>
	/// Convert a dynamic variable to a json value and vice versa
	/// </summary>
	public class JsonDynamicConverter : JsonConverter
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Raven.Imports.Newtonsoft.Json.JsonWriter"/> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
					writer.WriteNull();
				return;
			}
			if (value.GetType() == typeof (object))
			{
				writer.WriteStartObject();
				writer.WriteEndObject();
				return;
			}

			var dynamicValue = ((IDynamicMetaObjectProvider) value).GetMetaObject(Expression.Constant(value));

			writer.WriteStartObject();
			foreach (var dynamicMemberName in dynamicValue.GetDynamicMemberNames())
			{
				writer.WritePropertyName(dynamicMemberName);
				var memberValue = DynamicUtil.GetValueDynamically(value, dynamicMemberName);
				if(memberValue == null || memberValue is ValueType || memberValue is string)
					writer.WriteValue(memberValue);
				else
					serializer.Serialize(writer, memberValue);
			}
			writer.WriteEndObject();

		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Raven.Imports.Newtonsoft.Json.JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
            var token = RavenJTokenWrapper.ReadFrom(reader);
            var val = token as RavenJValueWrapper;
		    if(val != null)
		        return val.Value;
            var array = token as RavenJArrayWrapper;
			if (array != null)
			{
                var dynamicJsonObject = new DynamicJsonObject(new RavenJObjectWrapper());
				return new DynamicList(array.Select(dynamicJsonObject.TransformToValue).ToArray());
			}

			var typeName = token.Value<string>("$type");
			if(typeName != null)
			{
				var type = Type.GetType(typeName, false);
				if(type != null)
				{
					return serializer.Deserialize(new RavenJTokenReader(token), type);
				}
			}

		    return new DynamicJsonObject((RavenJObjectWrapper)((RavenJObjectWrapper)token).CloneToken());
		}

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof (DynamicJsonObject) || objectType == typeof (object);
		}
	}
}
#endif