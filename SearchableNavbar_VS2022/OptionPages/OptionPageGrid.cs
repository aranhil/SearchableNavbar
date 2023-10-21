using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchableNavbar
{
    public class OptionPageGrid : DialogPage
    {
        private bool sortAlphabetically = false;
        private bool showFullyQualifiedTags = true;
        private bool showAnonymousTags = false;
        private string ignoredCppMacros = "UPROPERTY+,UFUNCTION+,USTRUCT+,UMETA+,UPARAM+,UENUM+,UDELEGATE+,RIGVM_METHOD+";
        private string ignoredFileExtensions = "";

        [Category("General")]
        [DisplayName("Sort Alphabetically")]
        [Description("Enable or disable sorting items alphabetically.")]
        public bool SortAlphabetically
        {
            get { return sortAlphabetically; }
            set { sortAlphabetically = value; }
        }

        [Category("General")]
        [DisplayName("Show Fully Qualified Tags")]
        [Description("Enable or disable the display of fully qualified tags.")]
        public bool ShowFullyQualifiedTags
        {
            get { return showFullyQualifiedTags; }
            set { showFullyQualifiedTags = value; }
        }

        [Category("General")]
        [DisplayName("Show Anonymous Tags")]
        [Description("Enable or disable the display of anonymous tags.")]
        public bool ShowAnonymousTags
        {
            get { return showAnonymousTags; }
            set { showAnonymousTags = value; }
        }

        [Category("General")]
        [DisplayName("Ignored C/C++ Macros")]
        [Description("A list of macros that will be ignored in C/C++")]
        public string IgnoredCppMacros
        {
            get { return ignoredCppMacros; }
            set { ignoredCppMacros = value; }
        }

        [Category("General")]
        [DisplayName("Ignored File Extensions")]
        [Description("A list of file extensions that will be ignored")]
        public string IgnoredFileExtensions
        {
            get { return ignoredFileExtensions; }
            set { ignoredFileExtensions = value; }
        }
    }
}
