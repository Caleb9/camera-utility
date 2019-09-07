# camera-utility

Command line tool to copy photos and videos (e.g. from SD card) and
rename them by date.

## How to Use

You need [.NET Core Runtime](https://dotnet.microsoft.com/download) to
run the application.

These are the command line options:
```
  -s, --src-dir     Required. Directory containing pictures and/or videos. All
                    sub-directories will be searched too.

  -d, --dest-dir    Required. Destination directory root path where files will
                    be copied into auto-created sub-directories named after file
                    creation date (e.g. 2019_08_22/).

  -n, --dry-run     (Default: false) If present, no actual files will be copied.
                    The output will contain information about source and
                    destination paths.

  --help            Display this help screen.

  --version         Display version information.
```

*TODO add more description*
