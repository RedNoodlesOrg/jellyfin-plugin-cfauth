# Jellyfin Cloudflare Access Authentication Plugin

This plugin integrates Cloudflare Access with Jellyfin, enabling JWT-based authentication. With this setup, users only need to authenticate once through Cloudflare Access, eliminating the need for repeated authentication within Jellyfin.

## Features

- **Cloudflare Access Integration**: Leverages JWT tokens from Cloudflare Access for secure user authentication.
- **Customizable Security Settings**: Offers configurable token parameters to align with specific security requirements.

## Caveats

- **Login Form Requirement**: Users are still required to enter a value in the Jellyfin login form and press the login button, even though authentication is managed by Cloudflare.
- **Device Compatibility**: Using Jellyfin behind Cloudflare Access may be incompatible with smart TVs or other devices relying on native apps instead of web browsers.

## Prerequisites

- **Jellyfin Server**: Ensure you have a running instance of Jellyfin. If not, refer to the [official Jellyfin installation guide](https://jellyfin.org/docs/general/administration/installing.html) to get started.
- **Cloudflare Access**: Set up Cloudflare Access for your domain. Follow the [Cloudflare Zero Trust Get Started guide](https://developers.cloudflare.com/cloudflare-one/setup/) for setup instructions. To understand JWT validation, consult Cloudflare's [Validate JWTs guide](https://developers.cloudflare.com/cloudflare-one/identity/authorization-cookie/validating-json/).

## Installation

### Step 1: Download the Plugin

- Visit the [Releases](https://github.com/RedNoodlesOrg/jellyfin-plugin-cfauth/releases) page.
- Download the latest release ZIP file.

### Step 2: Install the Plugin

- Extract the contents of the ZIP file.
- Place the extracted folder into the `plugins` directory of your Jellyfin server. For more detailed installation instructions, refer to the [official Jellyfin plugin installation guide](https://jellyfin.org/docs/general/server/plugins/#installing).
- Restart the Jellyfin server to load the new plugin.

## Configuration

### Step 1: Access Plugin Settings

- Log in to your Jellyfin dashboard.
- Navigate to **Dashboard** > **Plugins**.
- Locate the **Cloudflare Access Authentication Plugin** and click on **Settings**.

### Step 2: Configure Token Settings

- Provide your Cloudflare Access JWT settings, including:
  - **Teamname**: The name of your Cloudflare Access team.
  - **Audience**: The audience claim expected in the token. You can specify multiple audiences separated by commas.
  - **CookieName**: The name of the cookie that contains the token. The default value typically works well.
  - **HeaderName**: The name of the header that contains the token. Again, the default value usually suffices.
- Save the settings.

**Note**: Only one of either the cookie or the header must be present, with the cookie taking precedence if both are available.

## Usage

Once configured, incoming requests to your Jellyfin server will be authenticated using JWT tokens issued by Cloudflare Access.

**Note**: If you have other applications, such as [Jellyseerr](https://github.com/Fallenbagel/jellyseerr), that authenticate through Jellyfin, make sure the plugin is configured to accept all relevant audience claims. Audience claims serve as identifiers for the intended recipients of the token. You can configure these by accessing the settings in your Cloudflare Access application and ensuring that all necessary audiences are listed in the plugin configuration. Additionally, these applications must forward the appropriate headers or cookies for authentication. Please note that I had to manually patch Jellyseerr to forward the headers; this patch is not included in this plugin.

## Support

For issues or feature requests, please open an [issue](https://github.com/RedNoodlesOrg/jellyfin-plugin-cfauth/issues) on the GitHub repository.

## License

This project is licensed under the [GPLv3 License](LICENSE).

*Note: While I am not planning any further changes, I will address issues as they arise, and community contributions are highly encouraged.*
