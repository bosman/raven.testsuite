using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;
using Raven.Imports.Newtonsoft.Json.Utilities;
using Raven.TestSuite.Common.Abstractions.Extensions;
using Raven.TestSuite.Common.Abstractions.Imports.Newtonsoft.Json;

namespace Raven.TestSuite.Common.Abstractions.Json.Linq
{
	/// <summary>
	/// Represents a JSON object.
	/// </summary>
	public class RavenJObjectWrapper : RavenJTokenWrapper, IEnumerable<KeyValuePair<string, RavenJTokenWrapper>>
	{
		/// <summary>
		/// This can be used to attach additional state for external clients
		/// Not used by anything related to JSON
		/// </summary>
		public Dictionary<string, object> __ExternalState
		{
			get { return externalState ?? (externalState = new Dictionary<string, object>()); }
		}

		private readonly IEqualityComparer<string> comparer;
		private Dictionary<string, object> externalState;

		/// <summary>
		/// Gets the node type for this <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <value>The type.</value>
		public override JTokenType Type
		{
			get { return JTokenType.Object; }
		}

		internal DictionaryWithParentSnapshot Properties { get; set; }

		public int Count
		{
			get { return Properties.Count; }
		}

		public ICollection<string> Keys
		{
			get { return Properties.Keys; }
		}

		public override bool IsSnapshot
		{
			get { return Properties.IsSnapshot; }
		}

		public RavenJObjectWrapper WithCaseInsensitivePropertyNames()
		{
			var props = new DictionaryWithParentSnapshot(StringComparer.OrdinalIgnoreCase);
			foreach (var property in Properties)
			{
				props[property.Key] = property.Value;
			}
			return new RavenJObjectWrapper(props);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RavenJObjectWrapper"/> class.
		/// </summary>
		public RavenJObjectWrapper() :this(StringComparer.Ordinal)
		{
		}

		public RavenJObjectWrapper(IEqualityComparer<string> comparer)
		{
			this.comparer = comparer;
			Properties = new DictionaryWithParentSnapshot(comparer);
		}

		public RavenJObjectWrapper(RavenJObjectWrapper other)
		{
			Properties = new DictionaryWithParentSnapshot(other.comparer);
			foreach (var kv in other.Properties)
			{
				Properties.Add(kv);
			}
		}

		private RavenJObjectWrapper(DictionaryWithParentSnapshot snapshot)
		{
			Properties = snapshot;
		}

		internal override bool DeepEquals(RavenJTokenWrapper other)
		{
			var t = other as RavenJObjectWrapper;
			if (t == null)
				return false;

			return base.DeepEquals(other);
		}

		/// <summary>
		/// Gets the <see cref="RavenJTokenWrapper"/> with the specified key converted to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to convert the tokenWrapper to.</typeparam>
		/// <param name="key">The tokenWrapper key.</param>
		/// <returns>The converted tokenWrapper value.</returns>
		public override T Value<T>(string key)
		{
			return this[key].Convert<T>();
		}

		/// <summary>
		/// Gets or sets the <see cref="RavenJTokenWrapper"/> with the specified property name.
		/// </summary>
		/// <value></value>
		public RavenJTokenWrapper this[string propertyName]
		{
			get
			{
				RavenJTokenWrapper ret;
				Properties.TryGetValue(propertyName, out ret);
				return ret;
			}
			set { Properties[propertyName] = value; }
		}

		public override RavenJTokenWrapper CloneToken()
		{
			return CloneTokenImpl(new RavenJObjectWrapper());
		}

		internal override IEnumerable<KeyValuePair<string, RavenJTokenWrapper>> GetCloningEnumerator()
		{
			return Properties;
		}

		/// <summary>
		/// Creates a <see cref="RavenJObjectWrapper"/> from an object.
		/// </summary>
		/// <param name="o">The object that will be used to create <see cref="RavenJObjectWrapper"/>.</param>
		/// <returns>A <see cref="RavenJObjectWrapper"/> with the values of the specified object</returns>
		public static new RavenJObjectWrapper FromObject(object o)
		{
			return FromObject(o, JsonExtensions.CreateDefaultJsonSerializer());
		}

		/// <summary>
		/// Creates a <see cref="RavenJArrayWrapper"/> from an object.
		/// </summary>
		/// <param name="o">The object that will be used to create <see cref="RavenJArrayWrapper"/>.</param>
		/// <param name="jsonSerializer">The <see cref="JsonSerializer"/> that will be used to read the object.</param>
		/// <returns>A <see cref="RavenJArrayWrapper"/> with the values of the specified object</returns>
		public static new RavenJObjectWrapper FromObject(object o, JsonSerializer jsonSerializer)
		{
			RavenJTokenWrapper tokenWrapper = FromObjectInternal(o, jsonSerializer);

			if (tokenWrapper != null && tokenWrapper.Type != JTokenType.Object)
				throw new ArgumentException("Object serialized to {0}. RavenJObjectWrapper instance expected.".FormatWith(CultureInfo.InvariantCulture, tokenWrapper.Type));

			return (RavenJObjectWrapper)tokenWrapper;
		}

		/// <summary>
		/// Loads an <see cref="RavenJObjectWrapper"/> from a <see cref="JsonReader"/>. 
		/// </summary>
		/// <param name="reader">A <see cref="JsonReader"/> that will be read for the content of the <see cref="RavenJObjectWrapper"/>.</param>
		/// <returns>A <see cref="RavenJObjectWrapper"/> that contains the JSON that was read from the specified <see cref="JsonReader"/>.</returns>
		public new static RavenJObjectWrapper Load(JsonReader reader)
		{
			if (reader.TokenType == JsonToken.None)
			{
				if (!reader.Read())
					throw new Exception("Error reading RavenJObjectWrapper from JsonReader.");
			}

			if (reader.TokenType != JsonToken.StartObject)
				throw new Exception(
					"Error reading RavenJObjectWrapper from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

			if (reader.Read() == false)
				throw new Exception("Unexpected end of json object");

			string propName = null;
			var o = new RavenJObjectWrapper();
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.Comment:
						// ignore comments
						break;
					case JsonToken.PropertyName:
						propName = reader.Value.ToString();
						break;
					case JsonToken.EndObject:
						return o;
					case JsonToken.StartObject:
						if (!string.IsNullOrEmpty(propName))
						{
							var val = RavenJObjectWrapper.Load(reader);
							o[propName] = val; // TODO: Assert when o.Properties.ContainsKey and its value != val
							propName = null;
						}
						else
						{
							throw new InvalidOperationException("The JsonReader should not be on a tokenWrapper of type {0}."
																	.FormatWith(CultureInfo.InvariantCulture,
																				reader.TokenType));
						}
						break;
					case JsonToken.StartArray:
						if (!string.IsNullOrEmpty(propName))
						{
							var val = RavenJArrayWrapper.Load(reader);
							o[propName] = val; // TODO: Assert when o.Properties.ContainsKey and its value != val
							propName = null;
						}
						else
						{
							throw new InvalidOperationException("The JsonReader should not be on a tokenWrapper of type {0}."
																	.FormatWith(CultureInfo.InvariantCulture,
																				reader.TokenType));
						}
						break;
					default:
						if (!string.IsNullOrEmpty(propName))
						{
							var val = RavenJValueWrapper.Load(reader);
							o[propName] = val; // TODO: Assert when o.Properties.ContainsKey and its value != val
							propName = null;
						}
						else
						{
							throw new InvalidOperationException("The JsonReader should not be on a tokenWrapper of type {0}."
																	.FormatWith(CultureInfo.InvariantCulture,
																				reader.TokenType));
						}
						break;
				}
			} while (reader.Read());

			throw new Exception("Error reading RavenJObjectWrapper from JsonReader.");
		}

		/// <summary>
		/// Load a <see cref="RavenJObjectWrapper"/> from a string that contains JSON.
		/// </summary>
		/// <param name="json">A <see cref="String"/> that contains JSON.</param>
		/// <returns>A <see cref="RavenJObjectWrapper"/> populated from the string that contains JSON.</returns>
		public new static RavenJObjectWrapper Parse(string json)
		{
			try
			{
				JsonReader jsonReader = new RavenJsonTextReader(new StringReader(json));
				return Load(jsonReader);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Could not parse json:" + Environment.NewLine + json, e);
			}
		}

		/// <summary>
		/// Writes this tokenWrapper to a <see cref="JsonWriter"/>.
		/// </summary>
		/// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
		/// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the tokenWrapper.</param>
		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartObject();

			if (Properties != null)
			{
				foreach (var property in Properties)
				{
					writer.WritePropertyName(property.Key);
					if(property.Value == null)
						writer.WriteNull();
					else
						property.Value.WriteTo(writer, converters);
				}
			}

			writer.WriteEndObject();
		}

		#region IEnumerable<KeyValuePair<string,RavenJTokenWrapper>> Members

		public IEnumerator<KeyValuePair<string, RavenJTokenWrapper>> GetEnumerator()
		{
			return Properties.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public void Add(string propName, RavenJTokenWrapper tokenWrapper)
		{
			Properties.Add(propName, tokenWrapper);
		}

		internal override void AddForCloning(string key, RavenJTokenWrapper tokenWrapper)
		{
			Properties[key] = tokenWrapper;
		}

		public bool Remove(string propName)
		{
			return Properties.Remove(propName);
		}

		public bool ContainsKey(string key)
		{
			return Properties.ContainsKey(key);
		}

		public bool TryGetValue(string name, out RavenJTokenWrapper value)
		{
			return Properties.TryGetValue(name, out value);	
		}

		public override RavenJTokenWrapper CreateSnapshot()
		{
			return new RavenJObjectWrapper(Properties.CreateSnapshot());
		}

		public override void EnsureCannotBeChangeAndEnableSnapshotting()
		{
			Properties.EnsureSnapshot();
		}

		public void EnsureSnapshot(string msg)
		{
			Properties.EnsureSnapshot(msg);
		}

		public override IEnumerable<RavenJTokenWrapper> Values()
		{
			return Properties.Values;
		}

		public override IEnumerable<T> Values<T>()
		{
			return Properties.Values.Convert<T>();
		}

		public static async Task<RavenJTokenWrapper> LoadAsync(JsonTextReaderAsync reader)
		{
			if (reader.TokenType == JsonToken.None)
			{
				if (!await reader.ReadAsync())
					throw new Exception("Error reading RavenJObjectWrapper from JsonReader.");
			}

			if (reader.TokenType != JsonToken.StartObject)
				throw new Exception(
					"Error reading RavenJObjectWrapper from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

			if (await reader.ReadAsync() == false)
				throw new Exception("Unexpected end of json object");

			string propName = null;
			var o = new RavenJObjectWrapper();
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.Comment:
						// ignore comments
						break;
					case JsonToken.PropertyName:
						propName = reader.Value.ToString();
						break;
					case JsonToken.EndObject:
						return o;
					case JsonToken.StartObject:
						if (!string.IsNullOrEmpty(propName))
						{
							var val = await RavenJObjectWrapper.LoadAsync(reader);
							o[propName] = val; // TODO: Assert when o.Properties.ContainsKey and its value != val
							propName = null;
						}
						else
						{
							throw new InvalidOperationException("The JsonReader should not be on a tokenWrapper of type {0}."
																	.FormatWith(CultureInfo.InvariantCulture,
																				reader.TokenType));
						}
						break;
					case JsonToken.StartArray:
						if (!string.IsNullOrEmpty(propName))
						{
							var val = await RavenJArrayWrapper.LoadAsync(reader);
							o[propName] = val; // TODO: Assert when o.Properties.ContainsKey and its value != val
							propName = null;
						}
						else
						{
							throw new InvalidOperationException("The JsonReader should not be on a tokenWrapper of type {0}."
																	.FormatWith(CultureInfo.InvariantCulture,
																				reader.TokenType));
						}
						break;
					default:
						if (!string.IsNullOrEmpty(propName))
						{
							var val = RavenJValueWrapper.Load(reader);
							o[propName] = val; // TODO: Assert when o.Properties.ContainsKey and its value != val
							propName = null;
						}
						else
						{
							throw new InvalidOperationException("The JsonReader should not be on a tokenWrapper of type {0}."
																	.FormatWith(CultureInfo.InvariantCulture,
																				reader.TokenType));
						}
						break;
				}
			} while (await reader.ReadAsync());

			throw new Exception("Error reading RavenJObjectWrapper from JsonReader.");
		}
	}
}