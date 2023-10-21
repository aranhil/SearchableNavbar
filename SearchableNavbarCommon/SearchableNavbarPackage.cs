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
using Task = System.Threading.Tasks.Task;

namespace SearchableNavbar
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(SearchableNavbarPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionPageGrid), "Searchable Navbar", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(CppPageGrid), "Searchable Navbar", "C++", 0, 0, true)]
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

        public bool SortAlphabetically
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.SortAlphabetically;
            }
        }

        public bool ShowFullyQualifiedTags
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.ShowFullyQualifiedTags;
            }
        }

        public bool ShowAnonymousTags
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.ShowAnonymousTags;
            }
        }

        public string IgnoredCppMacros
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.IgnoredCppMacros;
            }
        }

        public string IgnoredFileExtensions
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.IgnoredFileExtensions;
            }
        }

        public bool CppShowMacroDefinitions
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowMacroDefinitions;
            }
        }

        public bool CppShowFunctionDefinitions
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowFunctionDefinitions;
            }
        }

        public bool CppShowEnumerators
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowEnumerators;
            }
        }

        public bool CppShowEnumerationNames
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowEnumerationNames;
            }
        }

        //public bool CppShowIncludedHeaderFiles
        //{
        //    get
        //    {
        //        CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
        //        return page.CppShowIncludedHeaderFiles;
        //    }
        //}

        public bool CppShowLocalVariables
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowLocalVariables;
            }
        }

        public bool CppShowClassStructUnionMembers
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowClassStructUnionMembers;
            }
        }

        public bool CppShowFunctionPrototypes
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowFunctionPrototypes;
            }
        }

        public bool CppShowStructureNames
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowStructureNames;
            }
        }

        public bool CppShowTypedefs
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowTypedefs;
            }
        }

        public bool CppShowUnionNames
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowUnionNames;
            }
        }

        public bool CppShowVariableDefinitions
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowVariableDefinitions;
            }
        }

        public bool CppShowExternalAndForwardVariableDeclarations
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowExternalAndForwardVariableDeclarations;
            }
        }

        public bool CppShowFunctionParameters
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowFunctionParameters;
            }
        }

        public bool CppShowGotoLabels
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowGotoLabels;
            }
        }

        public bool CppShowMacroParameters
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowMacroParameters;
            }
        }

        public bool CppShowClasses
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowClasses;
            }
        }

        public bool CppShowNamespaces
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowNamespaces;
            }
        }

        //public bool CppShowNamespaceAliases
        //{
        //    get
        //    {
        //        CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
        //        return page.CppShowNamespaceAliases;
        //    }
        //}

        //public bool CppShowNamesImportedViaUsingScopeSymbol
        //{
        //    get
        //    {
        //        CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
        //        return page.CppShowNamesImportedViaUsingScopeSymbol;
        //    }
        //}

        public bool CppShowUsingNamespaceStatements
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowUsingNamespaceStatements;
            }
        }

        public bool CppShowTemplateParameters
        {
            get
            {
                CppPageGrid page = (CppPageGrid)GetDialogPage(typeof(CppPageGrid));
                return page.CppShowTemplateParameters;
            }
        }
    }
}
