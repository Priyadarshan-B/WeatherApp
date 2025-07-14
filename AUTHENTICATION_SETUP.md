# Google Authentication Setup with Supabase

This guide will help you set up Google authentication for your Blazor MAUI app using Supabase.

## Prerequisites

1. A Supabase account and project
2. A Google Cloud Console project with OAuth 2.0 credentials
3. .NET 8.0 SDK

## Step 1: Supabase Setup

### 1.1 Create a Supabase Project

1. Go to [supabase.com](https://supabase.com) and create a new project
2. Note down your project URL and anon key from the API settings

### 1.2 Configure Google OAuth in Supabase

1. In your Supabase dashboard, go to **Authentication** > **Providers**
2. Enable **Google** provider
3. Add your Google OAuth credentials (Client ID and Client Secret)
4. Set the redirect URL to: `com.companyname.blazormauiapp1://auth-callback`

## Step 2: Google Cloud Console Setup

### 2.1 Create OAuth 2.0 Credentials

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the Google+ API
4. Go to **APIs & Services** > **Credentials**
5. Click **Create Credentials** > **OAuth 2.0 Client IDs**
6. Choose **Web application** as the application type
7. Add authorized redirect URIs:
   - `https://your-project-ref.supabase.co/auth/v1/callback`
   - `com.companyname.blazormauiapp1://auth-callback`

### 2.2 Configure OAuth Consent Screen

1. Go to **OAuth consent screen**
2. Add your app name and description
3. Add your email as a test user
4. Save the configuration

## Step 3: Update App Configuration

### 3.1 Update SupabaseConfig.cs

Open `Services/SupabaseConfig.cs` and replace the placeholder values:

```csharp
public static class SupabaseConfig
{
    // Replace with your actual Supabase project URL
    public const string SupabaseUrl = "https://your-project-ref.supabase.co";

    // Replace with your actual Supabase anon key
    public const string SupabaseAnonKey = "your-anon-key-here";

    // OAuth redirect URI (must match what you configure in Supabase)
    public const string RedirectUri = "com.companyname.blazormauiapp1://auth-callback";
}
```

### 3.2 Update Application ID (Optional)

If you want to use a different application ID, update it in:

- `BlazorMauiApp1.csproj` - `ApplicationId` property
- `Platforms/Android/AndroidManifest.xml` - `android:scheme` value
- `Platforms/iOS/Info.plist` - `CFBundleURLSchemes` value
- `Platforms/Windows/Package.appxmanifest` - `Protocol Name` value

## Step 4: Build and Test

### 4.1 Build the Project

```bash
dotnet build
```

### 4.2 Test on Different Platforms

#### Android

```bash
dotnet build -f net8.0-android
```

#### iOS

```bash
dotnet build -f net8.0-ios
```

#### Windows

```bash
dotnet build -f net8.0-windows10.0.19041.0
```

## Step 5: Usage

### 5.1 Navigation

- Users can navigate to `/login` to sign in
- After successful authentication, they'll be redirected to the home page
- The profile page (`/profile`) will show user details from Google

### 5.2 User Details

The following user information is available after Google authentication:

- Name (full name)
- Email address
- Profile picture URL
- Given name and family name
- Email verification status
- Account creation date
- Last sign-in date

## Troubleshooting

### Common Issues

1. **"WebAuthenticator is not supported"**

   - Ensure you're running on a supported platform (Android, iOS, Windows)
   - Check that the Microsoft.Maui.Authentication package is properly installed

2. **"Invalid redirect URI"**

   - Verify the redirect URI matches exactly in both Supabase and Google Cloud Console
   - Check that the application ID is consistent across all platform configurations

3. **"Authentication failed"**

   - Verify your Supabase URL and anon key are correct
   - Check that Google OAuth is properly configured in Supabase
   - Ensure your Google OAuth credentials are valid

4. **"Network error"**
   - Check internet connectivity
   - Verify the app has network permissions

### Debug Information

The authentication service includes comprehensive logging. Check the console output for detailed error messages.

## Security Notes

1. Never commit your actual Supabase credentials to version control
2. Use environment variables or secure configuration for production
3. Regularly rotate your OAuth credentials
4. Monitor authentication logs in Supabase dashboard

## Additional Features

The authentication system includes:

- Automatic token refresh
- Secure local storage of user details
- Cross-platform compatibility
- Event-driven user state management
- Automatic session restoration

For more information, refer to:

- [Supabase Auth Documentation](https://supabase.com/docs/guides/auth)
- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [.NET MAUI Authentication](https://docs.microsoft.com/en-us/dotnet/maui/platform-integration/communication/authentication)
