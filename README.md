# Searchable Navbar
![GitHub issues](https://img.shields.io/github/issues/aranhil/SearchableNavbar)
![Visual Studio Marketplace Installs](https://img.shields.io/visual-studio-marketplace/i/Stefan-IulianChivu.SearchableNavBar-x64)
![Visual Studio Marketplace Downloads](https://img.shields.io/visual-studio-marketplace/d/Stefan-IulianChivu.SearchableNavBar-x64)
![GitHub](https://img.shields.io/github/license/aranhil/SearchableNavbar)

*Searchable Navbar* is a Visual Studio extension that replaces the standard navigation bar with a searchable navigation bar. It uses [Universal Ctags](https://github.com/universal-ctags/ctags) to generate the function tags, so any language supported by Universal Ctags is also supported by this extension.

![image](https://github.com/aranhil/SearchableNavbar/assets/755601/1724c5cf-f715-48e3-940c-60237f70ee8b)

## How to set up
1. After installing the extension, an additional navigation bar will be available at the top part of the active document. You can assign a shortcut to open the navigation bar and start searching by going to **Tools >> Options >> Environment >> Keyboard**, the command name is **SearchableNavbar.Open**.
2. Optionally, you can disable the default Visual Studio navigation bar from **Tools >> Options >> Text Editor >> (Language) >> Navigation Bar**.
3. For more options right click the navigation bar or go to **Tools >> Options >> Searchable Navbar**.
![image](https://github.com/aranhil/SearchableNavbar/assets/755601/644f8944-7b40-4c93-acac-1284faff28bf)

## Changelog
### Version 1.18 (23/10/2023)

- Optimized the navbar to be more responsive
- Fixed issue where an empty navbar would still remain visible
- Fixed issue with the icons not using the proper color based on the current theme
- Added options to toggle the inclusion of scope or signature into the search

### Version 1.17 (22/10/2023)

- Added options pages and a contextual menu
- Added more tag types that can be displayed for C/C++ and C#

### Version 1.16 (10/10/2023)

- Added support for ignorable file extensions

### Version 1.15 (17/09/2023)

- Added support for ignorable macros in C/C++ and added all the Unreal Engine macros to the list by default

### Version 1.14 (29/08/2023)

- Added support for multiple visible documents

### Version 1.13 (04/06/2023)

- The search bar will now remain hidden if there's nothing to display
- Added the parameter list to be both displayed and searchable
- The document is reparsed if it's saved through Visual Studio after being previously modified
- If the initial parsing fails, a reparse can be initiated by executing the command to display the search bar

### Version 1.12 (31/05/2023)

- Hotfix for lots of redundant processes being created and slowing down Visual Studio
### Version 1.11 (27/05/2023)

- Calling ctags in a separate process for better stability
- Added support for all languages that are supported by Universal Ctags
- Added a different style to the scope

## License

*Searchable Navbar* is licensed under the [MIT License](LICENSE).

## Support

If you encounter a bug or want to suggest a feature, please open an issue in the [GitHub repository](https://github.com/aranhil/SearchableNavbar/issues).

## Acknowledgements

This project has benefited from the use of the following open-source projects:

- [Universal Ctags]([https://github.com/zeux/qgrep](https://github.com/universal-ctags/ctags)): This project is licensed under the [GPL-2.0 license](./LICENSE-ctags.md).
