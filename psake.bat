@echo off 
REM c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command "& {.\psake.ps1}"
c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe set-executionpolicy unrestricted
c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -NoProfile -Noninteractive -Command "& {.\psake.ps1}"
