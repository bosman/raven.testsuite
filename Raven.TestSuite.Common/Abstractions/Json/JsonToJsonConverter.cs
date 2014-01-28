using System;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.Common.Abstractions.Linq;

namespace Raven.TestSuite.Common.Abstractions.Json
{
	public class JsonToJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is RavenJTokenWrapper)
                ((RavenJTokenWrapper)value).WriteTo(writer);
			else if(value is DynamicNullObject)
				writer.WriteNull();
			else
				((IDynamicJsonObject)value).WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			// NOTE: THIS DOESN'T SUPPORT READING OF DynamicJsonObject !!!

            var o = RavenJTokenWrapper.Load(reader);
			return (o.Type == JTokenType.Null || o.Type == JTokenType.Undefined) ? null : o;
		}

		public override bool CanConvert(Type objectType)
		{
            return objectType == typeof(RavenJTokenWrapper) ||
		           objectType == typeof (DynamicJsonObject) ||
		           objectType == typeof (DynamicNullObject) ||
                   objectType.IsSubclassOf(typeof(RavenJTokenWrapper)) ||
		           objectType.IsSubclassOf(typeof (DynamicJsonObject));
		}
	}
}