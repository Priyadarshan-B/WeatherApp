using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAudioFileService
{
    Task<List<string>> GetAudioFilesAsync();
}