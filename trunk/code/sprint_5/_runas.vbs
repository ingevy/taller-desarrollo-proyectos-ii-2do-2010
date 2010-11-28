cleanupScript = ".\_launcher.cmd"
args = ""
Set shell = CreateObject("Shell.Application")
shell.ShellExecute cleanupScript, args, "", "runas"

