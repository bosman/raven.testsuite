using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;
using Raven.Imports.Newtonsoft.Json.Utilities;
using Raven.TestSuite.Common.Abstractions.Extensions;
using Raven.TestSuite.Common.Abstractions.Imports.Newtonsoft.Json;

namespace Raven.TestSuite.Common.Abstractions.Json.Linq
{
	/// <summary>
	/// Represents an abstract JSON tokenWrapper.
	/// </summary>
	public abstract class RavenJTokenWrapper
	{
		/// <summary>
		/// Gets the node type for this <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <value>The type.</value>
		public abstract JTokenType Type { get; }

		/// <summary>
		/// Clones this object
		/// </summary>
		/// <returns>A cloned RavenJTokenWrapper</returns>
		public abstract RavenJTokenWrapper CloneToken();

		public abstract bool IsSnapshot { get; }

		public abstract void EnsureCannotBeChangeAndEnableSnapshotting();

		public abstract RavenJTokenWrapper CreateSnapshot();

		protected RavenJTokenWrapper CloneTokenImpl(RavenJTokenWrapper newObject)
		{
			var readingStack = new Stack<IEnumerable<KeyValuePair<string, RavenJTokenWrapper>>>();
			var writingStack = new Stack<RavenJTokenWrapper>();

			writingStack.Push(newObject);
			readingStack.Push(GetCloningEnumerator());

			while (readingStack.Count > 0)
			{
				var curReader = readingStack.Pop();
				var curObject = writingStack.Pop();
				foreach (var current in curReader)
				{
					if (current.Value == null)
					{
						curObject.AddForCloning(current.Key, null); // we call this explicitly to support null entries in JArray
						continue;
					}
					if (current.Value is RavenJValueWrapper)
					{
						curObject.AddForCloning(current.Key, current.Value.CloneToken());
						continue;
					}

					var newVal = current.Value is RavenJArrayWrapper ? (RavenJTokenWrapper)new RavenJArrayWrapper() : new RavenJObjectWrapper();

					curObject.AddForCloning(current.Key, newVal);

					writingStack.Push(newVal);
					readingStack.Push(current.Value.GetCloningEnumerator());
				}
			}
			return newObject;
		}

		internal static RavenJTokenWrapper FromObjectInternal(object o, JsonSerializer jsonSerializer)
		{
			var ravenJToken = o as RavenJTokenWrapper;
			if (ravenJToken != null)
				return ravenJToken;

			RavenJTokenWrapper tokenWrapper;
			using (var jsonWriter = new RavenJTokenWriter())
			{
				jsonSerializer.Serialize(jsonWriter, o);
				tokenWrapper = jsonWriter.TokenWrapper;
			}

			return tokenWrapper;
		}

		/// <summary>
		/// Creates a <see cref="RavenJTokenWrapper"/> from an object.
		/// </summary>
		/// <param name="o">The object that will be used to create <see cref="RavenJTokenWrapper"/>.</param>
		/// <returns>A <see cref="RavenJTokenWrapper"/> with the value of the specified object</returns>
		public static RavenJTokenWrapper FromObject(object o)
		{
			return FromObjectInternal(o, JsonExtensions.CreateDefaultJsonSerializer());
		}

		/// <summary>
		/// Creates a <see cref="RavenJTokenWrapper"/> from an object using the specified <see cref="JsonSerializer"/>.
		/// </summary>
		/// <param name="o">The object that will be used to create <see cref="RavenJTokenWrapper"/>.</param>
		/// <param name="jsonSerializer">The <see cref="JsonSerializer"/> that will be used when reading the object.</param>
		/// <returns>A <see cref="RavenJTokenWrapper"/> with the value of the specified object</returns>
		public static RavenJTokenWrapper FromObject(object o, JsonSerializer jsonSerializer)
		{
			return FromObjectInternal(o, jsonSerializer);
		}

		/// <summary>
		/// Returns the indented JSON for this tokenWrapper.
		/// </summary>
		/// <returns>
		/// The indented JSON for this tokenWrapper.
		/// </returns>
		public override string ToString()
		{
			return ToString(Formatting.Indented);
		}

		/// <summary>
		/// Returns the JSON for this tokenWrapper using the given formatting and converters.
		/// </summary>
		/// <param name="formatting">Indicates how the output is formatted.</param>
		/// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the tokenWrapper.</param>
		/// <returns>The JSON for this tokenWrapper using the given formatting and converters.</returns>
		public string ToString(Formatting formatting, params JsonConverter[] converters)
		{
			using (var sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				var jw = new JsonTextWriter(sw);
				jw.Formatting = formatting;

				WriteTo(jw, converters);

				return sw.ToString();
			}
		}

        /// <summary>
		/// Writes this tokenWrapper to a <see cref="JsonWriter"/>.
		/// </summary>
		/// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
		/// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the tokenWrapper.</param>
		public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

		/// <summary>
		/// Creates a <see cref="RavenJTokenWrapper"/> from a <see cref="JsonReader"/>.
		/// </summary>
		/// <param name="reader">An <see cref="JsonReader"/> positioned at the tokenWrapper to read into this <see cref="RavenJTokenWrapper"/>.</param>
		/// <returns>
		/// An <see cref="RavenJTokenWrapper"/> that contains the tokenWrapper and its descendant tokens
		/// that were read from the reader. The runtime type of the tokenWrapper is determined
		/// by the tokenWrapper type of the first tokenWrapper encountered in the reader.
		/// </returns>
		public static RavenJTokenWrapper ReadFrom(JsonReader reader)
		{
			if (reader.TokenType == JsonToken.None)
			{
				if (!reader.Read())
					throw new Exception("Error reading RavenJTokenWrapper from JsonReader.");
			}

			switch (reader.TokenType)
			{
				case JsonToken.StartObject:
					return RavenJObjectWrapper.Load(reader);
				case JsonToken.StartArray:
					return RavenJArrayWrapper.Load(reader);
				case JsonToken.String:
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.Date:
				case JsonToken.Boolean:
				case JsonToken.Bytes:
				case JsonToken.Null:
				case JsonToken.Undefined:
					return new RavenJValueWrapper(reader.Value);
			}

			throw new Exception("Error reading RavenJTokenWrapper from JsonReader. Unexpected tokenWrapper: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}

		/// <summary>
		/// Load a <see cref="RavenJTokenWrapper"/> from a string that contains JSON.
		/// </summary>
		/// <param name="json">A <see cref="String"/> that contains JSON.</param>
		/// <returns>A <see cref="RavenJTokenWrapper"/> populated from the string that contains JSON.</returns>
		public static RavenJTokenWrapper Parse(string json)
		{
			try
			{
				JsonReader jsonReader = new RavenJsonTextReader(new StringReader(json));

				return Load(jsonReader);
			}
			catch (Exception e)
			{
				throw new JsonSerializationException("Could not parse: [" + json + "]", e);
			}
		}

		public static RavenJTokenWrapper TryLoad(Stream stream)
		{
			var jsonTextReader = new RavenJsonTextReader(new StreamReader(stream));
			if (jsonTextReader.Read() == false || jsonTextReader.TokenType == JsonToken.None)
			{
				return null;
			}
			return Load(jsonTextReader);
		}

		public static async Task<RavenJTokenWrapper> TryLoadAsync(Stream stream)
		{
			var jsonTextReader = new JsonTextReaderAsync(new StreamReader(stream));
			if (await jsonTextReader.ReadAsync() == false || jsonTextReader.TokenType == JsonToken.None)
			{
				return null;
			}
			return await ReadFromAsync(jsonTextReader);
		}

		/// <summary>
		/// Creates a <see cref="RavenJTokenWrapper"/> from a <see cref="JsonReader"/>.
		/// </summary>
		/// <param name="reader">An <see cref="JsonReader"/> positioned at the tokenWrapper to read into this <see cref="RavenJTokenWrapper"/>.</param>
		/// <returns>
		/// An <see cref="RavenJTokenWrapper"/> that contains the tokenWrapper and its descendant tokens
		/// that were read from the reader. The runtime type of the tokenWrapper is determined
		/// by the tokenWrapper type of the first tokenWrapper encountered in the reader.
		/// </returns>
		public static RavenJTokenWrapper Load(JsonReader reader)
		{
			return ReadFrom(reader);
		}

		/// <summary>
		/// Gets the <see cref="RavenJTokenWrapper"/> with the specified key converted to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to convert the tokenWrapper to.</typeparam>
		/// <param name="key">The tokenWrapper key.</param>
		/// <returns>The converted tokenWrapper value.</returns>
		public virtual T Value<T>(string key)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Compares the values of two tokens, including the values of all descendant tokens.
		/// </summary>
		/// <param name="t1">The first <see cref="RavenJTokenWrapper"/> to compare.</param>
		/// <param name="t2">The second <see cref="RavenJTokenWrapper"/> to compare.</param>
		/// <returns>true if the tokens are equal; otherwise false.</returns>
		public static bool DeepEquals(RavenJTokenWrapper t1, RavenJTokenWrapper t2)
		{
			return (t1 == t2 || (t1 != null && t2 != null && t1.DeepEquals(t2)));
		}

		public static int GetDeepHashCode(RavenJTokenWrapper t)
		{
			return t == null ? 0 : t.GetDeepHashCode();
		}

		internal virtual bool DeepEquals(RavenJTokenWrapper other)
		{
			if (other == null)
				return false;

			if (Type != other.Type)
				return false;

			var otherStack = new Stack<RavenJTokenWrapper>();
			var thisStack = new Stack<RavenJTokenWrapper>();

			thisStack.Push(this);
			otherStack.Push(other);

			while (otherStack.Count > 0)
			{
				var curOtherReader = otherStack.Pop();
				var curThisReader = thisStack.Pop();

				if(curOtherReader == null && curThisReader == null)
					continue; // shouldn't happen, but we got an error report from a user about this
				if (curOtherReader == null || curThisReader == null)
					return false;

				if (curThisReader.Type == curOtherReader.Type)
				{
					switch (curOtherReader.Type)
					{
						case JTokenType.Array:
							var selfArray = (RavenJArrayWrapper)curThisReader;
							var otherArray = (RavenJArrayWrapper)curOtherReader;
							if (selfArray.Length != otherArray.Length)
								return false;

							for (int i = 0; i < selfArray.Length; i++)
							{
								thisStack.Push(selfArray[i]);
								otherStack.Push(otherArray[i]);
							}
							break;
						case JTokenType.Object:
							var selfObj = (RavenJObjectWrapper)curThisReader;
							var otherObj = (RavenJObjectWrapper)curOtherReader;
							if (selfObj.Count != otherObj.Count)
								return false;

							foreach (var kvp in selfObj.Properties)
							{
								RavenJTokenWrapper tokenWrapper;
								if (otherObj.TryGetValue(kvp.Key, out tokenWrapper) == false)
									return false;
								if (kvp.Value == null)
								{
									if (tokenWrapper != null && tokenWrapper.Type != JTokenType.Null)
										return false;
									continue;
								}
								switch (kvp.Value.Type)
								{
									case JTokenType.Array:
									case JTokenType.Object:
										otherStack.Push(tokenWrapper);
										thisStack.Push(kvp.Value);
										break;
									case JTokenType.Bytes:
										var bytes = kvp.Value.Value<byte[]>();
										byte[] tokenBytes = tokenWrapper.Type == JTokenType.String
																? Convert.FromBase64String(tokenWrapper.Value<string>())
																: tokenWrapper.Value<byte[]>();
								        if (tokenBytes == null)
								            return false;
										if (bytes.Length != tokenBytes.Length)
											return false;

										if (tokenBytes.Where((t, i) => t != bytes[i]).Any())
										{
											return false;
										}

										break;
									default:
										if (!kvp.Value.DeepEquals(tokenWrapper))
											return false;
										break;
								}
							}
							break;
						default:
							if (!curOtherReader.DeepEquals(curThisReader))
								return false;
							break;
					}
				}
				else
				{
					switch (curThisReader.Type)
					{
						case JTokenType.Guid:
							if (curOtherReader.Type != JTokenType.String)
								return false;

							if (curThisReader.Value<string>() != curOtherReader.Value<string>())
								return false;

							break;
						default:
							return false;
					}
				}
			}

			return true;
		}

		internal virtual int GetDeepHashCode()
		{
			var stack = new Stack<Tuple<int, RavenJTokenWrapper>>();
			int ret = 0;

			stack.Push(Tuple.Create(0, this));
			while (stack.Count > 0)
			{
				var cur = stack.Pop();

				if (cur.Item2.Type == JTokenType.Array)
				{
					var arr = (RavenJArrayWrapper)cur.Item2;
					for (int i = 0; i < arr.Length; i++)
					{
						stack.Push(Tuple.Create(cur.Item1 ^ (i * 397), arr[i]));
					}
				}
				else if (cur.Item2.Type == JTokenType.Object)
				{
					var selfObj = (RavenJObjectWrapper)cur.Item2;
					foreach (var kvp in selfObj.Properties)
					{
						stack.Push(Tuple.Create(cur.Item1 ^ (397 * kvp.Key.GetHashCode()), kvp.Value));
					}
				}
				else // value
				{
					ret ^= cur.Item1 ^ (cur.Item2.GetDeepHashCode() * 397);
				}
			}

			return ret;
		}


		/// <summary>
		/// Selects the tokenWrapper that matches the object pathWrapper.
		/// </summary>
		/// <param name="path">
		/// The object pathWrapper from the current <see cref="RavenJTokenWrapper"/> to the <see cref="RavenJTokenWrapper"/>
		/// to be returned. This must be a string of property names or array indexes separated
		/// by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
		/// <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
		/// </param>
		/// <returns>The <see cref="RavenJTokenWrapper"/> that matches the object pathWrapper or a null reference if no matching tokenWrapper is found.</returns>
		public RavenJTokenWrapper SelectToken(string path)
		{
			return SelectToken(path, false);
		}

		/// <summary>
		/// Selects the tokenWrapper that matches the object pathWrapper.
		/// </summary>
		/// <param name="path">
		/// The object pathWrapper from the current <see cref="RavenJTokenWrapper"/> to the <see cref="RavenJTokenWrapper"/>
		/// to be returned. This must be a string of property names or array indexes separated
		/// by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
		/// <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
		/// </param>
		/// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokenWrapper is found.</param>
		/// <returns>The <see cref="RavenJTokenWrapper"/> that matches the object pathWrapper.</returns>
		public RavenJTokenWrapper SelectToken(string path, bool errorWhenNoMatch)
		{
			var p = new RavenJPathWrapper(path);
			return p.Evaluate(this, errorWhenNoMatch);
		}

        /// <summary>
        /// Selects the tokenWrapper that matches the object pathWrapper.
        /// </summary>
        /// <param name="pathWrapper">
        /// The object pathWrapper from the current <see cref="RavenJTokenWrapper"/> to the <see cref="RavenJTokenWrapper"/>
        /// to be returned. This must be a string of property names or array indexes separated
        /// by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
        /// <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
        /// </param>
        /// <returns>The <see cref="RavenJTokenWrapper"/> that matches the object pathWrapper or a null reference if no matching tokenWrapper is found.</returns>
        public RavenJTokenWrapper SelectToken(RavenJPathWrapper pathWrapper)
        {
            return SelectToken(pathWrapper, false);
        }

        /// <summary>
        /// Selects the tokenWrapper that matches the object pathWrapper.
        /// </summary>
        /// <param name="pathWrapper">
        /// The object pathWrapper from the current <see cref="RavenJTokenWrapper"/> to the <see cref="RavenJTokenWrapper"/>
        /// to be returned. This must be a string of property names or array indexes separated
        /// by periods, such as <code>Tables[0].DefaultView[0].Price</code> in C# or
        /// <code>Tables(0).DefaultView(0).Price</code> in Visual Basic.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokenWrapper is found.</param>
        /// <returns>The <see cref="RavenJTokenWrapper"/> that matches the object pathWrapper.</returns>
        public RavenJTokenWrapper SelectToken(RavenJPathWrapper pathWrapper, bool errorWhenNoMatch)
        {
            return pathWrapper.Evaluate(this, errorWhenNoMatch);
        }

		/// <summary>
		/// Returns a collection of the child values of this tokenWrapper, in document order.
		/// </summary>
		/// <typeparam name="T">The type to convert the values to.</typeparam>
		/// <returns>
		/// A <see cref="IEnumerable{T}"/> containing the child values of this <see cref="RavenJTokenWrapper"/>, in document order.
		/// </returns>
		public virtual IEnumerable<T> Values<T>()
		{
			throw new NotSupportedException();
		}

        public virtual T Value<T>()
        {
            return this.Convert<T>();
        }

		/// <summary>
		/// Returns a collection of the child values of this tokenWrapper, in document order.
		/// </summary>
		public virtual IEnumerable<RavenJTokenWrapper> Values()
		{
			throw new NotSupportedException();
		}
		internal virtual void AddForCloning(string key, RavenJTokenWrapper tokenWrapper)
		{
			// kept virtual (as opposed to abstract) to waive the new for implementing this in RavenJValueWrapper
		}

		internal virtual IEnumerable<KeyValuePair<string, RavenJTokenWrapper>> GetCloningEnumerator()
		{
			return null;
		}

		#region Cast to operators
		/// <summary>
		/// Performs an implicit conversion from <see cref="Boolean"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(bool value)
		{
			return new RavenJValueWrapper(value);
		}

#if !PocketPC && !NET20
		/// <summary>
		/// Performs an implicit conversion from <see cref="DateTimeOffset"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(DateTimeOffset value)
		{
			return new RavenJValueWrapper(value);
		}
#endif

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Boolean}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(bool? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Int64}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(long value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{DateTime}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(DateTime? value)
		{
			return new RavenJValueWrapper(value);
		}

#if !PocketPC && !NET20
		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{DateTimeOffset}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(DateTimeOffset? value)
		{
			return new RavenJValueWrapper(value);
		}
#endif

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Decimal}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(decimal? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Double}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(double? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Int16"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(short value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="UInt16"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(ushort value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Int32"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(int value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Int32}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(int? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="DateTime"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(DateTime value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Int64}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(long? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Single}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(float? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Decimal"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(decimal value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{Int16}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(short? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{UInt16}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(ushort? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{UInt32}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(uint? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Nullable{UInt64}"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(ulong? value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Double"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(double value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Single"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(float value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="String"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(string value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="UInt32"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(uint value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="UInt64"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public static implicit operator RavenJTokenWrapper(ulong value)
		{
			return new RavenJValueWrapper(value);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="T:System.Byte[]"/> to <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <param name="value">The value to create a <see cref="RavenJValueWrapper"/> from.</param>
		/// <returns>The <see cref="RavenJValueWrapper"/> initialized with the specified value.</returns>
		public static implicit operator RavenJTokenWrapper(byte[] value)
		{
			return new RavenJValueWrapper(value);
		}
		#endregion

		public static async Task<RavenJTokenWrapper> ReadFromAsync(JsonTextReaderAsync reader)
		{
			if (reader.TokenType == JsonToken.None)
			{
				if (!await reader.ReadAsync())
					throw new Exception("Error reading RavenJTokenWrapper from JsonReader.");
			}

			switch (reader.TokenType)
			{
				case JsonToken.StartObject:
					return await RavenJObjectWrapper.LoadAsync(reader);
				case JsonToken.StartArray:
					return await RavenJArrayWrapper.LoadAsync(reader);
				case JsonToken.String:
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.Date:
				case JsonToken.Boolean:
				case JsonToken.Bytes:
				case JsonToken.Null:
				case JsonToken.Undefined:
					return new RavenJValueWrapper(reader.Value);
			}

			throw new Exception("Error reading RavenJTokenWrapper from JsonReader. Unexpected tokenWrapper: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

		}
	}
}
