using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Validation;

namespace ImageSearch.Helpers
{
    public static class FileHelper
    {
        private static readonly IReadOnlyDictionary<FileType, byte[]> _typeHeaders = new Dictionary<FileType, byte[]>
        {
            [FileType.Png]  = new byte[] { 0x89, 0x50, 0x4e, 0x47 },
            [FileType.Jpeg] = new byte[] { 0xff, 0xd8, 0xff, 0xe0 },
            [FileType.Gif]  = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39 },
            [FileType.Bmp]  = new byte[] { 0x42, 0x4d },
        };

        public static bool IsFileType(string filePath, FileType type)
        {
            Requires.Argument(File.Exists(filePath), nameof(filePath), $"File '{filePath}' doesn't exist.");
            Requires.Defined(type, nameof(type));

            byte[] typeHeader = _typeHeaders[type];
            byte[] fileHeader = ReadFileHeader(filePath, typeHeader.Length);

            return ArraysEqual(typeHeader, fileHeader, typeHeader.Length);
        }

        public static bool IsAnyFileType(string filePath, params FileType[] types)
        {
            Requires.Argument(File.Exists(filePath), nameof(filePath), $"File '{filePath}' doesn't exist.");
            Requires.NotNullOrEmpty(types, nameof(types));

            byte[][] typeHeaders = Array.ConvertAll(types, type => _typeHeaders[type]);
            byte[] fileHeader = ReadFileHeader(filePath, typeHeaders.Max(header => header.Length));

            foreach (byte[] typeHeader in typeHeaders)
            {
                if (ArraysEqual(typeHeader, fileHeader, typeHeader.Length))
                {
                    return true;
                }
            }

            return false;
        }

        private static byte[] ReadFileHeader(string filePath, int length)
        {
            Debug.Assert(File.Exists(filePath));
            Debug.Assert(length > 0);

            using Stream stream = File.OpenRead(filePath);
            using var reader = new BinaryReader(stream);

            return reader.ReadBytes(length);
        }

        private static bool ArraysEqual<T>(T[] a, T[] b, int length)
        {
            Debug.Assert(a is object);
            Debug.Assert(b is object);
            Debug.Assert(length > 0);

            if (a.Length < length)
            {
                return false;
            }

            if (b.Length < length)
            {
                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < length; i++)
            {
                if (comparer.Equals(a[i], b[i]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        public enum FileType
        {
            Bmp,
            Gif,
            Jpeg,
            Png,
        }
    }
}
