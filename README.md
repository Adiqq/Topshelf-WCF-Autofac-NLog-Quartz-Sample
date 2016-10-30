# Topshelf-WCF-Autofac-NLog-Quartz-Sample

Linux branch for mono version.

First, install mono-complete, http://www.mono-project.com/docs/getting-started/install/linux/

Clone this repository with `git clone https://github.com/Adiqq/Topshelf-WCF-Autofac-NLog-Quartz-Sample.git`

Checkout to this branch with `git checkout linux`

Download NuGet CLI for Windows, to base dir of repository, https://dist.nuget.org/index.html

Example: `wget https://dist.nuget.org/win-x86-commandline/v3.5.0/NuGet.exe`

Run `mono NuGet.exe restore` , it will restore NuGet packages

Run `xbuild` , it is MSBuild equivalent.

Now you can start server with `mono TopshelfWCF.exe`

If you want to start simultaneously server and client, you will need to use 
[screen](https://www.rackaid.com/blog/linux-screen-tutorial-and-how-to/)
or [tmux](http://www.dayid.org/comp/tm.html)

Example for tmux:

Run `tmux`

Run `mono TopshelfWCF.exe`

Press `Ctrl+b d` to detach session

Run `mono TopshelfWCF.Gateway.exe`

