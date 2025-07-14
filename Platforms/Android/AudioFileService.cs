using Android.Content;
using Android.Database;
using Android.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Android;
using AndroidX.Core.Content;
using Android.Content.PM;

[assembly: Dependency(typeof(AudioFileService))]
public class AudioFileService : IAudioFileService
{
    public Task<List<string>> GetAudioFilesAsync()
    {
        System.Diagnostics.Debug.WriteLine("🎵 GetAudioFilesAsync called");
        var context = Android.App.Application.Context;
        var audioList = new List<string>();

        // Check permissions first
        if (!HasAudioPermission())
        {
            System.Diagnostics.Debug.WriteLine("❌ No audio permission granted");
            return Task.FromResult(audioList);
        }

        System.Diagnostics.Debug.WriteLine("✅ Permission granted, querying media store...");

        try
        {
            var audioUri = MediaStore.Audio.Media.ExternalContentUri;
            System.Diagnostics.Debug.WriteLine($"🔗 URI: {audioUri}");

            string[] projection = {
                MediaStore.Audio.Media.InterfaceConsts.Id,
                MediaStore.Audio.Media.InterfaceConsts.Data,
                MediaStore.Audio.Media.InterfaceConsts.Title,
                MediaStore.Audio.Media.InterfaceConsts.Artist,
                MediaStore.Audio.Media.InterfaceConsts.Album
            };
            string selection = MediaStore.Audio.Media.InterfaceConsts.IsMusic + " != 0";
            string sortOrder = MediaStore.Audio.Media.InterfaceConsts.Title + " ASC";

            System.Diagnostics.Debug.WriteLine($"🔍 Querying with selection: {selection}");

            using (var cursor = context.ContentResolver != null ? context.ContentResolver.Query(audioUri, projection, selection, null, sortOrder) : null)
            {
                System.Diagnostics.Debug.WriteLine($"📊 Cursor is null: {cursor == null}");

                if (cursor != null)
                {
                    System.Diagnostics.Debug.WriteLine($"📊 Cursor count: {cursor.Count}");

                    if (cursor.MoveToFirst())
                    {
                        int idIndex = cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Id);
                        int dataIndex = cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Data);
                        int titleIndex = cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Title);
                        int artistIndex = cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Artist);
                        int albumIndex = cursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Album);
                        
                        System.Diagnostics.Debug.WriteLine($"📊 ID column index: {idIndex}");
                        System.Diagnostics.Debug.WriteLine($"📊 Data column index: {dataIndex}");
                        System.Diagnostics.Debug.WriteLine($"📊 Title column index: {titleIndex}");
                        System.Diagnostics.Debug.WriteLine($"📊 Artist column index: {artistIndex}");
                        System.Diagnostics.Debug.WriteLine($"📊 Album column index: {albumIndex}");

                        do
                        {
                            long id = cursor.GetLong(idIndex);
                            string filePath = cursor.GetString(dataIndex);
                            string title = cursor.GetString(titleIndex) ?? "Unknown Title";
                            string artist = cursor.GetString(artistIndex) ?? "Unknown Artist";
                            string album = cursor.GetString(albumIndex) ?? "Unknown Album";
                            
                            System.Diagnostics.Debug.WriteLine($"📁 ID: {id}");
                            System.Diagnostics.Debug.WriteLine($"📁 File path: {filePath}");
                            System.Diagnostics.Debug.WriteLine($"📁 Title: {title}");
                            System.Diagnostics.Debug.WriteLine($"📁 Artist: {artist}");
                            System.Diagnostics.Debug.WriteLine($"📁 Album: {album}");

                            if (!string.IsNullOrEmpty(filePath))
                            {
                                bool fileExists = System.IO.File.Exists(filePath);
                                System.Diagnostics.Debug.WriteLine($"📁 File exists: {fileExists}");

                                if (fileExists)
                                {
                                    // Get album artwork URI
                                    string albumArtUri = GetAlbumArtUri(id);
                                    
                                    // Create a JSON-like string with metadata
                                    var songInfo = new
                                    {
                                        Id = id,
                                        FilePath = filePath,
                                        Title = title,
                                        Artist = artist,
                                        Album = album,
                                        AlbumArtUri = albumArtUri
                                    };
                                    
                                    string songData = System.Text.Json.JsonSerializer.Serialize(songInfo);
                                    audioList.Add(songData);
                                    System.Diagnostics.Debug.WriteLine($"✅ Added audio file: {filePath} - {title} by {artist} from {album}");
                                }
                            }
                        } while (cursor.MoveToNext());
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ Cursor.MoveToFirst() returned false");
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"📱 Found {audioList.Count} audio files");
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error accessing audio files: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
        }

        return Task.FromResult(audioList);
    }

    private string GetAlbumArtUri(long songId)
    {
        try
        {
            var context = Android.App.Application.Context;
            
            // Use the correct album art URI format
            var albumArtUri = Android.Net.Uri.Parse($"content://media/external/audio/albumart/{songId}");
            
            // Test if the URI is accessible
            try
            {
                if (context.ContentResolver != null)
                {
                    using var inputStream = context.ContentResolver.OpenInputStream(albumArtUri);
                    if (inputStream != null)
                    {
                        inputStream.Close();
                        System.Diagnostics.Debug.WriteLine($"\u2705 Album art found for song ID: {songId}");
                        return albumArtUri.ToString();
                    }
                }
            }
            catch
            {
                // Album art doesn't exist, return empty
                System.Diagnostics.Debug.WriteLine($"\u274c No album art for song ID: {songId}");
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error getting album art: {ex.Message}");
        }
        
        return string.Empty;
    }

    private bool HasAudioPermission()
    {
        var context = Android.App.Application.Context;

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
        {
            // Android 13+ uses READ_MEDIA_AUDIO
            return ContextCompat.CheckSelfPermission(context, Android.Manifest.Permission.ReadMediaAudio) == Permission.Granted;
        }
        else
        {
            // Android 12 and below uses READ_EXTERNAL_STORAGE
            return ContextCompat.CheckSelfPermission(context, Android.Manifest.Permission.ReadExternalStorage) == Permission.Granted;
        }
    }
}