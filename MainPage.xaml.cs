using CommunityToolkit.Maui.Views;

namespace BlazorMauiApp1
{
    public partial class MainPage : TabbedPage
    {
        public static MediaElement? MediaPlayerInstance;

        public MainPage()
        {
            InitializeComponent();
            MediaPlayerInstance = mediaPlayer;
        }
    }
}
