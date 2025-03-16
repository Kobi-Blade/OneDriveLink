# OneDriveLink

A simple C# console application that processes a user-provided URL into a direct download link for OneDrive.

## Prerequisites

- [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime)

## Features

- Convert OneDrive shared links to direct download links
- Handle SharePoint links to extract download URLs
- Argument mode for processing URLs from the command line
- Error handling and logging

## Usage

### Interactive Mode

When you run the application without any arguments, it will prompt you to enter a OneDrive or SharePoint shared URL.

### Argument Mode

You can also run the application by providing the URLs as arguments. This mode is useful for automated scripts and batch processing.

```bash
.\OneDriveLink.exe "link1" "link2" "link3"
```

### Redirecting Output and Error Logs

To capture the output and error logs, you can redirect them to files as follows:

- Redirect standard output to a file:

```bash
.\OneDriveLink.exe "link1" "link2" "link3" > output.txt
```

- Redirect error output to a file:

```bash
.\OneDriveLink.exe "link1" "link2" "link3" 2> errors.txt
```

- Redirect both standard output and error output to separate files:

```bash
.\OneDriveLink.exe "link1" "link2" "link3" > output.txt 2> errors.txt
```

- Redirect both standard output and error output to the same file:

```bash
.\OneDriveLink.exe "link1" "link2" "link3" > all_output.txt 2>&1
```

### Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

### Build Project

This is a .NET 8.0 project and it ensures platform-independent practices for full Linux support. To compile, use the following command:

```bash
dotnet build
```