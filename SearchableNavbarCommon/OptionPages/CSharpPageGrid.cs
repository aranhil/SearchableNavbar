using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchableNavbar.OptionPages
{
    public class CSharpPageGrid : DialogPage
    {
        private bool cSharpShowClasses = false;
        private bool cSharpShowMacroDefinitions = false;
        private bool cSharpShowEnumerators = false;
        private bool cSharpShowEvents = false;
        private bool cSharpShowFields = false;
        private bool cSharpShowEnumerationNames = false;
        private bool cSharpShowInterfaces = false;
        private bool cSharpShowLocalVariables = false;
        private bool cSharpShowMethods = true;
        private bool cSharpShowNamespaces = false;
        private bool cSharpShowProperties = false;
        private bool cSharpShowStructureNames = false;
        private bool cSharpShowTypedefs = false;

        [Category("General")]
        [DisplayName("Show Classes")]
        [Description("Enable or disable the display of classes.")]
        public bool CSharpShowClasses
        {
            get { return cSharpShowClasses; }
            set { cSharpShowClasses = value; }
        }

        [Category("General")]
        [DisplayName("Show Macro Definitions")]
        [Description("Enable or disable the display of macro definitions.")]
        public bool CSharpShowMacroDefinitions
        {
            get { return cSharpShowMacroDefinitions; }
            set { cSharpShowMacroDefinitions = value; }
        }

        [Category("General")]
        [DisplayName("Show Enumerators")]
        [Description("Enable or disable the display of enumerators.")]
        public bool CSharpShowEnumerators
        {
            get { return cSharpShowEnumerators; }
            set { cSharpShowEnumerators = value; }
        }

        [Category("General")]
        [DisplayName("Show Events")]
        [Description("Enable or disable the display of events.")]
        public bool CSharpShowEvents
        {
            get { return cSharpShowEvents; }
            set { cSharpShowEvents = value; }
        }

        [Category("General")]
        [DisplayName("Show Fields")]
        [Description("Enable or disable the display of fields.")]
        public bool CSharpShowFields
        {
            get { return cSharpShowFields; }
            set { cSharpShowFields = value; }
        }

        [Category("General")]
        [DisplayName("Show Enumeration Names")]
        [Description("Enable or disable the display of enumeration names.")]
        public bool CSharpShowEnumerationNames
        {
            get { return cSharpShowEnumerationNames; }
            set { cSharpShowEnumerationNames = value; }
        }

        [Category("General")]
        [DisplayName("Show Interfaces")]
        [Description("Enable or disable the display of interfaces.")]
        public bool CSharpShowInterfaces
        {
            get { return cSharpShowInterfaces; }
            set { cSharpShowInterfaces = value; }
        }

        [Category("General")]
        [DisplayName("Show Local Variables")]
        [Description("Enable or disable the display of local variables.")]
        public bool CSharpShowLocalVariables
        {
            get { return cSharpShowLocalVariables; }
            set { cSharpShowLocalVariables = value; }
        }

        [Category("General")]
        [DisplayName("Show Methods")]
        [Description("Enable or disable the display of methods.")]
        public bool CSharpShowMethods
        {
            get { return cSharpShowMethods; }
            set { cSharpShowMethods = value; }
        }

        [Category("General")]
        [DisplayName("Show Namespaces")]
        [Description("Enable or disable the display of namespaces.")]
        public bool CSharpShowNamespaces
        {
            get { return cSharpShowNamespaces; }
            set { cSharpShowNamespaces = value; }
        }

        [Category("General")]
        [DisplayName("Show Properties")]
        [Description("Enable or disable the display of properties.")]
        public bool CSharpShowProperties
        {
            get { return cSharpShowProperties; }
            set { cSharpShowProperties = value; }
        }

        [Category("General")]
        [DisplayName("Show Structure Names")]
        [Description("Enable or disable the display of structure names.")]
        public bool CSharpShowStructureNames
        {
            get { return cSharpShowStructureNames; }
            set { cSharpShowStructureNames = value; }
        }

        [Category("General")]
        [DisplayName("Show Typedefs")]
        [Description("Enable or disable the display of typedefs.")]
        public bool CSharpShowTypedefs
        {
            get { return cSharpShowTypedefs; }
            set { cSharpShowTypedefs = value; }
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
