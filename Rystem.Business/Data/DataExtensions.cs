using Azure.Storage.Blobs.Models;
using Rystem;
using Rystem.Azure.Integration.Storage;
using Rystem.Business;
using Rystem.Business.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace System
{
    public static partial class DataExtensions
    {
        private static IDataManager<T> Manager<T>(this T entity)
            where T : IData
            => ServiceLocator.GetService<IDataManager<T>>();
        /// <summary>
        /// Write a stream as Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="stream">Stream that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>-
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteBlobAsync<T>(this T data, Stream stream, BlobUploadOptions blobUploadOptions = default, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            stream.Position = 0;
            return manager.WriteAsync(manager.GetName(data, installation), stream, blobUploadOptions, installation);
        }
        /// <summary>
        /// Write a string as Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="value">string that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteBlobAsync<T>(this T data, string value, BlobUploadOptions blobUploadOptions = default, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.WriteAsync(manager.GetName(data, installation), value, blobUploadOptions, installation);
        }

        /// <summary>
        /// Write an array of bytes as Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="value">Array of bytes that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteBlobAsync<T>(this T data, byte[] value, BlobUploadOptions blobUploadOptions = default, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.WriteAsync(manager.GetName(data, installation), value, blobUploadOptions, installation);
        }

        /// <summary>
        /// Write an object as Json Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="value">Object that you want to write as json.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteAsync<T>(this T data, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.WriteAsync(manager.GetName(data, installation), data, installation);
        }
        /// <summary>
        /// Write an object as Json Data on the right installation of IData.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="data">IData object.</param>
        /// <param name="value">Object that you want to write as json.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> WriteAsync<T, TEntity>(this T data, TEntity value, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.WriteAsync(manager.GetName(data, installation), value, installation);
        }

        /// <summary>
        /// Read a json Data object.
        /// </summary>
        /// <typeparam name="T">Type of the object that you wrote as json Data.</typeparam>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Object that you wrote as json Data.</returns>
        public static Task<T> ReadAsync<T>(this T data, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.ReadAsync(manager.GetName(data, installation), installation);
        }
        /// <summary>
        /// Read a json Data object.
        /// </summary>
        /// <typeparam name="T">Type of the object that you wrote as json Data.</typeparam>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Object that you wrote as json Data.</returns>
        public static Task<TEntity> ReadAsync<T, TEntity>(this T data, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.ReadAsync<TEntity>(manager.GetName(data, installation), installation);
        }

        /// <summary>
        /// Read a Stream from Data.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Stream that you wrote as Data.</returns>
        public static Task<Stream> ReadStreamAsync<T>(this T data, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.ReadStreamAsync(manager.GetName(data, installation), installation);
        }

        /// <summary>
        /// Get all items (till the first takeCount parameter) and retrieve the name and a list of T from json object, use your parameter Name in your IData to create the searchable string.
        /// For instance, if I want to get all files in a blob storage container in a specific folder that starts with H, I set the Name in IData as "folder/H".
        /// </summary>
        /// <typeparam name="T">Type of the object that you wrote as json Data.</typeparam>
        /// <param name="data">IData object.</param>
        /// <param name="takeCount">Number of first "takeCount" elements to retrieve. If null retrieve all elements that matches the searched pattern.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Get "takeCount" items with its Name and a list T objects.</returns>
        public static IAsyncEnumerable<(string Name, T Content)> ListAsync<T>(this T data, int? takeCount = null, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.ListAsync(manager.GetName(data), takeCount, installation);
        }
        /// <summary>
        /// Get all items (till the first takeCount parameter) and retrieve the name and the Stream, use your parameter Name in your IData to create the searchable string.
        /// For instance, if I want to get all files in a blob storage container in a specific folder that starts with H, I set the Name in IData as "folder/H".
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="takeCount">Number of first "takeCount" elements to retrieve. If null retrieve all elements that matches the searched pattern.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Get "takeCount" items with its Name and Stream.</returns>
        public static Task<IEnumerable<(string Name, Stream Content)>> ListStreamAsync<T>(this T data, int? takeCount = null, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.ListStreamAsync(manager.GetName(data, installation), takeCount ?? int.MaxValue, installation);
        }

        /// <summary>
        /// Delete the Data object.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> DeleteAsync<T>(this T data, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.DeleteAsync(manager.GetName(data, installation), installation);
        }

        /// <summary>
        /// Check the existence of the Data object.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> ExistsAsync<T>(this T data, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.ExistsAsync(manager.GetName(data, installation), installation);
        }

        /// <summary>
        /// Set properties for Azure blob storage Data.
        /// </summary>
        /// <param name="data">IData object.</param>
        /// <param name="properties">Properties for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        public static Task<bool> SetPropertiesAsync<T>(this T data, BlobStorageProperties properties, Installation installation = Installation.Default)
            where T : IData
        {
            var manager = data.Manager();
            return manager.SetPropertiesAsync(manager.GetName(data, installation), properties, installation);
        }
    }
}