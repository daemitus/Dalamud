using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Game;
using Serilog;

namespace Dalamud {
    internal static class Util {
        public static void DumpMemory(IntPtr offset, int len = 512) {
            var data = new byte[len];
            Marshal.Copy(offset, data, 0, len);
            Log.Information(ByteArrayToHex(data));
        }

        public static string ByteArrayToHex(byte[] bytes, int offset = 0, int bytesPerLine = 16) {
            if (bytes == null) return string.Empty;

            var hexChars = "0123456789ABCDEF".ToCharArray();

            var offsetBlock = 8 + 3;
            var byteBlock = offsetBlock + bytesPerLine * 3 + (bytesPerLine - 1) / 8 + 2;
            var lineLength = byteBlock + bytesPerLine + Environment.NewLine.Length;

            var line = (new string(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            var numLines = (bytes.Length + bytesPerLine - 1) / bytesPerLine;

            var sb = new StringBuilder(numLines * lineLength);

            for (var i = 0; i < bytes.Length; i += bytesPerLine) {
                var h = i + offset;

                line[0] = hexChars[(h >> 28) & 0xF];
                line[1] = hexChars[(h >> 24) & 0xF];
                line[2] = hexChars[(h >> 20) & 0xF];
                line[3] = hexChars[(h >> 16) & 0xF];
                line[4] = hexChars[(h >> 12) & 0xF];
                line[5] = hexChars[(h >> 8) & 0xF];
                line[6] = hexChars[(h >> 4) & 0xF];
                line[7] = hexChars[(h >> 0) & 0xF];

                var hexColumn = offsetBlock;
                var charColumn = byteBlock;

                for (var j = 0; j < bytesPerLine; j++) {
                    if (j > 0 && (j & 7) == 0) hexColumn++;

                    if (i + j >= bytes.Length) {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    } else {
                        var by = bytes[i + j];
                        line[hexColumn] = hexChars[(by >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[by & 0xF];
                        line[charColumn] = by < 32 ? '.' : (char) by;
                    }

                    hexColumn += 3;
                    charColumn++;
                }

                sb.Append(line);
            }

            return sb.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static string AssemblyVersion { get; } = Assembly.GetAssembly(typeof(ChatHandlers)).GetName().Version.ToString();

        /// <summary>
        ///     Convert a struct to a List of roughly ToString()'d components. It is purposely not joined together so if
        ///     you're dumping several structs, they can be output in an aligned table format.
        /// </summary>
        /// <typeparam name="T">Struct Type</typeparam>
        /// <param name="aStruct">The struct being stringified</param>
        public static List<string> StructToComponents<T>(T aStruct) where T : struct
        {
            var components = new List<string>();
            foreach (FieldInfo fieldInfo in aStruct.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object fieldData = fieldInfo.GetValue(aStruct);
                if (fieldInfo.FieldType.IsArray)
                {
                    StringBuilder sb = new StringBuilder();
                    var aFieldData = (Array)fieldData;
                    for (int i = 0; i < aFieldData.Length; i++)
                    {
                        sb.Append(aFieldData.GetValue(i).ToString());
                        if (i < aFieldData.Length - 1)
                            sb.Append(", ");
                    }
                    components.Add($"{fieldInfo.Name}=new {fieldInfo.FieldType} {{ {sb} }}");
                }
                else
                {
                    components.Add($"{fieldInfo.Name}={fieldData}");
                }
            }
            return components;
        }

        /// <summary>
        ///     Convert a struct to a string of roughly ToString()'d components joined by two spaces.
        /// </summary>
        /// <typeparam name="T">Struct Type</typeparam>
        /// <param name="aStruct">The struct being stringified</param>
        public static string StructToString<T>(T aStruct) where T : struct
        {
            return string.Join("  ", StructToComponents(aStruct));
        }
    }

    /// <summary>
    ///     Linq Extensions
    /// </summary>
    internal static class LinqExtensions
    {
        /// <summary>
        ///     Rotate a 2D nested array 90 degrees.
        /// </summary>
        /// <typeparam name="T">Type of the underlying 2D array</typeparam>
        /// <param name="source">Array to be transposed</param>
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
        {
            return from row in source
                   from col in row.Select(
                       (x, i) => new KeyValuePair<int, T>(i, x))
                   group col.Value by col.Key into c
                   select c as IEnumerable<T>;
        }
    }
}
