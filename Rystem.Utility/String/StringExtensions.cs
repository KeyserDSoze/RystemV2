using Rystem.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem
{
    public static class StringExtensions
    {
        public static string ToUpperCaseFirst(this string value)
            => string.IsNullOrWhiteSpace(value) ? value : $"{value.FirstOrDefault().ToString().ToUpper()}{value[1..].ToLower()}";
        public static Stream ToStream(this byte[] bytes)
            => new MemoryStream(bytes)
            {
                Position = 0
            };
        public static Task<string> ConvertToStringAsync(this Stream entity)
        {
            if (entity.CanSeek)
                entity.Position = 0;
            using StreamReader streamReader = new(entity);
            return streamReader.ReadToEndAsync();
        }
        public static string ConvertToString(this Memory<byte> buffer, EncodingType type = EncodingType.UTF8)
        {
            switch (type)
            {
                default:
                case EncodingType.Default:
                    return Encoding.Default.GetString(buffer.Span);
                case EncodingType.UTF8:
                    return Encoding.UTF8.GetString(buffer.Span);
                case EncodingType.UTF7:
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                    return Encoding.UTF7.GetString(buffer.Span);
#pragma warning restore SYSLIB0001 // Type or member is obsolete
                case EncodingType.UTF32:
                    return Encoding.UTF32.GetString(buffer.Span);
                case EncodingType.Latin1:
                    return Encoding.Latin1.GetString(buffer.Span);
                case EncodingType.ASCII:
                    return Encoding.ASCII.GetString(buffer.Span);
                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode.GetString(buffer.Span);
            }
        }
        public static async IAsyncEnumerable<string> ReadLinesAsync(this Stream entity)
        {
            if (entity.CanSeek)
                entity.Position = 0;
            using StreamReader streamReader = new(entity);
            while (!streamReader.EndOfStream)
                yield return await streamReader.ReadLineAsync().NoContext();
        }
        public static string ConvertToString(this Stream entity)
            => ConvertToStringAsync(entity).ToResult();
        public static async Task<Stream> ToStreamAsync(this string entity)
        {
            NotClosableStream memoryStream = new();
            using StreamWriter writer = new(memoryStream);
            await writer.WriteAsync(entity).NoContext();
            await writer.FlushAsync().NoContext();
            memoryStream.Position = 0;
            return memoryStream;
        }

        public static string ConvertToString(this byte[] entity, EncodingType type = EncodingType.UTF8)
        {
            switch (type)
            {
                default:
                case EncodingType.Default:
                    return Encoding.Default.GetString(entity);
                case EncodingType.UTF8:
                    return Encoding.UTF8.GetString(entity);
                case EncodingType.UTF7:
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                    return Encoding.UTF7.GetString(entity);
#pragma warning restore SYSLIB0001 // Type or member is obsolete
                case EncodingType.UTF32:
                    return Encoding.UTF32.GetString(entity);
                case EncodingType.Latin1:
                    return Encoding.Latin1.GetString(entity);
                case EncodingType.ASCII:
                    return Encoding.ASCII.GetString(entity);
                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode.GetString(entity);
            }
        }
        public static byte[] ToByteArray(this string entity, EncodingType type = EncodingType.UTF8)
        {
            switch (type)
            {
                default:
                case EncodingType.Default:
                    return Encoding.Default.GetBytes(entity);
                case EncodingType.UTF8:
                    return Encoding.UTF8.GetBytes(entity);
                case EncodingType.UTF7:
                    return Encoding.UTF7.GetBytes(entity);
                case EncodingType.UTF32:
                    return Encoding.UTF32.GetBytes(entity);
                case EncodingType.Latin1:
                    return Encoding.Latin1.GetBytes(entity);
                case EncodingType.ASCII:
                    return Encoding.ASCII.GetBytes(entity);
                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode.GetBytes(entity);
            }
        }
    }
}