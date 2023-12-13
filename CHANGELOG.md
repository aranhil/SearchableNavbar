### Version 1.11 (27/05/2023)

- Calling ctags in a separate process for better stability
- Added support for all languages that are supported by universal ctags
- Added a different style to the scope

### Version 1.12 (31/05/2023)

- Hotfix for lots of redundant processes being created and slowing down Visual Studio

### Version 1.13 (04/06/2023)

- The search bar will now remain hidden if there's nothing to display
- Added the parameter list to be both displayed and searchable
- The document is reparsed if it's saved through Visual Studio after being previously modified
- If the initial parsing fails, a reparse can be initiated by executing the command to display the search bar

### Version 1.14 (29/08/2023)

- Added support for multiple visible documents

### Version 1.15 (17/09/2023)

- Added support for ignorable macros in C/C++ and added all the Unreal Engine macros to the list by default

### Version 1.16 (10/10/2023)

- Added support for ignorable file extensions

### Version 1.17 (22/10/2023)

- Added options pages and a contextual menu
- Added more tag types that can be displayed for C/C++ and C#

### Version 1.18 (23/10/2023)

- Optimized the navbar to be more responsive
- Fixed issue where an empty navbar would still remain visible
- Fixed issue with the icons not using the proper color based on the current theme
- Added options to toggle the inclusion of scope or signature into the search

### Version 1.19 (13/12/2023)

- Added the official ctags executable to the package that doesn't need the Visual C++ Redistributable for Visual Studio 2013