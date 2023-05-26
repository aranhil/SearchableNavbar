using System;
using System.ComponentModel.Composition;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace FunctionsListing
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(ListingsMargin.MarginName)]
    [Order(After = PredefinedMarginNames.HorizontalScrollBar)]  // Ensure that the margin occurs below the horizontal scrollbar
    [MarginContainer(PredefinedMarginNames.Top)]             // Set the container to the bottom of the editor window
    [ContentType("C/C++")]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class FunctionsMarginFactory : IWpfTextViewMarginProvider
    {
        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider;

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            return new ListingsMargin(wpfTextViewHost.TextView, ServiceProvider.GetService(typeof(DTE)) as DTE2);
        }
    }
}
