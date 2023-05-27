# Searchable Navbar
![GitHub issues](https://img.shields.io/github/issues/aranhil/SearchableNavbar)
![GitHub](https://img.shields.io/github/license/aranhil/SearchableNavbar)

*Searchable Navbar* is a Visual Studio extension that replaces the standard navigation bar with a searchable navigation bar. It uses [Universal Ctags](https://github.com/universal-ctags/ctags) to generate the function tags, so any language supported by Universal Ctags is also supported by this extension.

![image](https://github.com/aranhil/SearchableNavbar/assets/755601/c3d62dd8-5196-4813-97d8-54e7f12a53a8)

## Setting up in Visual Studio
## Installation
1. After installing the extension, an additional navigation bar will be available at the top part of the active document. You can assign a shortcut to open the navigation bar and start searching by going to **Tools >> Options >> Environment >> Keyboard**, the command name is **SearchableNavbar.Open**.

## Changelog
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
