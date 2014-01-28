using System;
using System.Collections;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json.Linq;
using System.Linq;

namespace Raven.TestSuite.Common.Abstractions.Json.Linq
{
	internal class DictionaryWithParentSnapshot : IDictionary<string, RavenJTokenWrapper>
	{
		private readonly IEqualityComparer<string> comparer;
		private static readonly RavenJTokenWrapper DeletedMarker = new RavenJValueWrapper("*DeletedMarker*", JTokenType.Null);

		private readonly DictionaryWithParentSnapshot parentSnapshot;
		private int count;
		private IDictionary<string, RavenJTokenWrapper> localChanges;
		private string snapshotMsg;

		protected IDictionary<string, RavenJTokenWrapper> LocalChanges
		{
			get
			{
				if (localChanges == null)
					localChanges = new Dictionary<string, RavenJTokenWrapper>(comparer);
				return localChanges;
			}
		}

		public DictionaryWithParentSnapshot(IEqualityComparer<string> comparer)
		{
			this.comparer = comparer;
		}

		private DictionaryWithParentSnapshot(DictionaryWithParentSnapshot previous)
		{
			comparer = previous.comparer;
			parentSnapshot = previous;
		}

		#region Dictionary<string,TValue> Members

		public void Add(string key, RavenJTokenWrapper value)
		{
			if (IsSnapshot)
				throw new InvalidOperationException(snapshotMsg ?? "Cannot modify a snapshot, this is probably a bug");

			if (ContainsKey(key))
				throw new ArgumentException(string.Format("An item with the same key has already been added: '{0}'", key));

			count += 1;
			LocalChanges[key] = value; // we can't use Add, because LocalChanges may contain a DeletedMarker
		}

		public bool ContainsKey(string key)
		{
			RavenJTokenWrapper tokenWrapper;
			if (localChanges != null && localChanges.TryGetValue(key, out tokenWrapper))
			{
				if (tokenWrapper == DeletedMarker)
					return false;
				return true;
			}
			return (parentSnapshot != null && parentSnapshot.TryGetValue(key, out tokenWrapper) && tokenWrapper != DeletedMarker);
		}

		public ICollection<string> Keys
		{
			get
			{
				if (localChanges == null)
				{
					if (parentSnapshot != null)
					{
						return parentSnapshot.Keys;
					}
					return new HashSet<string>();
				}

				ICollection<string> ret = new HashSet<string>();
				if (parentSnapshot != null)
				{
					foreach (var key in parentSnapshot.Keys)
					{
						if (LocalChanges.ContainsKey(key))
							continue;
						ret.Add(key);
					}
				}

				foreach (var key in LocalChanges.Keys)
				{
					RavenJTokenWrapper value;
					if (LocalChanges.TryGetValue(key, out value) == false ||
						value == DeletedMarker)
						continue;
					ret.Add(key);
				}

				return ret;
			}
		}

		public bool Remove(string key)
		{
			if (IsSnapshot)
				throw new InvalidOperationException("Cannot modify a snapshot, this is probably a bug");

			RavenJTokenWrapper parentTokenWrapper = null;

			bool parentHasIt = parentSnapshot != null &&
							   parentSnapshot.TryGetValue(key, out parentTokenWrapper);

			RavenJTokenWrapper tokenWrapper;
			if (LocalChanges.TryGetValue(key, out tokenWrapper) == false)
			{
				if (parentHasIt && parentTokenWrapper != DeletedMarker)
				{
					LocalChanges[key] = DeletedMarker; 
					count -= 1;
					return true;
				}
				return false;
			}
			if (tokenWrapper == DeletedMarker)
				return false;
			count -= 1;
			LocalChanges[key] = DeletedMarker;
			return true;
		}

		public bool TryGetValue(string key, out RavenJTokenWrapper value)
		{
			value = null;
			RavenJTokenWrapper unsafeVal;
			if (localChanges != null && localChanges.TryGetValue(key, out unsafeVal))
			{
				if (unsafeVal == DeletedMarker)
					return false;

				value = unsafeVal;
				return true;
			}

			if (parentSnapshot == null ||
				!parentSnapshot.TryGetValue(key, out unsafeVal) ||
				unsafeVal == DeletedMarker)
				return false;

			if (IsSnapshot == false && unsafeVal != null)
			{
				if (unsafeVal.IsSnapshot == false && unsafeVal.Type != JTokenType.Object)
					unsafeVal.EnsureCannotBeChangeAndEnableSnapshotting();
			}

			value = unsafeVal;

			return true;
		}

		public ICollection<RavenJTokenWrapper> Values
		{
			get
			{
				ICollection<RavenJTokenWrapper> ret = new HashSet<RavenJTokenWrapper>();
				foreach (var key in Keys)
				{
					ret.Add(this[key]);
				}
				return ret;
			}
		}

		public RavenJTokenWrapper this[string key]
		{
			get
			{
				RavenJTokenWrapper tokenWrapper;
				if (TryGetValue(key, out tokenWrapper))
					return tokenWrapper;
				throw new KeyNotFoundException(key);
			}
			set
			{
				if (IsSnapshot)
					throw new InvalidOperationException("Cannot modify a snapshot, this is probably a bug");
				var isInsert = ContainsKey(key) == false;
				LocalChanges[key] = value;
				if (isInsert)
					count += 1;
			}
		}

		#endregion

		public IEnumerator<KeyValuePair<string, RavenJTokenWrapper>> GetEnumerator()
		{
			if (parentSnapshot != null)
			{
				foreach (var item in parentSnapshot)
				{
					if (item.Key == null)
						continue;
					if (LocalChanges.ContainsKey(item.Key))
						continue;
					yield return item;
				}
			}
			foreach (var localChange in LocalChanges)
			{
				if (localChange.Value == DeletedMarker)
					continue;
				yield return localChange;
			}

		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<string, RavenJTokenWrapper> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			// we either already have count set to -1, or it will be invalidated by a call to Remove below

			foreach (var key in Keys.ToArray()) // clone the values for the iteration
			{
				Remove(key);
			}
		}

		public bool Contains(KeyValuePair<string, RavenJTokenWrapper> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<string, RavenJTokenWrapper>[] array, int arrayIndex)
		{
			if (parentSnapshot != null)
			{
				parentSnapshot.CopyTo(array, arrayIndex);
				arrayIndex += parentSnapshot.Count;
			}
			LocalChanges.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, RavenJTokenWrapper> item)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get
			{
				if (parentSnapshot != null)
					return count + parentSnapshot.Count;
				return count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool CaseInsensitivePropertyNames { get; set; }
		public bool IsSnapshot { get; private set; }

		public DictionaryWithParentSnapshot CreateSnapshot()
		{
			if (IsSnapshot == false)
				throw new InvalidOperationException("Cannot create snapshot without previously calling EnsureSnapShot");
			return new DictionaryWithParentSnapshot(this);
		}

		public void EnsureSnapshot(string msg = null)
		{
			snapshotMsg = msg;
			IsSnapshot = true;
		}
	}
}
