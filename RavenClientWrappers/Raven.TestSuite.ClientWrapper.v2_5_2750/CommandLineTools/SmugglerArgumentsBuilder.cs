using Raven.TestSuite.Common.WrapperInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750.CommandLineTools
{
    public class SmugglerArgumentsBuilder : ISmugglerArgumentsBuilder
    {
        private string transferDirection = string.Empty;
        private string dumpFilePath = string.Empty;
        private StringBuilder sb = new StringBuilder();

        public ISmugglerArgumentsBuilder ExportFrom(string dbAddress)
        {
            this.transferDirection = "out " + dbAddress;
            return this;
        }

        public ISmugglerArgumentsBuilder ImportTo(string dbAddress)
        {
            this.transferDirection = "in" + dbAddress;
            return this;
        }

        public ISmugglerArgumentsBuilder UsingFile(string filePath)
        {
            this.dumpFilePath = filePath;
            return this;
        }

        public ISmugglerArgumentsBuilder WithOperateOnTypes(string value)
        {
            sb.AppendFormat(" --operate-on-types={0}", value);
            return this;
        }

        public ISmugglerArgumentsBuilder WithMetadataFilter(string metadataPropertyName, string value)
        {
            sb.AppendFormat(" --metadata-filter={0}={1}", metadataPropertyName, value);
            return this;
        }

        public ISmugglerArgumentsBuilder WithNegativeMetadataFilter(string metadataPropertyName, string value)
        {
            sb.AppendFormat(" --negative-metadata-filter={0}={1}", metadataPropertyName, value);
            return this;
        }

        public ISmugglerArgumentsBuilder WithFilter(string propertyName, string value)
        {
            sb.AppendFormat(" --filter={0}={1}", propertyName, value);
            return this;
        }

        public ISmugglerArgumentsBuilder WithNegativeFilter(string propertyName, string value)
        {
            sb.AppendFormat(" --negative-filter={0}={1}", propertyName, value);
            return this;
        }

        public ISmugglerArgumentsBuilder WithTransform(string script)
        {
            sb.AppendFormat(" --transform={0}", script);
            return this;
        }

        public ISmugglerArgumentsBuilder WithTransformFile(string scriptFile)
        {
            sb.AppendFormat(" --transform-file={0}", scriptFile);
            return this;
        }

        public ISmugglerArgumentsBuilder WithTimeout(int timeout)
        {
            sb.AppendFormat(" --timeout={0}", timeout);
            return this;
        }

        public ISmugglerArgumentsBuilder WithBatchSize(int batchSize)
        {
            sb.AppendFormat(" --batch-size={0}", batchSize);
            return this;
        }

        public ISmugglerArgumentsBuilder UsingDatabase(string databaseName)
        {
            sb.AppendFormat(" --database={0}", databaseName);
            return this;
        }

        public ISmugglerArgumentsBuilder AsUser(string username)
        {
            sb.AppendFormat(" --username={0}", username);
            return this;
        }

        public ISmugglerArgumentsBuilder WithPassword(string password)
        {
            sb.AppendFormat(" --password={0}", password);
            return this;
        }

        public ISmugglerArgumentsBuilder WithDomain(string domain)
        {
            sb.AppendFormat(" --domain={0}", domain);
            return this;
        }

        public ISmugglerArgumentsBuilder WithApiKey(string apiKey)
        {
            sb.AppendFormat(" --api-key={0}", apiKey);
            return this;
        }

        public ISmugglerArgumentsBuilder WithIncremental()
        {
            sb.Append(" --incremental");
            return this;
        }

        public ISmugglerArgumentsBuilder WithWaitForIndexing()
        {
            sb.Append(" --wait-for-indexing");
            return this;
        }

        public ISmugglerArgumentsBuilder WithExcludeExpired()
        {
            sb.Append(" --excludeexpired");
            return this;
        }

        public string Build()
        {
            return string.Format("{0} {1}{2}", transferDirection, dumpFilePath, sb.ToString());
        }
    }
}
