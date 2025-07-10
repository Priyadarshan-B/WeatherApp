using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(AudioFileService))]
public class AudioFileService : IAudioFileService
{
    public Task<List<string>> GetAudioFilesAsync()
    {
        var audioList = new List<string>();
        var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        if (Directory.Exists(docs))
        {
            var files = Directory.GetFiles(docs, "*.mp3");
            audioList.AddRange(files);
        }
        return Task.FromResult(audioList);
    }
}