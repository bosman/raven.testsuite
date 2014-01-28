using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;

namespace Raven.TestSuite.Common.Abstractions.Json.Linq
{
	/// <summary>
	/// Represents a writer that provides a fast, non-cached, forward-only way of generating Json data.
	/// </summary>
	public class RavenJTokenWriter : JsonWriter
	{
		private RavenJTokenWrapper _tokenWrapper;
		private RavenJValueWrapper _valueWrapper;
		private readonly Stack<RavenJTokenWrapper> _tokenStack = new Stack<RavenJTokenWrapper>();

		protected RavenJTokenWrapper CurrentTokenWrapper { get { return (_tokenStack.Count == 0) ? null : _tokenStack.Peek(); } }

		/// <summary>
		/// Gets the tokenWrapper being writen.
		/// </summary>
		/// <valueWrapper>The tokenWrapper being writen.</valueWrapper>
		public RavenJTokenWrapper TokenWrapper
		{
			get
			{
				if (_tokenWrapper != null)
					return _tokenWrapper;

				return _valueWrapper;
			}
		}

		/// <summary>
		/// Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
		/// </summary>
		public override void Flush()
		{
		}

		private string _tempPropName;

		public override void WritePropertyName(string name)
		{
			base.WritePropertyName(name);

			if (_tempPropName != null)
				throw new JsonWriterException("Was not expecting a property name here");

			_tempPropName = name;
		}

		private void AddParent(RavenJTokenWrapper tokenWrapper)
		{
			if (_tokenWrapper == null)
			{
				_tokenWrapper = tokenWrapper;
				_tokenStack.Push(_tokenWrapper);
				return;
			}

			switch (CurrentTokenWrapper.Type)
			{
				case JTokenType.Object:
					((RavenJObjectWrapper)CurrentTokenWrapper)[_tempPropName] = tokenWrapper;
					_tempPropName = null;
					break;
				case JTokenType.Array:
					((RavenJArrayWrapper)CurrentTokenWrapper).Add(tokenWrapper);
					break;
				default:
					throw new JsonWriterException("Unexpected tokenWrapper: " + CurrentTokenWrapper.Type);
			}

			_tokenStack.Push(tokenWrapper);
		}

		private void RemoveParent()
		{
			_tokenStack.Pop();
		}

		public override void WriteStartObject()
		{
			base.WriteStartObject();

			AddParent(new RavenJObjectWrapper());
		}

		/// <summary>
		/// Writes the beginning of a Json array.
		/// </summary>
		public override void WriteStartArray()
		{
			base.WriteStartArray();

			AddParent(new RavenJArrayWrapper());
		}

		/// <summary>
		/// Writes the end.
		/// </summary>
		/// <param name="token">The tokenWrapper.</param>
		protected override void WriteEnd(JsonToken token)
		{
			RemoveParent();
		}

		private void AddValue(object value, JsonToken token)
		{
			AddValue(new RavenJValueWrapper(value), token);
		}

		internal void AddValue(RavenJValueWrapper valueWrapper, JsonToken token)
		{
			if (_tokenStack.Count == 0)
				_valueWrapper = valueWrapper;
			else
			{
				switch (CurrentTokenWrapper.Type)
				{
					case JTokenType.Object:
						((RavenJObjectWrapper)CurrentTokenWrapper)[_tempPropName] = valueWrapper;
						_tempPropName = null;
						break;
					case JTokenType.Array:
						((RavenJArrayWrapper)CurrentTokenWrapper).Add(valueWrapper);
						break;
					default:
						throw new JsonWriterException("Unexpected tokenWrapper: " + token);
				}
			}
		}

		public override void WriteRaw(string json)
		{
			throw new NotSupportedException();
		}

		#region WriteValue methods
		/// <summary>
		/// Writes a null valueWrapper.
		/// </summary>
		public override void WriteNull()
		{
			base.WriteNull();
			AddValue(new RavenJValueWrapper(null, JTokenType.Null), JsonToken.Null);
		}

		/// <summary>
		/// Writes an undefined valueWrapper.
		/// </summary>
		public override void WriteUndefined()
		{
			base.WriteUndefined();
			AddValue(new RavenJValueWrapper(null, JTokenType.Null), JsonToken.Undefined);
		}

		/// <summary>
		/// Writes a <see cref="String"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="String"/> valueWrapper to write.</param>
		public override void WriteValue(string value)
		{
			base.WriteValue(value);
			if (value == null)
			{
				AddValue(new RavenJValueWrapper(null, JTokenType.Null), JsonToken.Null);
			}
			else
			{
				AddValue(value, JsonToken.String);
			}
		}

		/// <summary>
		/// Writes a <see cref="Int32"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Int32"/> valueWrapper to write.</param>
		public override void WriteValue(int value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="UInt32"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="UInt32"/> valueWrapper to write.</param>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public override void WriteValue(uint value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="Int64"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Int64"/> valueWrapper to write.</param>
		public override void WriteValue(long value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="UInt64"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="UInt64"/> valueWrapper to write.</param>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public override void WriteValue(ulong value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="Single"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Single"/> valueWrapper to write.</param>
		public override void WriteValue(float value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Float);
		}

		/// <summary>
		/// Writes a <see cref="Double"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Double"/> valueWrapper to write.</param>
		public override void WriteValue(double value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Float);
		}

		/// <summary>
		/// Writes a <see cref="Boolean"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Boolean"/> valueWrapper to write.</param>
		public override void WriteValue(bool value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Boolean);
		}

		/// <summary>
		/// Writes a <see cref="Int16"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Int16"/> valueWrapper to write.</param>
		public override void WriteValue(short value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="UInt16"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="UInt16"/> valueWrapper to write.</param>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public override void WriteValue(ushort value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="Char"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Char"/> valueWrapper to write.</param>
		public override void WriteValue(char value)
		{
			base.WriteValue(value);
			AddValue(value.ToString(), JsonToken.String);
		}

		/// <summary>
		/// Writes a <see cref="Byte"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Byte"/> valueWrapper to write.</param>
		public override void WriteValue(byte value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="SByte"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="SByte"/> valueWrapper to write.</param>
#if !SILVERLIGHT
		[CLSCompliant(false)]
#endif
		public override void WriteValue(sbyte value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Integer);
		}

		/// <summary>
		/// Writes a <see cref="Decimal"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Decimal"/> valueWrapper to write.</param>
		public override void WriteValue(decimal value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Float);
		}

		/// <summary>
		/// Writes a <see cref="DateTime"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="DateTime"/> valueWrapper to write.</param>
		public override void WriteValue(DateTime value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Date);
		}

#if !PocketPC && !NET20
		/// <summary>
		/// Writes a <see cref="DateTimeOffset"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="DateTimeOffset"/> valueWrapper to write.</param>
		public override void WriteValue(DateTimeOffset value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Date);
		}
#endif

		/// <summary>
		/// Writes a <see cref="T:Byte[]"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="T:Byte[]"/> valueWrapper to write.</param>
		public override void WriteValue(byte[] value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.Bytes);
		}

		/// <summary>
		/// Writes a <see cref="Guid"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Guid"/> valueWrapper to write.</param>
		public override void WriteValue(Guid value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.String);
		}

		/// <summary>
		/// Writes a <see cref="TimeSpan"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="TimeSpan"/> valueWrapper to write.</param>
		public override void WriteValue(TimeSpan value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.String);
		}

		/// <summary>
		/// Writes a <see cref="Uri"/> valueWrapper.
		/// </summary>
		/// <param name="value">The <see cref="Uri"/> valueWrapper to write.</param>
		public override void WriteValue(Uri value)
		{
			base.WriteValue(value);
			AddValue(value, JsonToken.String);
		}
		
		#endregion
	}
}
