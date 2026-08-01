@echo off

SET EXTENTION=*.suo *.user *.tmp *.log *.bak *.skrold

@echo --------------------------------------------------
@echo Delete the %EXTENTION%.
@echo --------------------------------------------------

del /f /s /a- %EXTENTION%

@echo --------------------------------------------------
@echo Deleted the %EXTENTION%.
@echo --------------------------------------------------

pause
