using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BlazorMauiApp1.Models
{
    public class Song
    {
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public string AudioUrl { get; set; } = "";
    }
}

