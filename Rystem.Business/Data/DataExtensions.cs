using Azure.Storage.Blobs.Models;
using Rystem;
using Rystem.Azure.Integration.Storage;
using Rystem.Business;
using Rystem.Business.Data;
using Rystem.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static partial class DataExtensions
    {
        private static IDataManager<IData> Manager(this IData entity)
            => ServiceLocator.GetService<IDataManager<IData>>();
        /// <summary>
        /// Write a stream as Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="stream">Stream that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteAsync(this IData data, Stream stream, BlobUploadOptions blobUploadOptions = default, Installation installation = Installation.Default)
        {
            var manager = data.Manager();
            stream.Position = 0;
            return data.Manager().WriteAsync(data.Name, stream, blobUploadOptions, installation);
        }
        /// <summary>
        /// Write a string as Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="value">string that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteAsync(this IData data, string value, BlobUploadOptions blobUploadOptions = default, Installation installation = Installation.Default)
          => data.Manager().WriteAsync(data.Name, value, blobUploadOptions, installation);
        /// <summary>
        /// Write an array of bytes as Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="value">Array of bytes that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteAsync(this IData data, byte[] value, BlobUploadOptions blobUploadOptions = default, Installation installation = Installation.Default)
          => data.Manager().WriteAsync(data.Name, value, blobUploadOptions, installation);
        /// <summary>
        /// Write an object as Json Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="value">Object that you want to write as json.</param>
        /// <param name="breakLine">True if you want to put a \n at the end of the write (used usually in append configuration).</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteAsync(this IData data, object value, bool breakLine = false, Installation installation = Installation.Default)
          => data.Manager().WriteAsync(data.Name, value, breakLine, installation);
        /// <summary>
        /// Read a json Data object.
        /// </summary>
        /// <typeparam name="T">Type of the object that you wrote as json Data.</typeparam>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Object that you wrote as json Data.</returns>
        public static Task<T> ReadAsync<T>(this IData data, Installation installation = Installation.Default)
          => data.Manager().ReadAsync<T>(data.Name, installation);
        /// <summary>
        /// Read a json Data object.
        /// </summary>
        /// <typeparam name="T">Type of the object that you wrote as json Data.</typeparam>
        /// <param name="data">IData object.</param>
        /// <param name="breakLine">If the content contains a break line pattern (usually used in append configuration).</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Object that you wrote as json Data.</returns>
        public static Task<IEnumerable<T>> ReadAsync<T>(this IData data, bool breakLine = false, Installation installation = Installation.Default)
          => data.Manager().ReadAsync<T>(data.Name, breakLine, installation);
        /// <summary>
        /// Read a Stream from Data.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Stream that you wrote as Data.</returns>
        public static Task<Stream> ReadAsync(this IData data, Installation installation = Installation.Default)
          => data.Manager().ReadAsync(data.Name, installation);
        /// <summary>
        /// Get all items (till the first takeCount parameter) and retrieve the name and the T from json object, use your parameter Name in your IData to create the searchable string.
        /// For instance, if I want to get all files in a blob storage container in a specific folder that starts with H, I set the Name in IData as "folder/H".
        /// </summary>
        /// <typeparam name="T">Type of the object that you wrote as json Data.</typeparam>
        /// <param name="data">IData object.</param>
        /// <param name="takeCount">Number of first "takeCount" elements to retrieve. If null retrieve all elements that matches the searched pattern.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Get "takeCount" items with its Name and T object.</returns>
        public static async IAsyncEnumerable<(string Name, T Content)> ListAsync<T>(this IData data, int? takeCount = null, Installation installation = Installation.Default)
        {
            foreach (var (Name, Value) in await data.Manager().ListStreamAsync(data.Name, takeCount ?? int.MaxValue, installation).NoContext())
                yield return (Name, await Value.FromJsonAsync<T>().NoContext());
        }
        /// <summary>
        /// Get all items (till the first takeCount parameter) and retrieve the name and a list of T from json object, use your parameter Name in your IData to create the searchable string.
        /// For instance, if I want to get all files in a blob storage container in a specific folder that starts with H, I set the Name in IData as "folder/H".
        /// </summary>
        /// <typeparam name="T">Type of the object that you wrote as json Data.</typeparam>
        /// <param name="data">IData object.</param>
        /// <param name="takeCount">Number of first "takeCount" elements to retrieve. If null retrieve all elements that matches the searched pattern.</param>
        /// <param name="breakLine">If the content contains a break line pattern (usually used in append configuration).</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Get "takeCount" items with its Name and a list T objects.</returns>
        public static async IAsyncEnumerable<(string Name, List<T> Content)> ListAsync<T>(this IData data, int? takeCount = null, bool breakLine = false, Installation installation = Installation.Default)
        {
            foreach (var (Name, Value) in await data.Manager().ListStreamAsync(data.Name, takeCount ?? int.MaxValue, installation).NoContext())
                if (!breakLine)
                    yield return (Name, await Value.FromJsonAsync<List<T>>().NoContext());
                else
                    yield return (Name, (await Value.ConvertToStringAsync().NoContext()).Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.FromJson<T>()).ToList());
        }
        /// <summary>
        /// Get all items (till the first takeCount parameter) and retrieve the name and the Stream, use your parameter Name in your IData to create the searchable string.
        /// For instance, if I want to get all files in a blob storage container in a specific folder that starts with H, I set the Name in IData as "folder/H".
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="takeCount">Number of first "takeCount" elements to retrieve. If null retrieve all elements that matches the searched pattern.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Get "takeCount" items with its Name and Stream.</returns>
        public static Task<IEnumerable<(string Name, Stream Content)>> ListAsync(this IData data, int? takeCount = null, Installation installation = Installation.Default)
            => data.Manager().ListStreamAsync(data.Name, takeCount ?? int.MaxValue, installation);
        /// <summary>
        /// Delete the Data object.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> DeleteAsync(this IData data, Installation installation = Installation.Default)
          => data.Manager().DeleteAsync(data.Name, installation);
        /// <summary>
        /// Check the existence of the Data object.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> ExistsAsync(this IData data, Installation installation = Installation.Default)
          => data.Manager().ExistsAsync(data.Name, installation);
        /// <summary>
        /// Set properties for Azure blob storage Data.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="properties">Properties for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> SetPropertiesAsync(this IData data, BlobStorageProperties properties, Installation installation = Installation.Default)
          => data.Manager().SetPropertiesAsync(data.Name, properties, installation);
    }
}