# camera-utility

Command line tool to copy photos and videos (e.g. from SD card) and
rename them by date.

[![Build status](https://ci.appveyor.com/api/projects/status/bjyr7h0qwtcx1kby/branch/master?svg=true)](https://ci.appveyor.com/project/Caleb9/camera-utility/branch/master)


## Why?

I've been downloading photos from my Canon EOS (DSLR) camera using EOS
Utility which puts pictures into directories by date, e.g. like this

```
2009_02_19/
├── IMG_0030.JPG
├── IMG_0032.JPG
├── IMG_0034.JPG
├── IMG_0037.JPG
├── IMG_0038.JPG
```

Note that file names are based on sequential number.

Sometimes, I was also downloading photos from my Android phone, which
resulted in this:

```
├── IMG_20190105_120559.jpg
├── IMG_20190105_120600.jpg
└── IMG_20190105_211552.jpg
```

So I wanted to have a tool which would copy those files in a unified
way from both sources. I liked how EOS Utility divided things into
sub-directories by date, but I preferred Android's naming scheme,
where each photo has the creation date in its name.

This small program does just that. It finds all the photos and videos
in the input directory, reads the creation date from EXIF metadata,
and copies the file into a sub-directory derived from the date (this
can be optionally disabled), renaming it so that the name is also
derived from the creation date.

In the output directory result looks something like this:

```
2019_01_05/
├── IMG_20190105_120559870.jpg
├── IMG_20190105_120600440.jpg
└── IMG_20190105_211552800.jpg
```

This differs slightly from Android's scheme, in that there are
milliseconds also in the time portion of the name. This is to support
DSLR's High-Speed Shooting mode, where more than one photo per second
is taken.


## Supported File Types

Currently camera-util finds files of following types:
* JPG / JPEG (note that extension stays unaffected)
* CR2 (Canon's raw format)
* DNG (Android's raw format)
* MP4 (Video)
* MOV (Video)


## How to Use

There are three sub-commands to use:
* `copy` and `move`
* `check`

```
Usage:                                                                      │
  camera-utility [options] [command]                                        │
                                                                            │
Options:                                                                    │
  --version         Show version information                                │
  -?, -h, --help    Show help and usage information                         │
                                                                            │
Commands:                                                                   │
  copy, cp <src-path> <dst-dir>    Copies suppported image and video files  │
                                   to destination directory and renames     │
                                   them by date recorded in EXIF metadata.  │
  move, mv <src-path> <dst-dir>    Moves suppported image and video files   │
                                   to destination directory and renames     │
                                   them by date recorded in EXIF metadata.  │
  c, check <src-paths>             Scans file or directory for supported    │
                                   image and video files and checks if      │
                                   EXIF metadata is present. [default: .]
```

Both `copy` and `move` commands take the following options:

```
Arguments:                                                                  │
  <src-path>    Path to a camera file (image or video) or a directory       │
                containing camera files. When a directory is specified,     │
                all sub-directories will be scanned as well.                │
  <dst-dir>     Destination directory root path where files will be copied  │
                or moved into auto-created sub-directories named after      │
                file creation date (e.g. 2019_08_22/), unless               │
                --skip-date-subdir option is present.                       │
                                                                            │
Options:                                                                    │
  -n, --dry-run         If present, no actual files will be transferred.    │
                        The output will contain information about source    │
                        and destination paths.                              │
  -k, --keep-going      Try to continue operation when errors for           │
                        individual files occur.                             │
  --skip-date-subdir    Do not create date sub-directories in destination   │
                        directory.                                          │
  --overwrite           Transfer files even if they already exist in        │
                        destination.
```

The `<src-path>` and `<dst-dir>` are the only required
arguments. Remaining options can be added in any combination.

By default (when `--keep-going` is not used), the application will
bail out on first error, e.g. if it cannot read file's EXIF metadata
(this can happen for pictures taken with old phone for example). It
will also **not overwrite any existing files**, unless `--overwrite`
option is used.

With `--keep-going`, there's a report printed by the end containing
list of skipped files and those where metadata could not be read.

I'd recommend using the `check` command first to see if all the files
have valid metadata.

The application is written in .NET Core, I have successfully used in
on Windows, Linux and macOS.


### Examples

Find all camera files in current directory and check if they contain
necessary metadata to derive date-based name for `copy` or `move`
commands:
```
camera-utility check
```

Check a specific directory:
```
camera-utility check /source/directory/path
```

Execute a test run to see how files would be copied and renamed:
```
camera-utility copy /source/directory/path /destination/path --dry-run --keep-going
```

Move files and ignore errors for files which don't contain metadata:
```
camera-utility move /source/directory/path /destination/path --keep-going
```

Copy files directly into `/destination/path` without the date
sub-directory:
```
camera-utility copy /source/file.jpg /destination/path --skip-date-subdir
```

etc.


## Things to Do

* List of multiple individual files as input (currently only a single
  file or entire directory is supported)
