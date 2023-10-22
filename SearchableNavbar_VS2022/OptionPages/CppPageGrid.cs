using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchableNavbar
{
    public class CppPageGrid : DialogPage
    {
        private bool cppShowMacroDefinitions = false;
        private bool cppShowFunctionDefinitions = true;
        private bool cppShowEnumerators = false;
        private bool cppShowEnumerationNames = false;
        //private bool cppShowIncludedHeaderFiles = false;
        private bool cppShowLocalVariables = false;
        private bool cppShowClassStructUnionMembers = false;
        private bool cppShowFunctionPrototypes = true;
        private bool cppShowStructureNames = false;
        private bool cppShowTypedefs = false;
        private bool cppShowUnionNames = false;
        private bool cppShowVariableDefinitions = false;
        private bool cppShowExternalAndForwardVariableDeclarations = false;
        private bool cppShowFunctionParameters = false;
        private bool cppShowGotoLabels = false;
        private bool cppShowMacroParameters = false;
        private bool cppShowClasses = false;
        private bool cppShowNamespaces = false;
        //private bool cppShowNamespaceAliases = false;
        //private bool cppShowNamesImportedViaUsingScopeSymbol = false;
        private bool cppShowUsingNamespaceStatements = false;
        private bool cppShowTemplateParameters = false;

        [Category("General")]
        [DisplayName("Show Macro Definitions")]
        [Description("Enable or disable the display of macro definitions.")]
        public bool CppShowMacroDefinitions
        {
            get { return cppShowMacroDefinitions; }
            set { cppShowMacroDefinitions = value; }
        }

        [Category("General")]
        [DisplayName("Show Function Definitions")]
        [Description("Enable or disable the display of function definitions.")]
        public bool CppShowFunctionDefinitions
        {
            get { return cppShowFunctionDefinitions; }
            set { cppShowFunctionDefinitions = value; }
        }

        [Category("General")]
        [DisplayName("Show Enumerators")]
        [Description("Enable or disable the display of enumerators (values inside an enumeration).")]
        public bool CppShowEnumerators
        {
            get { return cppShowEnumerators; }
            set { cppShowEnumerators = value; }
        }

        [Category("General")]
        [DisplayName("Show Enumeration Names")]
        [Description("Enable or disable the display of enumeration names.")]
        public bool CppShowEnumerationNames
        {
            get { return cppShowEnumerationNames; }
            set { cppShowEnumerationNames = value; }
        }

        //[Category("General")]
        //[DisplayName("Show Included Header Files")]
        //[Description("Enable or disable the display of included header files.")]
        //public bool CppShowIncludedHeaderFiles
        //{
        //    get { return cppShowIncludedHeaderFiles; }
        //    set { cppShowIncludedHeaderFiles = value; }
        //}

        [Category("General")]
        [DisplayName("Show Local Variables")]
        [Description("Enable or disable the display of local variables.")]
        public bool CppShowLocalVariables
        {
            get { return cppShowLocalVariables; }
            set { cppShowLocalVariables = value; }
        }

        [Category("General")]
        [DisplayName("Show Class, Struct, and Union Members")]
        [Description("Enable or disable the display of class, struct, and union members.")]
        public bool CppShowClassStructUnionMembers
        {
            get { return cppShowClassStructUnionMembers; }
            set { cppShowClassStructUnionMembers = value; }
        }

        [Category("General")]
        [DisplayName("Show Function Prototypes")]
        [Description("Enable or disable the display of function prototypes.")]
        public bool CppShowFunctionPrototypes
        {
            get { return cppShowFunctionPrototypes; }
            set { cppShowFunctionPrototypes = value; }
        }

        [Category("General")]
        [DisplayName("Show Structure Names")]
        [Description("Enable or disable the display of structure names.")]
        public bool CppShowStructureNames
        {
            get { return cppShowStructureNames; }
            set { cppShowStructureNames = value; }
        }

        [Category("General")]
        [DisplayName("Show Typedefs")]
        [Description("Enable or disable the display of typedefs.")]
        public bool CppShowTypedefs
        {
            get { return cppShowTypedefs; }
            set { cppShowTypedefs = value; }
        }

        [Category("General")]
        [DisplayName("Show Union Names")]
        [Description("Enable or disable the display of union names.")]
        public bool CppShowUnionNames
        {
            get { return cppShowUnionNames; }
            set { cppShowUnionNames = value; }
        }

        [Category("General")]
        [DisplayName("Show Variable Definitions")]
        [Description("Enable or disable the display of variable definitions.")]
        public bool CppShowVariableDefinitions
        {
            get { return cppShowVariableDefinitions; }
            set { cppShowVariableDefinitions = value; }
        }

        [Category("General")]
        [DisplayName("Show External and Forward Variable Declarations")]
        [Description("Enable or disable the display of external and forward variable declarations.")]
        public bool CppShowExternalAndForwardVariableDeclarations
        {
            get { return cppShowExternalAndForwardVariableDeclarations; }
            set { cppShowExternalAndForwardVariableDeclarations = value; }
        }

        [Category("General")]
        [DisplayName("Show Function Parameters")]
        [Description("Enable or disable the display of function parameters inside function or prototype definitions.")]
        public bool CppShowFunctionParameters
        {
            get { return cppShowFunctionParameters; }
            set { cppShowFunctionParameters = value; }
        }

        [Category("General")]
        [DisplayName("Show Goto Labels")]
        [Description("Enable or disable the display of goto labels.")]
        public bool CppShowGotoLabels
        {
            get { return cppShowGotoLabels; }
            set { cppShowGotoLabels = value; }
        }

        [Category("General")]
        [DisplayName("Show Parameters Inside Macro Definitions")]
        [Description("Enable or disable the display of parameters inside macro definitions.")]
        public bool CppShowMacroParameters
        {
            get { return cppShowMacroParameters; }
            set { cppShowMacroParameters = value; }
        }

        [Category("General")]
        [DisplayName("Show Classes")]
        [Description("Enable or disable the display of classes.")]
        public bool CppShowClasses
        {
            get { return cppShowClasses; }
            set { cppShowClasses = value; }
        }

        [Category("General")]
        [DisplayName("Show Namespaces")]
        [Description("Enable or disable the display of namespaces.")]
        public bool CppShowNamespaces
        {
            get { return cppShowNamespaces; }
            set { cppShowNamespaces = value; }
        }

        //[Category("General")]
        //[DisplayName("Show Namespace Aliases")]
        //[Description("Enable or disable the display of namespace aliases.")]
        //public bool CppShowNamespaceAliases
        //{
        //    get { return cppShowNamespaceAliases; }
        //    set { cppShowNamespaceAliases = value; }
        //}

        //[Category("General")]
        //[DisplayName("Show Names Imported via Using scope::symbol")]
        //[Description("Enable or disable the display of names imported via 'using scope::symbol' declarations.")]
        //public bool CppShowNamesImportedViaUsingScopeSymbol
        //{
        //    get { return cppShowNamesImportedViaUsingScopeSymbol; }
        //    set { cppShowNamesImportedViaUsingScopeSymbol = value; }
        //}

        [Category("General")]
        [DisplayName("Show Using Namespace Statements")]
        [Description("Enable or disable the display of 'using namespace' statements.")]
        public bool CppShowUsingNamespaceStatements
        {
            get { return cppShowUsingNamespaceStatements; }
            set { cppShowUsingNamespaceStatements = value; }
        }

        [Category("General")]
        [DisplayName("Show Template Parameters")]
        [Description("Enable or disable the display of template parameters.")]
        public bool CppShowTemplateParameters
        {
            get { return cppShowTemplateParameters; }
            set { cppShowTemplateParameters = value; }
        }

        public event EventHandler SettingsChanged;

        public virtual void OnSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                OnSettingsChanged();
            }
        }
    }
}
