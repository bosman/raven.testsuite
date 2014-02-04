using System;
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

        public IMultiLoadResultWrapper Get(string[] ids, string[] includes, string transformer = null, Dictionary<string, RavenJTokenWrapper> queryInputs = null,
                                           bool metadataOnly = false)
        {
            Dictionary<string, RavenJToken> qi = null;
            if (queryInputs != null)
            {
                qi = queryInputs.ToDictionary(queryInput => queryInput.Key, queryInput => RavenJToken.Parse(queryInput.Value.ToString()));
            }
            var multiLoadResult = inner.Get(ids, includes, transformer, qi, metadataOnly);
            var result = new MultiLoadResultWrapper
                {
                    Includes = multiLoadResult.Includes.Select(i => RavenJObjectWrapper.Parse(i.ToString())).ToList(),
                    Results = multiLoadResult.Results.Select(r => RavenJObjectWrapper.Parse(r.ToString())).ToList()
                };
            return result;
        }

        public IJsonDocumentWrapper[] GetDocuments(int start, int pageSize, bool metadataOnly = false)
        {
            var docs = inner.GetDocuments(start, pageSize, metadataOnly);
            return docs.Select(d => new JsonDocumentWrapper(d) as IJsonDocumentWrapper).ToArray();
        }

        public IPutResultWrapper Put(string key, EtagWrapper etag, RavenJObjectWrapper document, RavenJObjectWrapper metadata)
        {
            //TODO: 
            throw new NotImplementedException();
            /*
            return
                new PutResultWrapper(inner.Put(key, (Etag)etag.Unwrap(), (RavenJObject) document.Unwrap(),
                                               (RavenJObject)metadata.Unwrap()));*/
        }

        public void Delete(string key, EtagWrapper etag)
        {
           inner.Delete(key, Etag.Parse(etag.ToString()));
        }

        public void PutAttachment(string key, EtagWrapper etag, Stream data, RavenJObjectWrapper metadata)
        {
            inner.PutAttachment(key, Etag.Parse(etag.ToString()), data, RavenJObject.Parse(metadata.ToString()));
        }

        public void UpdateAttachmentMetadata(string key, EtagWrapper etag, RavenJObjectWrapper metadata)
        {
            inner.UpdateAttachmentMetadata(key, Etag.Parse(etag.ToString()), RavenJObject.Parse(metadata.ToString()));
        }

        public IAttachmentWrapper GetAttachment(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAttachmentWrapper> GetAttachmentHeadersStartingWith(string idPrefix, int start, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IAttachmentWrapper HeadAttachment(string key)
        {
            throw new NotImplementedException();
        }

        public void DeleteAttachment(string key, EtagWrapper etag)
        {
            inner.DeleteAttachment(key, Etag.Parse(etag.ToString()));
        }

        public string[] GetDatabaseNames(int pageSize, int start = 0)
        {
            return inner.GetDatabaseNames(pageSize, start);
        }

        public string[] GetIndexNames(int start, int pageSize)
        {
            return inner.GetIndexNames(start, pageSize);
        }

        public IIndexDefinitionWrapper[] GetIndexes(int start, int pageSize)
        {
            throw new NotImplementedException();
        }

        public void ResetIndex(string name)
        {
            inner.ResetIndex(name);
        }

        public IIndexDefinitionWrapper GetIndex(string name)
        {
            throw new NotImplementedException();
        }

        public string PutIndex(string name, IIndexDefinitionWrapper indexDef)
        {
            throw new NotImplementedException();
        }

        public string PutTransformer(string name, ITransformerDefinitionWrapper indexDef)
        {
            throw new NotImplementedException();
        }

        public string PutIndex(string name, IIndexDefinitionWrapper indexDef, bool overwrite)
        {
            throw new NotImplementedException();
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
    }
}