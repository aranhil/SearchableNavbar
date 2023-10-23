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
    [ProvideOptionPage(typeof(CppPageGrid), "Searchable Navbar", "C/C++", 0, 0, true)]
    [ProvideOptionPage(typeof(CSharpPageGrid), "Searchable Navbar", "C#", 0, 0, true)]
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

            CSharpPageGrid cSharpPage = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
            cSharpPage.SettingsChanged += optionsChanged;
        }

        public void UnregisterFromOptionsChangeEvents(EventHandler optionsChanged)
        {
            OptionPageGrid optionPage = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            optionPage.SettingsChanged -= optionsChanged;

            CppPageGrid cppPage = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
            cppPage.SettingsChanged -= optionsChanged;

            CSharpPageGrid cSharpPage = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
            cSharpPage.SettingsChanged -= optionsChanged;
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

        public bool IncludeScopeInSearch
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.IncludeScopeInSearch;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.IncludeScopeInSearch = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool IncludeSignatureInSearch
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.IncludeSignatureInSearch;
            }
            set
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                page.IncludeSignatureInSearch = value;
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

        public string CppIgnoredMacros
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppIgnoredMacros;
            }
            set
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                page.CppIgnoredMacros = value;
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

        public bool CSharpShowClasses
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowClasses;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowClasses = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowMacroDefinitions
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowMacroDefinitions;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowMacroDefinitions = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowEnumerators
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowEnumerators;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowEnumerators = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowEvents
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowEvents;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowEvents = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowFields
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowFields;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowFields = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowEnumerationNames
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowEnumerationNames;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowEnumerationNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowInterfaces
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowInterfaces;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowInterfaces = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowLocalVariables
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowLocalVariables;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowLocalVariables = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowMethods
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowMethods;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowMethods = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowNamespaces
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowNamespaces;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowNamespaces = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowProperties
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowProperties;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowProperties = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowStructureNames
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowStructureNames;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowStructureNames = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }

        public bool CSharpShowTypedefs
        {
            get
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                return page.CSharpShowTypedefs;
            }
            set
            {
                CSharpPageGrid page = (CSharpPageGrid)GetDialogPage(typeof(CSharpPageGrid));
                page.CSharpShowTypedefs = value;
                page.SaveSettingsToStorage();
                page.OnSettingsChanged();
            }
        }
    }
}
