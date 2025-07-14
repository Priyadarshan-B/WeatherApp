namespace BlazorMauiApp1.Services
{
    public static class SupabaseConfig
    {
        // Replace these with your actual Supabase project URL and anon key
        public const string SupabaseUrl = "https://qoyvfgklgvyvoqxrdhrzh.supabase.co";
        public const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFveXZmZ2tsZ3l2b3F4cmRocnpoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDIyNzg4NDMsImV4cCI6MjA1Nzg1NDg0M30.0wIFDdDRE6duzNFbeBpzCwOvvC4JXFwVvB5S5H6UVqo";
        
            // OAuth redirect URI (must match what you configure in Supabase)
    public const string RedirectUri = "com.companyname.blazormauiapp1://auth-callback";
    
    // For Google Cloud Console, use this format:
    public const string GoogleRedirectUri = "https://qoyvfgklgvyvoqxrdhrzh.supabase.co/auth/v1/callback";
    }
} 