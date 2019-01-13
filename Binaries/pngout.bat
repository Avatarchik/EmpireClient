cd ..\assets\

for /R %%i in (*.png) do ..\Binaries\pngout.exe "%%i" /c6 /kpHYs

pause