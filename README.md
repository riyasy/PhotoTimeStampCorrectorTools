# PhotoTimeStampCorrectorTools

# TimeAdjusterConsole
A console utility to get timestamp from file name (not from actual file) and then increment the date time by the input seconds given in console and sets the modified datetime of the file using the incremented file.
Other tools like exiftool.exe can be used to get the modified datetime and setit in the metadata of actual file.
This was originally used to be run as a context menu action to correct date time for multiple files at once.

# TimeSyncer
A windows GUI utility to visually sync the time between two folders. For e.g two cameras can cover the same event and one camera can have a wrong date and time set. This utility can be used to see the photos from multiple cameras at once, make corrections in time for each folder to sync all of the photos from different camera. This is also a non-destructive tool which doesnt write the metadata. Only modified datetime is corrected by the tool. Other tools like exiftool.exe can then be used to write the actual metadata correctly.
