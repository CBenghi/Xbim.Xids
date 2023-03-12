:: standard and examples
xcopy ..\..\BuildingSmart\IDS\Development\*.xsd Xbim.InformationSpecifications.NewTests\bsFiles\ /y
xcopy ..\..\BuildingSmart\IDS\Development\*.ids Xbim.InformationSpecifications.NewTests\bsFiles\ /y
:: generator
xcopy ..\..\BuildingSmart\IDS\Documentation\Units.md Xbim.InformationSpecifications.Generator\Files\ /y
xcopy ..\..\BuildingSmart\IDS\Development\*.xsd Xbim.InformationSpecifications.Generator\Files\ /y
pause