@echo off

@echo --------------------------------------------------
@echo Delete the suo *.user *.tmp *.bak *.log files.
@echo --------------------------------------------------

del /f /s /a- *.suo *.user *.tmp *.bak *.log

@echo --------------------------------------------------
@echo Deleted the suo *.user *.tmp *.bak *.log files.
@echo --------------------------------------------------

pause