# IOInfoExtensions
This contains some quality of life improvements to the [*System.IO.DirectoryInfo*](https://learn.microsoft.com/en-us/dotnet/api/system.io.directoryinfo) and [*System.IO.FileInfo*](https://learn.microsoft.com/en-us/dotnet/api/system.io.fileinfo) classes that I find useful. The core code is written for dotnet but I also created a PowerShell wrapper as a PowerShell module so it can be used there as well.

## Dotnet
The [**IOInfoExtensions**](src/IOInfoExtensions/) project generates a nuget package that can be imported into your project and referenced.

- https://www.nuget.org/packages/IOInfoExtensions/

## PowerShell
The [**IOInfoExtensions.PowerShell**](src/IOInfoExtensions.PowerShell/) project creates a PowerShell module that can be imported in 5.1+. It contians [wrappers](src/IOInfoExtensions.PowerShell/ExtensionsWrapper.cs) around [**IOInfoExtensions**](src/IOInfoExtensions/) methods and imports them as types into PowerShell. This allows native `.methodName()` usage.

 - https://www.powershellgallery.com/packages/IOInfoExtensions.PowerShell

## Technical Documentation
The technical documentation with examples can be found [here](https://apstrickland.github.io/IOInfoExtensions/).
