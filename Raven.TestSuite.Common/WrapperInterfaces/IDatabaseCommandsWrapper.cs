using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.ClientWrapper.v2_5_2750;
using Raven.TestSuite.Common.Data;
using Raven.TestSuite.Common.Indexing;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IDatabaseCommandsWrapper
    {
        NameValueCollection OperationsHeaders { get; set; }

        IJsonDocumentWrapper[] StartsWith(string keyPrefix, string matches, int start, int pageSize, bool metadataOnly = false, string exclude = null);

        IJsonDocumentWrapper Get(string key);

        MultiLoadResultWrapper Get(string[] ids, string[] includes, string transformer = null, Dictionary<string, RavenJTokenWrapper> queryInputs = null, bool metadataOnly = false);

        IJsonDocumentWrapper[] GetDocuments(int start, int pageSize, bool metadataOnly = false);

        PutResultWrapper Put(string key, EtagWrapper etag, RavenJObjectWrapper document, RavenJObjectWrapper metadata);

        void Delete(string key, EtagWrapper etag);

        void PutAttachment(string key, EtagWrapper etag, Stream data, RavenJObjectWrapper metadata);

        void UpdateAttachmentMetadata(string key, EtagWrapper etag, RavenJObjectWrapper metadata);

        AttachmentWrapper GetAttachment(string key);

        IEnumerable<AttachmentWrapper> GetAttachmentHeadersStartingWith(string idPrefix, int start, int pageSize);

        AttachmentWrapper HeadAttachment(string key);

        void DeleteAttachment(string key, EtagWrapper etag);

        string[] GetDatabaseNames(int pageSize, int start = 0);

        string[] GetIndexNames(int start, int pageSize);

        IndexDefinitionWrapper[] GetIndexes(int start, int pageSize);

        void ResetIndex(string name);

        IndexDefinitionWrapper GetIndex(string name);

        string PutIndex(string name, IndexDefinitionWrapper indexDef);

        string PutTransformer(string name, ITransformerDefinitionWrapper indexDef);

        string PutIndex(string name, IndexDefinitionWrapper indexDef, bool overwrite);

        string PutIndex<TDocument, TReduceResult>(string name, IIndexDefinitionBuilderWrapper<TDocument, TReduceResult> indexDef);

        string PutIndex<TDocument, TReduceResult>(string name, IIndexDefinitionBuilderWrapper<TDocument, TReduceResult> indexDef, bool overwrite);

        //TODO: QueryResult Query(string index, IIndexQueryWrapper query, string[] includes, bool metadataOnly = false, bool indexEntriesOnly = false);

        //TODO: IEnumerator<RavenJObjectWrapper> StreamQuery(string index, IIndexQueryWrapper query, out QueryHeaderInformation queryHeaderInfo);

        IEnumerator<RavenJObjectWrapper> StreamDocs(EtagWrapper fromEtag = null, string startsWith = null, string matches = null, int start = 0, int pageSize = int.MaxValue, string exclude = null);

        void DeleteIndex(string name);

        //TODO: BatchResult[] Batch(IEnumerable<ICommandData> commandDatas);

        void Commit(string txId);

        void Rollback(string txId);

        IDatabaseCommandsWrapper With(ICredentials credentialsForSession);

        IOperationWrapper DeleteByIndex(string indexName, IIndexQueryWrapper queryToDelete);

        IOperationWrapper DeleteByIndex(string indexName, IIndexQueryWrapper queryToDelete, bool allowStale);

        IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IPatchRequestWrapper[] patchRequests);

        IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IScriptedPatchRequestWrapper patch);

        IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IPatchRequestWrapper[] patchRequests, bool allowStale);

        IOperationWrapper UpdateByIndex(string indexName, IIndexQueryWrapper queryToUpdate, IScriptedPatchRequestWrapper patch, bool allowStale);

        IDatabaseCommandsWrapper ForDatabase(string database);

        IDatabaseCommandsWrapper ForSystemDatabase();

        //TODO: SuggestionQueryResult Suggest(string index, SuggestionQuery suggestionQuery);

        //TODO: MultiLoadResult MoreLikeThis(MoreLikeThisQuery query);

        IEnumerable<string> GetTerms(string index, string field, string fromValue, int pageSize);

        //TODO: FacetResults GetFacets(string index, IIndexQueryWrapper query, string facetSetupDoc, int start = 0, int? pageSize = null);

        //TODO: FacetResults GetFacets(string index, IIndexQueryWrapper query, List<Facet> facets, int start = 0, int? pageSize = null);

        RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patches);

        RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patches, bool ignoreMissing);

        RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patch);

        RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patch, bool ignoreMissing);

        RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patches, EtagWrapper etag);

        RavenJObjectWrapper Patch(string key, IPatchRequestWrapper[] patchesToExisting, IPatchRequestWrapper[] patchesToDefault, RavenJObjectWrapper defaultMetadata);

        RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patch, EtagWrapper etag);

        RavenJObjectWrapper Patch(string key, IScriptedPatchRequestWrapper patchExisting, IScriptedPatchRequestWrapper patchDefault, RavenJObjectWrapper defaultMetadata);

        IDisposable DisableAllCaching();

        //TODO: GetResponse[] MultiGet(GetRequest[] requests);

        DatabaseStatisticsWrapper GetStatistics();

        //TODO: JsonDocumentMetadata Head(string key);

        long NextIdentityFor(string name);

        long SeedIdentityFor(string name, long value);

        string UrlFor(string documentKey);

        IDisposable ForceReadFromMaster();

//TODO:        ILowLevelBulkInsertOperation GetBulkInsertOperation(BulkInsertOptions options, IDatabaseChanges changes);

        ITransformerDefinitionWrapper[] GetTransformers(int start, int pageSize);

        ITransformerDefinitionWrapper GetTransformer(string name);

        void DeleteTransformer(string name);

        void PrepareTransaction(string txId);

    }
}