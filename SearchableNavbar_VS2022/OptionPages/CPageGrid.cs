using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchableNavbar.OptionPages
{
    public class CPageGrid : DialogPage
    {
        private bool cShowMacroDefinitions = false;
        private bool cShowEnumerators = false;
        private bool cShowFunctionDefinitions = true;
        private bool cShowEnumerationNames = false;
        private bool cShowLocalVariables = false;
        private bool cShowStructMembers = false;
        private bool cShowFunctionPrototypes = true;
        private bool cShowStructureNames = false;
        private bool cShowTypedefs = false;
        private bool cShowUnionNames = false;
        private bool cShowVariableDefinitions = false;
        private bool cShowExternalVariableDeclarations = false;
        private bool cShowFunctionParameters = false;
        private bool cShowGotoLabels = false;
        private bool cShowMacroParameters = false;

        [Category("General")]
        [DisplayName("Show Macro Definitions")]
        [Description("Enable or disable the display of macro definitions.")]
        public bool CShowMacroDefinitions
        {
            get { return cShowMacroDefinitions; }
            set { cShowMacroDefinitions = value; }
        }

        [Category("General")]
        [DisplayName("Show Enumerators")]
        [Description("Enable or disable the display of enumerators.")]
        public bool CShowEnumerators
        {
            get { return cShowEnumerators; }
            set { cShowEnumerators = value; }
        }

        [Category("General")]
        [DisplayName("Show Function Definitions")]
        [Description("Enable or disable the display of function definitions.")]
        public bool CShowFunctionDefinitions
        {
            get { return cShowFunctionDefinitions; }
            set { cShowFunctionDefinitions = value; }
        }

        [Category("General")]
        [DisplayName("Show Enumeration Names")]
        [Description("Enable or disable the display of enumeration names.")]
        public bool CShowEnumerationNames
        {
            get { return cShowEnumerationNames; }
            set { cShowEnumerationNames = value; }
        }

        [Category("General")]
        [DisplayName("Show Local Variables")]
        [Description("Enable or disable the display of local variables.")]
        public bool CShowLocalVariables
        {
            get { return cShowLocalVariables; }
            set { cShowLocalVariables = value; }
        }

        [Category("General")]
        [DisplayName("Show Struct Members")]
        [Description("Enable or disable the display of struct and union members.")]
        public bool CShowStructMembers
        {
            get { return cShowStructMembers; }
            set { cShowStructMembers = value; }
        }

        [Category("General")]
        [DisplayName("Show Function Prototypes")]
        [Description("Enable or disable the display of function prototypes.")]
        public bool CShowFunctionPrototypes
        {
            get { return cShowFunctionPrototypes; }
            set { cShowFunctionPrototypes = value; }
        }

        [Category("General")]
        [DisplayName("Show Structure Names")]
        [Description("Enable or disable the display of structure names.")]
        public bool CShowStructureNames
        {
            get { return cShowStructureNames; }
            set { cShowStructureNames = value; }
        }

        [Category("General")]
        [DisplayName("Show Typedefs")]
        [Description("Enable or disable the display of typedefs.")]
        public bool CShowTypedefs
        {
            get { return cShowTypedefs; }
            set { cShowTypedefs = value; }
        }

        [Category("General")]
        [DisplayName("Show Union Names")]
        [Description("Enable or disable the display of union names.")]
        public bool CShowUnionNames
        {
            get { return cShowUnionNames; }
            set { cShowUnionNames = value; }
        }

        [Category("General")]
        [DisplayName("Show Variable Definitions")]
        [Description("Enable or disable the display of variable definitions.")]
        public bool CShowVariableDefinitions
        {
            get { return cShowVariableDefinitions; }
            set { cShowVariableDefinitions = value; }
        }

        [Category("General")]
        [DisplayName("Show External Variable Declarations")]
        [Description("Enable or disable the display of external and forward variable declarations.")]
        public bool CShowExternalVariableDeclarations
        {
            get { return cShowExternalVariableDeclarations; }
            set { cShowExternalVariableDeclarations = value; }
        }

        [Category("General")]
        [DisplayName("Show Function Parameters")]
        [Description("Enable or disable the display of function parameters inside function or prototype definitions.")]
        public bool CShowFunctionParameters
        {
            get { return cShowFunctionParameters; }
            set { cShowFunctionParameters = value; }
        }

        [Category("General")]
        [DisplayName("Show Goto Labels")]
        [Description("Enable or disable the display of goto labels.")]
        public bool CShowGotoLabels
        {
            get { return cShowGotoLabels; }
            set { cShowGotoLabels = value; }
        }

        [Category("General")]
        [DisplayName("Show Parameters Inside Macro Definitions")]
        [Description("Enable or disable the display of parameters inside macro definitions.")]
        public bool CShowMacroParameters
        {
            get { return cShowMacroParameters; }
            set { cShowMacroParameters = value; }
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
