Backup-Manager
==============
This program will recursively copy the folder specified by the first argument
into the folder specified by the second argument, skipping all files which have
not been modified since the last backup. By default this program skips the two
most common VirtualBox folder names as well as AppData; this can be overridden
by adding "/a" as a third argument.
