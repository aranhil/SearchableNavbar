﻿using Microsoft.VisualStudio.Shell;
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
        private bool showTagSignature = true;
        private bool includeScopeInSearch = true;
        private bool includeSignatureInSearch = true;
        private string ignoredFileExtensions = "";
        private string languageMap = "c++:+.cu.cuh.cl.clcpp";

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
        [DisplayName("Show Tag Signature")]
        [Description("Enable or disable the display of the tag signature.")]
        public bool ShowTagSignature
        {
            get { return showTagSignature; }
            set { showTagSignature = value; }
        }

        [Category("General")]
        [DisplayName("Include Scope in Search")]
        [Description("Enable or disable the inclusion of the tag's scope when searching.")]
        public bool IncludeScopeInSearch
        {
            get { return includeScopeInSearch; }
            set { includeScopeInSearch = value; }
        }

        [Category("General")]
        [DisplayName("Include Signature in Search")]
        [Description("Enable or disable the inclusion of the tag's signature when searching.")]
        public bool IncludeSignatureInSearch
        {
            get { return includeSignatureInSearch; }
            set { includeSignatureInSearch = value; }
        }

        [Category("General")]
        [DisplayName("Ignored File Extensions")]
        [Description("A list of file extensions that will be ignored")]
        public string IgnoredFileExtensions
        {
            get { return ignoredFileExtensions; }
            set { ignoredFileExtensions = value; }
        }

        [Category("General")]
        [DisplayName("Language map")]
        [Description("A map of file extensions to languages")]
        public string LanguageMap
        {
            get { return languageMap; }
            set { languageMap = value; }
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
