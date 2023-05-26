using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace FunctionsListing
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(FunctionsListingPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class FunctionsListingPackage : AsyncPackage
    {
        public const string PackageGuidString = "5ff8b546-f79f-471f-805a-c89eabc400b2";

        public IVsEditorAdaptersFactoryService Editor;
        public IVsTextManager TextManager;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            TextManager = (IVsTextManager)GetService(typeof(SVsTextManager));

            IComponentModel componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            if(componentModel != null)
            Editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            await FocusCommand.InitializeAsync(this);
        }
    }
}
