using System;
using System.ComponentModel.Composition;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace SearchableNavbar
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(SearchableNavbarMargin.MarginName)]
    [Order(After = PredefinedMarginNames.HorizontalScrollBar)]  // Ensure that the margin occurs below the horizontal scrollbar
    [MarginContainer(PredefinedMarginNames.Top)]             // Set the container to the bottom of the editor window
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class SearchableNavbarMarginFactory : IWpfTextViewMarginProvider
    {
#pragma warning disable CS0649
        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider;
        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService;
#pragma warning restore CS0649

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new SearchableNavbarMargin(wpfTextViewHost.TextView, 
                ServiceProvider.GetService(typeof(DTE)) as DTE2, 
                ServiceProvider.GetService(typeof(SVsImageService)) as IVsImageService2,
                TextDocumentFactoryService);
        }
    }
}
