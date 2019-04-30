@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in the .vs folder and the
ECHO BIN and OBJ folders contained in the following projects
ECHO.
ECHO Aehnlich
ECHO Aehnlich\Components\ServiceLocator
ECHO Aehnlich\Components\Settings\Settings
ECHO Aehnlich\Components\Settings\SettingsModel
ECHO.
ECHO AehnlichLib
ECHO AehnlichLib_UnitTests
ECHO AehnlichViewLib
ECHO Demos\TextFileDemo\AehnlichViewModelsLib
ECHO Demos\TextFileDemo\AehnlichFileDemo
ECHO Demos\Dir\AehnlichDirDemo
ECHO Demos\Dir\AehnlichDirViewModelLib
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
ECHO Deleting BIN and OBJ Folders in Aehnlich
ECHO.
RMDIR /S /Q "Aehnlich\Aehnlich\bin"
RMDIR /S /Q "Aehnlich\Aehnlich\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in ServiceLocator
ECHO.
RMDIR /S /Q "Aehnlich\Components\ServiceLocator\bin"
RMDIR /S /Q "Aehnlich\Components\ServiceLocator\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Aehnlich
ECHO.
RMDIR /S /Q "Aehnlich\Aehnlich\bin"
RMDIR /S /Q "Aehnlich\Aehnlich\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Settings
ECHO.
RMDIR /S /Q "Aehnlich\Components\Settings\Settings\bin"
RMDIR /S /Q "Aehnlich\Components\Settings\Settings\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in SettingsModel
ECHO.
RMDIR /S /Q "Aehnlich\Components\Settings\SettingsModel\bin"
RMDIR /S /Q "Aehnlich\Components\Settings\SettingsModel\obj"

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
ECHO Deleting BIN and OBJ Folders in Demos\TextFileDemo\AehnlichViewModelsLib
ECHO.
RMDIR /S /Q "Demos\TextFileDemo\AehnlichViewModelsLib\bin"
RMDIR /S /Q "Demos\TextFileDemo\AehnlichViewModelsLib\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\TextFileDemo\AehnlichFileDemo
ECHO.
RMDIR /S /Q "Demos\TextFileDemo\AehnlichFileDemo\bin"
RMDIR /S /Q "Demos\TextFileDemo\AehnlichFileDemo\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\Dir\AehnlichDirDemo
ECHO.
RMDIR /S /Q "Demos\Dir\AehnlichDirDemo\bin"
RMDIR /S /Q "Demos\Dir\AehnlichDirDemo\obj"

ECHO.
ECHO Deleting BIN and OBJ Folders in Demos\AehnlichFileDemo
ECHO.
RMDIR /S /Q "Demos\Dir\AehnlichDirViewModelLib\bin"
RMDIR /S /Q "Demos\Dir\AehnlichDirViewModelLib\obj"

PAUSE

:EndOfBatch
