using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;
using Raven.Imports.Newtonsoft.Json.Utilities;
using Raven.TestSuite.Common.Abstractions.Imports.Newtonsoft.Json;

namespace Raven.TestSuite.Common.Abstractions.Json.Linq
{
	/// <summary>
	/// Represents a JSON array.
	/// </summary>
	public class RavenJArrayWrapper : RavenJTokenWrapper, IEnumerable<RavenJTokenWrapper>
	{
		private bool isSnapshot;

		/// <summary>
		/// Initializes a new instance of the <see cref="RavenJArrayWrapper"/> class.
		/// </summary>
		public RavenJArrayWrapper()
		{
			Items = new List<RavenJTokenWrapper>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RavenJArrayWrapper"/> class with the specified content.
		/// </summary>
		/// <param name="content">The contents of the array.</param>
		public RavenJArrayWrapper(IEnumerable content)
		{
			Items = new List<RavenJTokenWrapper>();
			if (content == null)
				return;

			var ravenJToken = content as RavenJTokenWrapper;
			if (ravenJToken != null && ravenJToken.Type != JTokenType.Array)
			{
				Items.Add(ravenJToken);
			}
			else
			{
				foreach (var item in content)
				{
					ravenJToken = item as RavenJTokenWrapper;
					Items.Add(ravenJToken ?? new RavenJValueWrapper(item));
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RavenJArrayWrapper"/> class with the specified content.
		/// </summary>
		/// <param name="content">The contents of the array.</param>
		public RavenJArrayWrapper(params object[] content)
			: this((IEnumerable)content)
		{

		}

		public RavenJArrayWrapper(IEnumerable<RavenJTokenWrapper> content)
		{
			Items = new List<RavenJTokenWrapper>();
			if (content != null)
			{
				Items.AddRange(content);
			}
		}

		/// <summary>
		/// Gets the node type for this <see cref="RavenJTokenWrapper"/>.
		/// </summary>
		/// <valueWrapper>The type.</valueWrapper>
		public override JTokenType Type
		{
			get { return JTokenType.Array; }
		}

		/// <summary>
		/// Gets or sets the <see cref="RavenJTokenWrapper"/> at the specified index.
		/// </summary>
		/// <valueWrapper></valueWrapper>
		public RavenJTokenWrapper this[int index]
		{
			get { return Items[index]; }
			set
			{
				if (isSnapshot)
					throw new InvalidOperationException("Cannot modify a snapshot, this is probably a bug");

				Items[index] = value;
			}
		}

		public override RavenJTokenWrapper CloneToken()
		{
			return CloneTokenImpl(new RavenJArrayWrapper());
		}

		public override bool IsSnapshot
		{
			get { return isSnapshot; }
		}

		public int Length { get { return Items.Count; } }

		private List<RavenJTokenWrapper> Items { get; set; }

		public new static RavenJArrayWrapper Load(JsonReader reader)
		{
			if (reader.TokenType == JsonToken.None)
			{
				if (!reader.Read())
					throw new Exception("Error reading RavenJArrayWrapper from JsonReader.");
			}

			if (reader.TokenType != JsonToken.StartArray)
				throw new Exception("Error reading RavenJArrayWrapper from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

			if (reader.Read() == false)
				throw new Exception("Unexpected end of json array");

			var ar = new RavenJArrayWrapper();
			RavenJTokenWrapper val = null;
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.Comment:
						// ignore comments
						break;
					case JsonToken.EndArray:
						return ar;
					case JsonToken.StartObject:
						val = RavenJObjectWrapper.Load(reader);
						ar.Items.Add(val);
						break;
					case JsonToken.StartArray:
						val = RavenJArrayWrapper.Load(reader);
						ar.Items.Add(val);
						break;
					default:
						val = RavenJValueWrapper.Load(reader);
						ar.Items.Add(val);
						break;
				}
			} while (reader.Read());

			throw new Exception("Error reading RavenJArrayWrapper from JsonReader.");
		}

		/// <summary>
		/// Load a <see cref="RavenJArrayWrapper"/> from a string that contains JSON.
		/// </summary>
		/// <param name="json">A <see cref="String"/> that contains JSON.</param>
		/// <returns>A <see cref="RavenJArrayWrapper"/> populated from the string that contains JSON.</returns>
		public new static RavenJArrayWrapper Parse(string json)
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
			writer.WriteStartArray();

			if (Items != null)
			{
				foreach (var token in Items)
				{
					token.WriteTo(writer, converters);
				}
			}

			writer.WriteEndArray();
		}

		#region IEnumerable<RavenJTokenWrapper> Members

		public IEnumerator<RavenJTokenWrapper> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		#endregion

		internal override IEnumerable<KeyValuePair<string, RavenJTokenWrapper>> GetCloningEnumerator()
		{
			return Items.Select(i => new KeyValuePair<string, RavenJTokenWrapper>(null, i));
		}

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion


		public void Add(RavenJTokenWrapper tokenWrapper)
		{
			if (isSnapshot)
				throw new InvalidOperationException("Cannot modify a snapshot, this is probably a bug");

			Items.Add(tokenWrapper);
		}

		public bool Remove(RavenJTokenWrapper tokenWrapper)
		{
			if (isSnapshot)
				throw new InvalidOperationException("Cannot modify a snapshot, this is probably a bug");

			return Items.Remove(tokenWrapper);
		}

		public void RemoveAt(int index)
		{
			if (isSnapshot)
				throw new InvalidOperationException("Cannot modify a snapshot, this is probably a bug");

			Items.RemoveAt(index);
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
		public void Insert(int index, RavenJTokenWrapper item)
		{
			if (isSnapshot)
				throw new InvalidOperationException("Cannot modify a snapshot, this is probably a bug");

			Items.Insert(index, item);
		}

		public override IEnumerable<T> Values<T>()
		{
			return Items.Convert<T>();
		}

		public override IEnumerable<RavenJTokenWrapper> Values()
		{
			return Items;
		}

		internal override void AddForCloning(string key, RavenJTokenWrapper tokenWrapper)
		{
			Add(tokenWrapper);
		}

		public override void EnsureCannotBeChangeAndEnableSnapshotting()
		{
			isSnapshot = true;
		}

		public override RavenJTokenWrapper CreateSnapshot()
		{
			if (isSnapshot == false)
				throw new InvalidOperationException("Cannot create snapshot without previously calling EnsureSnapShot");

			return new RavenJArrayWrapper(Items);
		}

		public static async Task<RavenJTokenWrapper> LoadAsync(JsonTextReaderAsync reader)
		{
			if (reader.TokenType == JsonToken.None)
			{
				if (!await reader.ReadAsync())
					throw new Exception("Error reading RavenJArrayWrapper from JsonReader.");
			}

			if (reader.TokenType != JsonToken.StartArray)
				throw new Exception("Error reading RavenJArrayWrapper from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

			if (await reader.ReadAsync() == false)
				throw new Exception("Unexpected end of json array");

			var ar = new RavenJArrayWrapper();
			RavenJTokenWrapper val = null;
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.Comment:
						// ignore comments
						break;
					case JsonToken.EndArray:
						return ar;
					case JsonToken.StartObject:
						val = await RavenJObjectWrapper.LoadAsync(reader);
						ar.Items.Add(val);
						break;
					case JsonToken.StartArray:
						val = await RavenJArrayWrapper.LoadAsync(reader);
						ar.Items.Add(val);
						break;
					default:
						val = RavenJValueWrapper.Load(reader);
						ar.Items.Add(val);
						break;
				}
			} while (await reader.ReadAsync());

			throw new Exception("Error reading RavenJArrayWrapper from JsonReader.");
		}
	}
}