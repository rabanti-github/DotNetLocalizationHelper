# .Net Loclaization Helper [NLH] #

NLH is a tool to load, save, import, export and edit localization resources from .NET assemblies.
The tool is based on the locbaml tool, provided by Microsoft.

## Features

* Loading and Saving of localization resources (DLL, EXE, RESOURCES or BAML files)
* Import and export of loaded localization tables as CSV or XLSX files
* Inline editing of translation strings
* Automatic handling of culture info (e.g. en-US or de-DE)
* Filtering of localization tables and optional hiding of columns


## ToDo

* Implementation of Save (work in progress)
* Check licensing
  * The locbaml console application is currently an altered <strike>verbatim</strike> copy of https://github.com/JeremyDurnell/locbaml)
  * Licensing is still pendinng (Microsoft contacted)
  * The tool was altered to process streams instead of files (as is status not applicable anymore)
  * An independent replacement with a (minimum) functional sub-set of locbaml will be developed if the license of the locbaml tool cannot be determined
* UI implementation (work in progress)


## License

NLH is licensed under the MIT license. However, several dependencies are published under other but compatible licesnses, like BSD 2-clause. The license of the used locbaml component is still under investigation.