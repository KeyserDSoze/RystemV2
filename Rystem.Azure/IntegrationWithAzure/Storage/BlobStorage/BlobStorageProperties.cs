using Azure.Storage.Blobs.Models;
using System.Collections.Generic;

namespace Rystem.Azure.Integration.Storage
{
    public sealed record BlobStorageProperties(BlobHttpHeaders Headers = default, Dictionary<string, string> Metadata = default, Dictionary<string, string> Tags = default, BlobRequestConditions HeadersConditions = default, BlobRequestConditions MetadataConditions = default, BlobRequestConditions TagConditions = default);
}