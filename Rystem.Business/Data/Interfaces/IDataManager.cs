using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rystem.Business.Data
{
    public interface IDataManager<TEntity>
    {
        /// <summary>
        /// Delete the Data object.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        Task<bool> DeleteAsync(string name, Installation installation = Installation.Default);
        /// <summary>
        /// Check the existence of the Data object.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        Task<bool> ExistsAsync(string name, Installation installation = Installation.Default);
        /// <summary>
        /// Read a json Data object.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Object that you wrote as json Data.</returns>
        Task<TEntity> ReadAsync(string name, Installation installation = Installation.Default);
        /// <summary>
        /// Read a Stream from Data.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Stream that you wrote as Data.</returns>
        Task<Stream> ReadStreamAsync(string name, Installation installation = Installation.Default);
        /// <summary>
        /// Read a json Data object.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Object that you wrote as json Data on multiple lines.</returns>
        Task<List<TEntity>> ReadMultipleAsync(string name, Installation installation = Installation.Default);
        /// <summary>
        /// Get all items (till the first takeCount parameter) and retrieve the name and the Stream, use your parameter Name in your IData to create the searchable string.
        /// For instance, if I want to get all files in a blob storage container in a specific folder that starts with H, I set the Name in IData as "folder/H".
        /// </summary>
        /// <param name="startsWith">name which starts with. Use "/" to search in virtual folder.</param>
        /// <param name="takeCount">Number of first "takeCount" elements to retrieve. If null retrieve all elements that matches the searched pattern.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Get "takeCount" items with its Name and Stream.</returns>
        Task<IEnumerable<(string Name, Stream Value)>> ListStreamAsync(string startsWith, int? takeCount = null, Installation installation = Installation.Default);
        /// <summary>
        /// Get all items (till the first takeCount parameter) and retrieve the name and a list of T from json object, use your parameter Name in your IData to create the searchable string.
        /// For instance, if I want to get all files in a blob storage container in a specific folder that starts with H, I set the Name in IData as "folder/H".
        /// </summary>
        /// <param name="startsWith">name which starts with. Use "/" to search in virtual folder.</param>
        /// <param name="takeCount">Number of first "takeCount" elements to retrieve. If null retrieve all elements that matches the searched pattern.</param>
        /// <param name="breakLine">If the content contains a break line pattern (usually used in append configuration).</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>Get "takeCount" items with its Name and a list T objects.</returns>
        IAsyncEnumerable<(string Name, TEntity Content)> ListAsync(string startsWith, int? takeCount = null, Installation installation = Installation.Default);
        /// <summary>
        /// Write a stream as Data on the right installation of IData.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="stream">Stream that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        Task<bool> WriteAsync(string name, Stream stream, dynamic options, Installation installation = Installation.Default);
        /// <summary>
        /// Write a string as Data on the right installation of IData.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="value">string that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        Task<bool> WriteAsync(string name, string value, dynamic options, Installation installation = Installation.Default);
        /// <summary>
        /// Write an array of bytes as Data on the right installation of IData.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="value">Array of bytes that you want to write.</param>
        /// <param name="blobUploadOptions">Options for Azure blob storage.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        Task<bool> WriteAsync(string name, byte[] value, dynamic options, Installation installation = Installation.Default);
        /// <summary>
        /// Write an object as Json Data on the right installation of IData.
        /// </summary>
        /// <param name="name">name of the object. Use "/" in name to create virtual folder.</param>
        /// <param name="value">Object that you want to write as json.</param>
        /// <param name="installation">Rystem installation value.</param>
        /// <returns>True if all goes ok.</returns>
        Task<bool> WriteAsync(string name, TEntity value, Installation installation = Installation.Default);
        Task<bool> SetPropertiesAsync(string name, dynamic properties, Installation installation = Installation.Default);
        string GetName(TEntity entity, Installation installation = Installation.Default);
    }
}