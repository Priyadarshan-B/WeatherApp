using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(AudioFileService))]
public class AudioFileService : IAudioFileService
{
    public Task<List<string>> GetAudioFilesAsync()
    {
        var audioList = new List<string>();
        var musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        if (Directory.Exists(musicFolder))
        {
            var files = Directory.GetFiles(musicFolder, "*.mp3");
            audioList.AddRange(files);
        }
        return Task.FromResult(audioList);
    }
}