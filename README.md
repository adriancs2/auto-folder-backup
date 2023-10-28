# Automatic Folder Backup for File Server (Windows) for Small Office

![https://raw.githubusercontent.com/adriancs2/auto-folder-backup/main/wiki/screenshot02.png](https://raw.githubusercontent.com/adriancs2/auto-folder-backup/main/wiki/screenshot02.png)

Download the program at the "![Release](https://github.com/adriancs2/auto-folder-backup/releases)" section at the right side of this page.

To begin with, please run "Setup.exe" for setting up the parameters, or else the program will do nothing until the required parameters are set.

Then, to avoid accidentally human error for modifying the setup parameters, rename "Setup.exe" to "Setup", or remove the file "Setup.exe".

## Features

- Able to perform both full and incremental backup for selected folder.

## Limitations

- Designed to backup only 1 folder. i.e. a file server of a small office.
- Rely on Windows Task Scheduler to trigger the daily or weekly backup.
- Require two (or more) designated drives to run the backup.

## Recommended Hard Disk Setup

- 1st hard disk: A 250GB SSD hard disk for Windows OS (Drive C)
- 2nd hard disk: A 500GB high grade SSD hard disk for the main file server storage (Drive D)<br />This is usually the target folder.<br />Reference for a high grade SSD: https://ssd.userbenchmark.com/
- 3rd hard disk: A 2-4TB large capacity SSD hard disk for backup, partitioned into two or more drives

## How does this program work

Assume that you set the "Total Days For Full Backup" as 7 days. For every interval of 7 days, the program will create a folder with the name time stamp (i.e. 2023-10-28 010000), everything from the target folder will be copied here.

- At start, the program will select the first selected drive for the backup destination.
- When the first drive is full, select second drive as destination drive.
- When the second drive is full, select the third drive (if there is a third drive).
- When the third drive is full, format the first drive (a quick way to erase everything), start using first drive as destination.

Now this is for the full backup. For every other day, the program will perform incremental backup in the latest created destination folder among the drives. It will compare all the files, if the file from the target folder is newer than the destination folder, the file will be overwritten by the newer file.

There are two types of log file will be created along side with the backup operation. The "Main Log" and the "Files Backup Log".

**Main Log**

- Located at the root folder of the program. This records all the main operation events. Including success and failed events.
- Filename: `log.txt` and `log-old.txt` for old log.
- Max file size: 1MB

```
Example of Main Log
...
2023-10-28 03:35:33  Process started
2023-10-28 03:35:33  Acquired target backup folder: J:\dd
2023-10-28 03:35:33  Acquired total size to backup: 1.455 GB (0.000 seconds)
2023-10-28 03:35:33  Executing full backup
2023-10-28 03:35:33  Acquired destination folder: K:\2023-10-28 033533 (0.004 seconds)
2023-10-28 03:35:33  Backup process begin
2023-10-28 03:35:35  Backup process ended - 0 h 0 m 1 s 667 ms
2023-10-28 03:35:35  Exit program gracefully

2023-10-28 03:40:31  Process started
2023-10-28 03:40:31  Acquired target backup folder: J:\dd
2023-10-28 03:40:31  Acquired total size to backup: 1.455 GB (0.000 seconds)
2023-10-28 03:40:31  Total days for full backup: 7, Days since last backup: 0.0
2023-10-28 03:40:31  Executing incremental backup
2023-10-28 03:40:31  Acquired destination folder: K:\2023-10-28 033533 (0.004 seconds)
2023-10-28 03:40:31  Backup process begin
2023-10-28 03:40:31  Backup process ended - 0 h 0 m 0 s 2 ms
2023-10-28 03:40:31  Exit program gracefully
...
```

**Files Backup Log**

- Located inside the root folder of every destination folder.
- Filename: `log-<datetime>-success.txt`, `log-<datetime>-fail.txt` and `log-<datetime>-stat.txt`
- Contains the list of successful and files that failed to back up.
- `log-<datetime>-stat.txt` contains files count. i.e. Total Files, Total Success, Total Failed.

## Setup Windows Task Scheduler

- Open Windows Task Scheduler, create a Task.
- Set the task scheduler's action to run [auto_folder_backup.exe]
- Run the task scheduler with administrative user or System
- Run it whether user is logged on or not
- Run with highest privileges
- Set a trigger with your preferred execution time (i.e. 3am)

Questino: What if I need to backup multiple folders that I can't put them within a single folder to begin with?

Answer: you can copy "Auto Folder Backup" to multiple folders and setup the parameters individually, then create multiple Task Schedulers for each target folder.
