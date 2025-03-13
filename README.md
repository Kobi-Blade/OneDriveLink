# OneDrive Link

A simple C# console application that processes a user-provided URL, into a direct download link for OneDrive.

## Prerequisites

- [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/Kobi-Blade/OneDriveLink.git
cd OneDriveLink
```

### Build the Application

Use the dotnet CLI tool to build the project:

```bash
dotnet build
```

### Run the Application

When prompted, paste in your URL (for example, https://1drv.ms/u/s!xxx) and press Enter.
The program will process the URL, handle any redirection, and output the direct download URL.

## How It Works

1. User Input and Validation: The application starts by asking for a URL, removes any extra whitespace, and uses Uri.TryCreate to verify that it is a valid absolute URL.
2. HTTP Request with Redirect Disabled: A HttpClientHandler with AllowAutoRedirect set to false is used to send a GET request. This allows the program to inspect the HTTP response for redirection (status codes 302, 301, etc.).
3. Processing the Redirect: If a redirect is detected, the application extracts the Location header and ensures it is not null. It then utilizes a UriBuilder to create the download URL.