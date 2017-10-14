using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TextGameExperiment.Core.Services
{
    public static class FileService
    {
        /// <summary>
        /// Attempts to open a file embedded as a resource in the current assembly, and return a Stream pointing to it.
        /// </summary>
        /// <param name="assemblyRelativePath">The path to the file, relative to the assembly root. Directories should be separated by dots.</param>
        /// <returns>An Optional wrapper around a Stream if the file can be opened. Option.None() otherwise.</returns>
        public static Option<StreamReader> LoadFromResources(string assemblyRelativePath)
        {
            Assembly assembly = typeof(FileService).GetTypeInfo().Assembly;
            string resourceName = $"{assembly.GetName().Name}.{assemblyRelativePath}";

            try
            {
                return Option.Some(new StreamReader(assembly.GetManifestResourceStream(resourceName)));
            }
            catch (Exception ex) when (ex is ArgumentNullException
                                       || ex is ArgumentException
                                       || ex is FileLoadException
                                       || ex is FileNotFoundException
                                       || ex is BadImageFormatException)

            {
                Debug.WriteLine($"Unable to open file at {resourceName}. Exception message: {ex.Message}. Stack trace: {ex.StackTrace}");
                return Option.None<StreamReader>();
            }
        }

        public static async Task<Option<string>> ReadFirstLineFromResourcesAsync(string assemblyRelativePath)
        {
            Assembly assembly = typeof(FileService).GetTypeInfo().Assembly;
            string resourceName = $"{assembly.GetName().Name}.{assemblyRelativePath}";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return await ReadFirstLineAsync(reader);
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException
                                       || ex is ArgumentException
                                       || ex is FileLoadException
                                       || ex is FileNotFoundException
                                       || ex is BadImageFormatException)

            {
                Debug.WriteLine($"Unable to read file at {resourceName}. Exception message: {ex.Message}. Stack trace: {ex.StackTrace}");
                return Option.None<string>();
            }
        }

        public static async Task<Option<string>> ReadFirstLineAsync(StreamReader fileStream)
        {
            if (fileStream.Peek() != -1)
            {
                return Option.Some(await fileStream.ReadLineAsync());
            }
            else
            {
                return Option.None<string>();
            }
        }

        public static async Task<Option<string>> ReadFileFromResourcesAsync(string assemblyRelativePath)
        {
            Assembly assembly = typeof(FileService).GetTypeInfo().Assembly;
            string resourceName = $"{assembly.GetName().Name}.{assemblyRelativePath}";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return await ReadFileAsync(reader);
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException
                                       || ex is ArgumentException
                                       || ex is FileLoadException
                                       || ex is FileNotFoundException
                                       || ex is BadImageFormatException)

            {
                Debug.WriteLine($"Unable to read file at {resourceName}. Exception message: {ex.Message}. Stack trace: {ex.StackTrace}");
                return Option.None<string>();
            }
        }

        public static async Task<Option<string>> ReadFileAsync(StreamReader fileStream)
        {
            if (fileStream.Peek() != -1)
            {
                return Option.Some(await fileStream.ReadToEndAsync());
            }
            else
            {
                return Option.None<string>();
            }
        }
    }
}
