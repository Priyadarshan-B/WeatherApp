# Google OAuth Authentication Setup (Alternative Method)

This guide will help you set up direct Google OAuth authentication for your Blazor MAUI app without using Supabase.

## Prerequisites

1. A Google Cloud Console project with OAuth 2.0 credentials
2. .NET 8.0 SDK

## Step 1: Google Cloud Console Setup

### 1.1 Create OAuth 2.0 Credentials

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the Google+ API
4. Go to **APIs & Services** > **Credentials**
5. Click **Create Credentials** > **OAuth 2.0 Client IDs**
6. Choose **Web application** as the application type
7. Add authorized redirect URIs:
   - `com.companyname.blazormauiapp1://auth-callback`

### 1.2 Configure OAuth Consent Screen

1. Go to **OAuth consent screen**
2. Add your app name and description
3. Add your email as a test user
4. Save the configuration

## Step 2: Update App Configuration

### 2.1 Update GoogleAuthConfig.cs

Open `Services/GoogleAuthConfig.cs` and replace the placeholder values:

```csharp
public static class GoogleAuthConfig
{
    // Replace with your actual Google OAuth credentials
    public const string ClientId = "your-google-client-id.apps.googleusercontent.com";
    public const string ClientSecret = "your-google-client-secret";

    // OAuth redirect URI (must match what you configure in Google Cloud Console)
    public const string RedirectUri = "com.companyname.blazormauiapp1://auth-callback";
}
```

### 2.2 Update Application ID (Optional)

If you want to use a different application ID, update it in:

- `BlazorMauiApp1.csproj` - `ApplicationId` property
- `Platforms/Android/AndroidManifest.xml` - `android:scheme` value
- `Platforms/iOS/Info.plist` - `CFBundleURLSchemes` value
- `Platforms/Windows/Package.appxmanifest` - `Protocol Name` value

## Step 3: Build and Test

### 3.1 Build the Project

```bash
dotnet build
```

### 3.2 Test on Different Platforms

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

## Step 4: Usage

### 4.1 Navigation

- Users can navigate to `/login` to sign in
- After successful authentication, they'll be redirected to the home page
- The profile page (`/profile`) will show user details from Google

### 4.2 User Details

The following user information is available after Google authentication:

- Name (full name)
- Email address
- Profile picture URL
- Given name and family name
- Email verification status
- Account creation date
- Last sign-in date

## How It Works

1. **OAuth Flow**: The app opens Google's OAuth page in a web view
2. **User Authentication**: User signs in with their Google account
3. **Code Exchange**: The app exchanges the authorization code for access tokens
4. **User Info**: The app fetches user details using the access token
5. **Local Storage**: User details are stored securely in local storage
6. **Session Management**: The app maintains the user session locally

## Security Features

- **Secure Storage**: User details are stored using MAUI's SecureStorage
- **Token Management**: Access tokens are handled securely
- **Local Session**: No server-side session management required
- **Automatic Logout**: Users can sign out to clear local data

## Troubleshooting

### Common Issues

1. **"WebAuthenticator is not supported"**

   - Ensure you're running on a supported platform (Android, iOS, Windows)
   - Check that the Microsoft.Maui.Authentication package is properly installed

2. **"Invalid redirect URI"**

   - Verify the redirect URI matches exactly in Google Cloud Console
   - Check that the application ID is consistent across all platform configurations

3. **"Authentication failed"**

   - Verify your Google OAuth credentials are correct
   - Check that Google OAuth is properly configured
   - Ensure your OAuth consent screen is configured

4. **"Network error"**
   - Check internet connectivity
   - Verify the app has network permissions

### Debug Information

The authentication service includes comprehensive logging. Check the console output for detailed error messages.

## Advantages of This Method

1. **Simpler Setup**: No need for Supabase or external services
2. **Direct Integration**: Direct Google OAuth without intermediaries
3. **Local Storage**: User data stored locally on the device
4. **Cross-Platform**: Works on all supported MAUI platforms
5. **No Backend**: No server-side code required

## Limitations

1. **Local Only**: User data is not synced across devices
2. **No Server Validation**: Authentication is handled client-side
3. **Manual Token Management**: You need to handle token refresh manually if needed

## Security Notes

1. Never commit your actual Google OAuth credentials to version control
2. Use environment variables or secure configuration for production
3. Regularly rotate your OAuth credentials
4. Monitor authentication logs in Google Cloud Console

For more information, refer to:

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [.NET MAUI Authentication](https://docs.microsoft.com/en-us/dotnet/maui/platform-integration/communication/authentication)
