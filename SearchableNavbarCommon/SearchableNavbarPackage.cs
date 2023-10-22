using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using SearchableNavbar.OptionPages;
using Task = System.Threading.Tasks.Task;

namespace SearchableNavbar
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(SearchableNavbarPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionPageGrid), "Searchable Navbar", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(CppPageGrid), "Searchable Navbar", "C++", 0, 0, true)]
    [ProvideOptionPage(typeof(CPageGrid), "Searchable Navbar", "C", 0, 0, true)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class SearchableNavbarPackage : AsyncPackage
    {
        public const string PackageGuidString = "5ff8b546-f79f-471f-805a-c89eabc400b2";

        public IVsEditorAdaptersFactoryService Editor;
        public IVsTextManager TextManager;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            TextManager = (IVsTextManager)await GetServiceAsync(typeof(SVsTextManager));

            IComponentModel componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));
            if(componentModel != null)
            Editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            await FocusCommand.InitializeAsync(this);
        }

        public void RegisterToOptionsChangeEvents(EventHandler optionsChanged)
        {
            OptionPageGrid optionPage = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            optionPage.SettingsChanged += optionsChanged;

            CppPageGrid cppPage = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
            cppPage.SettingsChanged += optionsChanged;

            CPageGrid cPage = (CPageGrid)GetDialogPage(typeof(CPageGrid));
            cPage.SettingsChanged += optionsChanged;
        }

        public void UnregisterFromOptionsChangeEvents(EventHandler optionsChanged)
        {
            OptionPageGrid optionPage = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            optionPage.SettingsChanged -= optionsChanged;

            CppPageGrid cppPage = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
            cppPage.SettingsChanged -= optionsChanged;

            CPageGrid cPage = (CPageGrid)GetDialogPage(typeof(CPageGrid));
            cPage.SettingsChanged -= optionsChanged;
        }

        public void ShowSettings()
        {
            ShowOptionPage(typeof(OptionPageGrid));
        }

        public bool SortAlphabetically
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.SortAlphabetically;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.SortAlphabetically = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool ShowFullyQualifiedTags
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.ShowFullyQualifiedTags;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.ShowFullyQualifiedTags = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool ShowAnonymousTags
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.ShowAnonymousTags;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.ShowAnonymousTags = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool ShowTagSignature
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.ShowTagSignature;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.ShowTagSignature = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public string IgnoredCppMacros
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.IgnoredCppMacros;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.IgnoredCppMacros = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public string IgnoredFileExtensions
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.IgnoredFileExtensions;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.IgnoredFileExtensions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowMacroDefinitions
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowMacroDefinitions;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowMacroDefinitions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowFunctionDefinitions
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowFunctionDefinitions;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowFunctionDefinitions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowEnumerators
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowEnumerators;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowEnumerators = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowEnumerationNames
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowEnumerationNames;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowEnumerationNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowLocalVariables
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowLocalVariables;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowLocalVariables = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowClassStructUnionMembers
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowClassStructUnionMembers;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowClassStructUnionMembers = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowFunctionPrototypes
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowFunctionPrototypes;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowFunctionPrototypes = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowStructureNames
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowStructureNames;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowStructureNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowTypedefs
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowTypedefs;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowTypedefs = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowUnionNames
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowUnionNames;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowUnionNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowVariableDefinitions
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowVariableDefinitions;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowVariableDefinitions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowExternalAndForwardVariableDeclarations
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowExternalAndForwardVariableDeclarations;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowExternalAndForwardVariableDeclarations = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowFunctionParameters
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowFunctionParameters;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowFunctionParameters = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowGotoLabels
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowGotoLabels;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowGotoLabels = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowMacroParameters
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowMacroParameters;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowMacroParameters = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowClasses
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowClasses;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowClasses = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowNamespaces
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowNamespaces;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowNamespaces = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowUsingNamespaceStatements
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowUsingNamespaceStatements;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowUsingNamespaceStatements = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CppShowTemplateParameters
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowTemplateParameters;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppShowTemplateParameters = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowMacroDefinitions
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowMacroDefinitions;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowMacroDefinitions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowEnumerators
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowEnumerators;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowEnumerators = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowFunctionDefinitions
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowFunctionDefinitions;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowFunctionDefinitions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowEnumerationNames
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowEnumerationNames;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowEnumerationNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowLocalVariables
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowLocalVariables;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowLocalVariables = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowStructMembers
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowStructMembers;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowStructMembers = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowFunctionPrototypes
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowFunctionPrototypes;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowFunctionPrototypes = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowStructureNames
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowStructureNames;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowStructureNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowTypedefs
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowTypedefs;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowTypedefs = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowUnionNames
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowUnionNames;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowUnionNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowVariableDefinitions
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowVariableDefinitions;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowVariableDefinitions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowExternalVariableDeclarations
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowExternalVariableDeclarations;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowExternalVariableDeclarations = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowFunctionParameters
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowFunctionParameters;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowFunctionParameters = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowGotoLabels
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowGotoLabels;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowGotoLabels = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CShowMacroParameters
        {
            get
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                return page.CShowMacroParameters;
            }
            set
            {
                CPageGrid page = (CPageGrid)GetDialogPage(typeof(CPageGrid));
                page.CShowMacroParameters = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }
    }
}
