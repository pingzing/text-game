using System.IO;
using System.Threading.Tasks;
using Optional;

namespace TextGameExperiment.Core.Services
{
    public interface IFileService
    {
        Option<StreamReader> LoadFromResources(string assemblyRelativePath);
        Task<Option<string>> ReadFileAsync(StreamReader fileStream);
        Task<Option<string>> ReadFileFromResourcesAsync(string assemblyRelativePath);
        Task<Option<string>> ReadFirstLineAsync(StreamReader fileStream);
        Task<Option<string>> ReadFirstLineFromResourcesAsync(string assemblyRelativePath);
    }
}