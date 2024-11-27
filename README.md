# Jellyfin Cloudflare Access Authentication Plugin

This plugin integrates Cloudflare Access with Jellyfin, enabling JWT-based authentication to enhance your server's security.

## Features

- **Cloudflare Access Integration**: Utilizes JWT tokens from Cloudflare Access for user authentication.
- **Fallback Authentication**: If a valid token isn't present and the provider is set to default, the plugin defers to other configured authentication providers.
- **Customizable Settings**: Configure token parameters to meet your security requirements.

## Prerequisites

- **Jellyfin Server**: Ensure you have a running instance of Jellyfin.
- **Cloudflare Access**: Set up Cloudflare Access for your domain.

## Installation

1. **Download the Plugin**:
   - Visit the [Releases](https://github.com/RedNoodlesOrg/jellyfin-plugin-cfauth/releases) page.
   - Download the latest release ZIP file.

2. **Install the Plugin**:
   - Extract the contents of the ZIP file.
   - Place the extracted folder into the `plugins` directory of your Jellyfin server.
   - Restart the Jellyfin server to load the new plugin.

## Configuration

1. **Access Plugin Settings**:
   - Log in to your Jellyfin dashboard.
   - Navigate to **Dashboard** > **Plugins**.
   - Locate the **Cloudflare Access Authentication Plugin** and click on **Settings**.

2. **Configure Token Settings**:
   - Enter your Cloudflare Access JWT settings, including:
     - **Teamname**: Your Cloudflare Access Team name.
     - **Audience**: The audience claim expected in the token. This can be multiple audiences separated with a comma.
   - Save the settings.

## Usage

With the plugin configured, incoming requests to your Jellyfin server will be authenticated using JWT tokens from Cloudflare Access.

**Note**: If you have other applications, such as [Jellyseerr](Link to repo), that authenticate through Jellyfin, ensure that plugin is configured to accept both audience claims. Additionally, these applications must forward the necessary headers or cookies to maintain seamless authentication. I had to patch Jellyseerr in order for it to forward the headers, this is not included in this plugin.

## Support

For issues or feature requests, please open an [issue](https://github.com/RedNoodlesOrg/jellyfin-plugin-cfauth/issues) on the GitHub repository.

## License

This project is licensed under the [MIT License](LICENSE).

*Note: This plugin is in not under active development anymore since it's working for me. Contributions and feedback are welcome.* 
