@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in the .vs folder and the
ECHO BIN and OBJ folders contained in the following projects
ECHO.
ECHO AehnlichLib
ECHO AehnlichLib_UnitTests
ECHO AehnlichViewLib
ECHO Demos\AehnlichLibViewModels
ECHO Demos\AehnlichDemo
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO Removing vs settings folder with *.sou file
ECHO.
RMDIR /S /Q .vs

ECHO.
ECHO Deleting BIN and OBJ Folders in AehnlichLib
ECHO.
RMDIR /S /Q "AehnlichLib\bin"
RMDIR /S /Q "AehnlichLib\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in AehnlichLib_UnitTests
ECHO.
RMDIR /S /Q "AehnlichLib_UnitTests\bin"
RMDIR /S /Q "AehnlichLib_UnitTests\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in AehnlichViewLib
ECHO.
RMDIR /S /Q "AehnlichViewLib\bin"
RMDIR /S /Q "AehnlichViewLib\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos/AehnlichLibViewModels
ECHO.
RMDIR /S /Q "Demos\AehnlichLibViewModels\bin"
RMDIR /S /Q "Demos\AehnlichLibViewModels\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\GenericDemo
ECHO.
RMDIR /S /Q "Demos\AehnlichDemo\bin"
RMDIR /S /Q "Demos\AehnlichDemo\obj"

PAUSE

:EndOfBatch
