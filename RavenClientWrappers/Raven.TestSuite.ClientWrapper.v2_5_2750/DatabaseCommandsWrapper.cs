﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Raven.Client.Connection;
using Raven.Json.Linq;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.Common.WrapperInterfaces;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.TestSuite.ClientWrapper.v2_5_2750.Extensions;
using Raven.TestSuite.Common.Data;
using Raven.TestSuite.Common.Indexing;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DatabaseCommandsWrapper : IDatabaseCommandsWrapper
    {
        private readonly IDatabaseCommands inner;

        public DatabaseCommandsWrapper(IDatabaseCommands databaseCommands)
        {
            inner = databaseCommands;
        }

        public NameValueCollection OperationsHeaders { get; set; }

        public IJsonDocumentWrapper[] StartsWith(string keyPrefix, string matches, int start, int pageSize, bool metadataOnly = false,
                                                 string exclude = null)
        {
            return inner.StartsWith(keyPrefix, matches, start, pageSize, metadataOnly, exclude)
                 .Select(d => new JsonDocumentWrapper(d)).ToArray<IJsonDocumentWrapper>();
        }

        public IJsonDocumentWrapper Get(string key)
        {
            return new JsonDocumentWrapper(inner.Get(key));
        }

        public MultiLoadResultWrapper Get(string[] ids, string[] includes, string transformer = null, Dictionary<string, RavenJTokenWrapper> queryInputs = null,
                                           bool metadataOnly = false)
        {
            Dictionary<string, RavenJToken> qi = null;
            if (queryInputs != null)
            {
                qi = queryInputs.ToDictionary(queryInput => queryInput.Key, queryInput => queryInput.Value.Unwrap());
            }
            var multiLoadResult = inner.Get(ids, includes, transformer, qi, metadataOnly);
            var result = new MultiLoadResultWrapper
                {
                    Includes = multiLoadResult.Includes.Select(i => i.Wrap()).ToList(),
                    Results = multiLoadResult.Results.Select(r => r.Wrap()).ToList()
                };
            return result;
        }

        public IJsonDocumentWrapper[] GetDocuments(int start, int pageSize, bool metadataOnly = false)
        {
            var docs = inner.GetDocuments(start, pageSize, metadataOnly);
            return docs.Select(d => new JsonDocumentWrapper(d) as IJsonDocumentWrapper).ToArray();
        }

        public PutResultWrapper Put(string key, EtagWrapper etag, RavenJObjectWrapper document, RavenJObjectWrapper metadata)
        {
            return inner.Put(key, etag.Unwrap(), document.Unwrap(), metadata.Unwrap()).Wrap();
        }

        public void Delete(string key, EtagWrapper etag)
        {
            inner.Delete(key, etag.Unwrap());
        }

        public void PutAttachment(string key, EtagWrapper etag, Stream data, RavenJObjectWrapper metadata)
        {
            inner.PutAttachment(key, etag.Unwrap(), data, metadata.Unwrap());
        }

        public void UpdateAttachmentMetadata(string key, EtagWrapper etag, RavenJObjectWrapper metadata)
        {
            inner.UpdateAttachmentMetadata(key, etag.Unwrap(), metadata.Unwrap());
        }

        public AttachmentWrapper GetAttachment(string key)
        {
            return inner.GetAttachment(key).Wrap();
        }

        public IEnumerable<AttachmentWrapper> GetAttachmentHeadersStartingWith(string idPrefix, int start, int pageSize)
        {
            return
                inner.GetAttachmentHeadersStartingWith(idPrefix, start, pageSize)
                     .Select(a => a.Wrap());
        }

        public AttachmentWrapper HeadAttachment(string key)
        {
            return inner.HeadAttachment(key).Wrap();
        }

        public void DeleteAttachment(string key, EtagWrapper etag)
        {
            inner.DeleteAttachment(key, etag.Unwrap());
        }

        public string[] GetDatabaseNames(int pageSize, int start = 0)
        {
            return inner.GetDatabaseNames(pageSize, start);
        }

        public string[] GetIndexNames(int start, int pageSize)
        {
            return inner.GetIndexNames(start, pageSize);
        }

        public IndexDefinitionWrapper[] GetIndexes(int start, int pageSize)
        {
            return inner.GetIndexes(start, pageSize).Select(x => x.Wrap()).ToArray();
        }

        public void ResetIndex(string name)
        {
            inner.ResetIndex(name);
        }

        public IndexDefinitionWrapper GetIndex(string name)
        {
            return inner.GetIndex(name).Wrap();
        }

        public string PutIndex(string name, IndexDefinitionWrapper indexDef)
        {
            return inner.PutIndex(name, indexDef.Unwrap());
        }

        public string PutTransformer(string name, ITransformerDefinitionWrapper indexDef)
        {
            throw new NotImplementedException();
        }

        public string PutIndex(string name, IndexDefinitionWrapper indexDef, bool overwrite)
        {
            return inner.PutIndex(name, indexDef.Unwrap(), overwrite);
        }

        public string PutIndex<TDocument, TReduceResult>(string name, IIndexDefinitionBuilderWrapper<TDocument, TReduceResult> indexDef)
        {
            throw new NotImplementedException();
        }

        public string PutIndex<TDocument, TReduceResult>(string name, IIndexDefinitionBuilderWrapper<TDocument, TReduceResult> indexDef, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<RavenJObjectWrapper> StreamDocs(EtagWrapper fromEtag = null, string startsWith = null, string matches = null, int start = 0,
                                      int pageSize = Int32.MaxValue, string exclude = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteIndex(string name)
        {
            inner.DeleteIndex(name);
        }

        public void Commit(string txId)
        {
            inner.Commit(txId);
        }

        public void Rollback(string txId)
        {
            inner.Rollback(txId);
        }

        public IDatabaseCommandsWrapper With(ICredentials credentialsForSession)
        {
            throw new NotImplementedException();
        }

        public IOperationWrapper DeleteByIndex(string indexName, IIndexQueryWrapper queryToDelete)
        {
            throw new NotImplementedException();
        }

        public IOperationWrapper DeleteByIndex(string indexName, IIndexQueryWrapper queryToDelete, bool allowStale)
        {
            throw new NotImplementedException();
        }

        public IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IPatchRequestWrapper[] patchRequests)
        {
            throw new NotImplementedException();
        }

        public IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IScriptedPatchRequestWrapper patch)
        {
            throw new NotImplementedException();
        }

        public IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IPatchRequestWrapper[] patchRequests,
                                               bool allowStale)
        {
            throw new NotImplementedException();
        }

        public IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IScriptedPatchRequestWrapper patch,
                                               bool allowStale)
        {
            throw new NotImplementedException();
        }

        public IDatabaseCommandsWrapper ForDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public IDatabaseCommandsWrapper ForSystemDatabase()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTerms(string index, string field, string fromValue, int pageSize)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patches)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patches, bool ignoreMissing)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patch)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patch, bool ignoreMissing)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patches, EtagWrapper etag)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patchesToExisting, IPatchRequestWrapper[] patchesToDefault,
                                          RavenJObjectWrapper defaultMetadata)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patch, EtagWrapper etag)
        {
            throw new NotImplementedException();
        }

        public RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patchExisting,
                                          IScriptedPatchRequestWrapper patchDefault, RavenJObjectWrapper defaultMetadata)
        {
            throw new NotImplementedException();
        }

        public IDisposable DisableAllCaching()
        {
            throw new NotImplementedException();
        }

        public long NextIdentityFor(string name)
        {
            throw new NotImplementedException();
        }

        public long SeedIdentityFor(string name, long value)
        {
            throw new NotImplementedException();
        }

        public string UrlFor(string documentKey)
        {
            throw new NotImplementedException();
        }

        public IDisposable ForceReadFromMaster()
        {
            throw new NotImplementedException();
        }

        public ITransformerDefinitionWrapper[] GetTransformers(int start, int pageSize)
        {
            throw new NotImplementedException();
        }

        public ITransformerDefinitionWrapper GetTransformer(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteTransformer(string name)
        {
            inner.DeleteTransformer(name);
        }

        public void PrepareTransaction(string txId)
        {
            throw new NotImplementedException();
        }

        public DatabaseStatisticsWrapper GetStatistics()
        {
            return inner.GetStatistics().Wrap();
        }
    }
}