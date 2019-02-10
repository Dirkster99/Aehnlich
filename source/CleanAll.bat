@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in the .vs folder and the
ECHO BIN and OBJ folders contained in the following projects
ECHO.
ECHO DiffLib
ECHO DiffLib_UnitTests
ECHO DiffViewLib
ECHO Demos\DiffLibViewModels
ECHO Demos\GenericDemo
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
ECHO Deleting BIN and OBJ Folders in Diff.Net
ECHO.
RMDIR /S /Q "DiffLib\bin"
RMDIR /S /Q "DiffLib\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Libs/Menees.Diffs
ECHO.
RMDIR /S /Q "DiffLib_UnitTests\bin"
RMDIR /S /Q "DiffLib_UnitTests\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Libs/Menees.Common
ECHO.
RMDIR /S /Q "DiffViewLib\bin"
RMDIR /S /Q "DiffViewLib\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Libs/Menees.Diffs.Controls
ECHO.
RMDIR /S /Q "Demos\DiffLibViewModels\bin"
RMDIR /S /Q "Demos\DiffLibViewModels\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\GenericDemo
ECHO.
RMDIR /S /Q "Demos\GenericDemo\bin"
RMDIR /S /Q "Demos\GenericDemo\obj"

PAUSE

:EndOfBatch
