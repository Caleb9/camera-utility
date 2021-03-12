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

There are two sub commands to use: `copy` and `move`:

```
Usage:
  camera-utility [options] [command]

Options:
  --version         Show version information
  -?, -h, --help    Show help and usage information

Commands:
  copy    Copy files
  move    Move files
```

Both commands take the following options:

```
Options:
  -s, --src-path <src-path> (REQUIRED)    Path to a camera file (image or video) or a directory containing camera
                                          files. When a directory is specified, all sub-directories will be
                                          scanned as well.
  -d, --dst-dir <dst-dir> (REQUIRED)      Destination directory root path where files will be copied or moved
                                          into auto-created sub-directories named after file creation date (e.g.
                                          2019_08_22/), unless --skip-date-subdir option is present.
  -n, --dry-run                           If present, no actual files will be transferred. The output will
                                          contain information about source and destination paths.
  -k, --keep-going                        Try to continue operation when errors for individual files occur.
  --skip-date-subdir                      Do not create date sub-directories in destination directory.
  -?, -h, --help                          Show help and usage information
```

The `--src-path` and `--dst-dir` are the only required
options. `--dry-run`, `--keep-going` and `--skip-date-subdir` can be
added in any combination.

By default (if `--keep-going` is not used), the application will bail
out on first error, e.g. if it cannot read file's EXIF metadata (this
can happen for pictures taken with old phone for example). It will
also **not overwrite any existing files**.

With `--keep-going`, there's a report printed by the end containing
list of skipped files and those where metadata could not be read.

I'd recommend trying it first with `--dry-run` option to see if all
the files have valid metadata.

The application is written in .NET Core, I have successfully used in
on both Windows and Linux.


### Examples

```
camera-utility copy --src-path /source/directory/path --dst-path /destination/path --dry-run --keep-going
```

```
camera-utility move --src-path /source/directory/path --dst-path /destination/path --keep-going
```

```
camera-utility copy --src-path /source/file.jpg --dst-path /destination/path --skip-date-subdir
```

etc.


## Things to Do

* Force-overwrite mode.
* List of individual files as input.
