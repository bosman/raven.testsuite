using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface ISmugglerArgumentsBuilder
    {
        ISmugglerArgumentsBuilder ExportFrom(string dbAddress);

        ISmugglerArgumentsBuilder ImportTo(string dbAddress);

        ISmugglerArgumentsBuilder UsingFile(string filePath);

        ISmugglerArgumentsBuilder WithOperateOnTypes(string value);

        ISmugglerArgumentsBuilder WithMetadataFilter(string metadataPropertyName, string value);

        ISmugglerArgumentsBuilder WithNegativeMetadataFilter(string metadataPropertyName, string value);

        ISmugglerArgumentsBuilder WithFilter(string propertyName, string value);

        ISmugglerArgumentsBuilder WithNegativeFilter(string propertyName, string value);

        ISmugglerArgumentsBuilder WithTransform(string script);

        ISmugglerArgumentsBuilder WithTransformFile(string scriptFile);

        ISmugglerArgumentsBuilder WithTimeout(int timeout);

        ISmugglerArgumentsBuilder WithBatchSize(int batchSize);

        ISmugglerArgumentsBuilder UsingDatabase(string databaseName);

        ISmugglerArgumentsBuilder AsUser(string username);

        ISmugglerArgumentsBuilder WithPassword(string password);

        ISmugglerArgumentsBuilder WithDomain(string domain);

        ISmugglerArgumentsBuilder WithApiKey(string apiKey);

        ISmugglerArgumentsBuilder WithIncremental();

        ISmugglerArgumentsBuilder WithWaitForIndexing();

        ISmugglerArgumentsBuilder WithExcludeExpired();

        string Build();
    }
}
