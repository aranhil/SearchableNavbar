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