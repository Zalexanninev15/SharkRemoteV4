@for /f %%i in ('@wmic path win32_desktopmonitor get pnpdeviceid ^|@find "DISPLAY"') do @set val="HKLM\SYSTEM\CurrentControlSet\Enum\%%i\Device Parameters"
@reg query %val% /v EDID>NUL
@if %errorlevel% GTR 0 @echo BAD EDID&EXIT
@for /f "skip=2 tokens=1,2,3*" %%a in ('@reg query %val% /v EDID') do @set edid=%%c
@set /A Y=%edid:~34,1%*16+%edid:~35,1%+1990
@echo.%Y%