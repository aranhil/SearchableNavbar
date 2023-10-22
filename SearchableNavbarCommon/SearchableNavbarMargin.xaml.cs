using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static Microsoft.VisualStudio.Threading.AsyncReaderWriterLock;
using static Microsoft.VisualStudio.VSConstants;
using static System.Windows.Forms.AxHost;
using Task = System.Threading.Tasks.Task;

namespace SearchableNavbar
{
    public class FunctionParam
    {
        public string Parameter { get; set; }
        public string Type { get; set; }
    }
    public class FunctionInfo : SelectableData
    {
        public FunctionInfo()
        {
        }

        public string Tag { get; set; }
        public string Signature { get; set; }
        public string Scope { get; set; }
        public string LineNo { get; set; }
        public ImageMoniker Moniker { get; set; }

        public BitmapSource imageSource;
        public BitmapSource ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                OnPropertyChanged();
            }
        }
    }
    public partial class SearchableNavbarMargin : UserControl, IWpfTextViewMargin
    {
        public const string MarginName = "SearchableNavbarMargin";

        private DTE2 DTE;
        ITextCaret Caret;
        public FrameworkElement DocumentElement = null;

        private bool isDisposed;
        private IWpfTextView TextView;
        private ITextBuffer TextBuffer;
        private IVsImageService2 ImageService;
        private ITextDocumentFactoryService TextDocumentFactoryService;
        private SearchableNavbarPackage Package;
        private string FilePath = "";
        private string FileType = "";

        FunctionInfo currentInfo = null;
        List<FunctionInfo> functionLines = new List<FunctionInfo>();
        ObservableCollection<FunctionInfo> filteredFunctionLines = new ObservableCollection<FunctionInfo>();

        private ManualResetEvent QueueEvent = new ManualResetEvent(false);
        CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        bool DocumentModified = false;

        public SearchableNavbarMargin(IWpfTextView textView, DTE2 DTE, IVsImageService2 ImageService, ITextDocumentFactoryService TextDocumentFactoryService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            InitializeComponent();

            this.DTE = DTE;
            this.TextView = textView;
            this.ImageService = ImageService;
            this.TextDocumentFactoryService = TextDocumentFactoryService;

            Caret = textView.Caret;
            TextBuffer = textView.TextBuffer;
            DocumentElement = textView.VisualElement;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextBuffer, out ITextDocument document))
            {
                FilePath = document.FilePath;
            }

            FunctionsListBox.ItemsSource = filteredFunctionLines;
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            DocumentModified = true;
        }

        async Task UpdateQueueAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                QueueEvent.WaitOne();
                QueueEvent.Reset();

                cancellationToken.ThrowIfCancellationRequested();

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                UpdateFunctions();
                await TaskScheduler.Default;

                await Task.Delay(1000, cancellationToken);
            }
        }

        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();

                // Since this is a horizontal margin, its width will be bound to the width of the text view.
                // Therefore, its size is its height.
                return ActualHeight;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();

                // The margin should always be enabled
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }

        private void UpdateFunctions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (Package == null)
                {
                    LoadPackage();
                    if (Package == null)
                    {
                        return;
                    }
                }

                EnvDTE.Document doc = DTE.ActiveDocument;
                string path = FilePath.Length > 0 ? FilePath : (doc?.FullName ?? "");

                if(ExcludeFileExtension(path, Package))
                {
                    return;
                }

                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    string tags = CTagsWrapper.Parse(path, Package);
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    bool PreviouslyEmpty = functionLines.Count == 0;

                    try
                    {
                        functionLines.Clear();

                        if (tags != null)
                        {
                            string[] lines = tags.Split('\n');
                            foreach (string line in lines)
                            {
                                string[] fields = line.Split('\t');

                                if (fields.Length == 5)
                                {
                                    if(Package.ShowTagSignature)
                                    {
                                        fields[2] = fields[2].Replace(",", ", ");
                                    }
                                    else
                                    {
                                        fields[2] = "";
                                    }

                                    fields[4] = fields[4].Replace("\r", "");

                                    FunctionInfo newFunctionInfo = new FunctionInfo()
                                    {
                                        Tag = fields[0],
                                        LineNo = fields[1],
                                        Signature = fields[2].Length == 1 && fields[2][0] == '-' ? "" : fields[2],
                                        Scope = "",
                                        Moniker = GetMonikerFromLetterAndLanguage(fields[3], fields[4])
                                    };

                                    FileType = fields[4];

                                    if(CanTagBeFullyQualified(functionLines, newFunctionInfo, fields[3], fields[4], out int index))
                                    {
                                        if (newFunctionInfo.Tag.Length > functionLines[index].Tag.Length)
                                        {
                                            newFunctionInfo.Scope = newFunctionInfo.Tag.Substring(0, newFunctionInfo.Tag.Length - functionLines[index].Tag.Length);
                                            newFunctionInfo.Tag = functionLines[index].Tag;
                                            functionLines[index] = newFunctionInfo;
                                        }
                                        else
                                        {
                                            functionLines[index].Scope = functionLines[index].Tag.Substring(0, functionLines[index].Tag.Length - newFunctionInfo.Tag.Length);
                                            functionLines[index].Tag = newFunctionInfo.Tag;
                                        }
                                    }
                                    else
                                    {
                                        functionLines.Add(newFunctionInfo);
                                    }
                                }
                            }
                        }
                    }
                    catch { }

                    InitializeContextMenu();
                    FilterFunctions();

                    if(PreviouslyEmpty && functionLines.Count > 0)
                    {
                        SearchableNavbarControl.Visibility = Visibility.Visible;

                        try
                        {
                            UpdateOverlayFromCurrentCaretPosition(TextView.Caret.Position.BufferPosition);
                            SelectSearchResult(currentInfo);
                        }
                        catch { }
                    }
                });
            }
            catch(Exception)
            {
            }
        }

        private bool CanTagBeFullyQualified(List<FunctionInfo> functionLines, FunctionInfo newFunctionInfo, string kind, string language, out int index)
        {
            index = -1;

            if (language == "C++")
            {
                if (kind[0] == 'l') return false;
                if (kind[0] == 'z') return false;
                if (kind[0] == 'D') return false;
                if (kind[0] == 'Z') return false;
            }

            index = functionLines.FindIndex(x => x.LineNo == newFunctionInfo.LineNo && (x.Tag.Contains(newFunctionInfo.Tag) || newFunctionInfo.Tag.Contains(x.Tag)));
            return index >= 0;
        }

        private ImageMoniker GetMonikerFromLetterAndLanguage(string kind, string language)
        {
            if(kind.Length == 0)
            {
                return KnownMonikers.Method;
            }

            if(language == "C++")
            {
                if (kind[0] == 'f') return KnownMonikers.Method;
                if (kind[0] == 'p') return KnownMonikers.MethodShortcut;
                if (kind[0] == 'c') return KnownMonikers.Class;
                if (kind[0] == 'n') return KnownMonikers.Namespace;
                if (kind[0] == 'd') return KnownMonikers.MacroPublic;
                if (kind[0] == 'g') return KnownMonikers.EnumerationPublic;
                if (kind[0] == 'e') return KnownMonikers.EnumerationItemPublic;
                if (kind[0] == 's') return KnownMonikers.Structure;
                if (kind[0] == 'l') return KnownMonikers.LocalVariable;
                if (kind[0] == 't') return KnownMonikers.TypeDefinition;
                if (kind[0] == 'z') return KnownMonikers.Parameter;
                if (kind[0] == 'D') return KnownMonikers.Parameter;
                if (kind[0] == 'u') return KnownMonikers.Union;
                if (kind[0] == 'x') return KnownMonikers.ExternalVariableValue;
                if (kind[0] == 'U') return KnownMonikers.NamespaceShortcut;
                if (kind[0] == 'Z') return KnownMonikers.Parameter;
                if (kind[0] == 'm') return KnownMonikers.Field;
                if (kind[0] == 'v') return KnownMonikers.Field;
            }

            return KnownMonikers.Method;
        }

        private void LoadPackage()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var vsShell = (IVsShell)ServiceProvider.GlobalProvider.GetService(typeof(IVsShell));
            if (vsShell != null)
            {
                IVsPackage package = null;
                if (vsShell.IsPackageLoaded(new Guid(SearchableNavbarPackage.PackageGuidString), out package) == S_OK)
                {
                    Package = (SearchableNavbarPackage)package;
                }
                else if (ErrorHandler.Succeeded(vsShell.LoadPackage(new Guid(SearchableNavbarPackage.PackageGuidString), out package)))
                {
                    Package = package as SearchableNavbarPackage;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Caret.PositionChanged += Caret_PositionChanged;
            TextBuffer.Changed += TextBuffer_Changed;

            if (TextDocumentFactoryService.TryGetTextDocument(TextView.TextBuffer, out ITextDocument document))
            {
                document.FileActionOccurred += Document_FileActionOccurred;
            }

            CancellationTokenSource = new CancellationTokenSource();
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await TaskScheduler.Default;
                await UpdateQueueAsync(CancellationTokenSource.Token);
            });
            
            RegisterToOptionsChangeEvents();

            QueueEvent.Set();
        }

        private void RegisterToOptionsChangeEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Package == null)
            {
                LoadPackage();
                if (Package == null)
                {
                    return;
                }
            }

            Package.RegisterToOptionsChangeEvents(OptionsChanged);
        }

        private void Document_FileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
            {
                if (DocumentModified)
                {
                    DocumentModified = false;
                    QueueEvent.Set();
                }
            }
        }

        private void OptionsChanged(object sender, EventArgs e)
        {
            QueueEvent.Set();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            QueueEvent.Set();
            CancellationTokenSource.Cancel();

            Caret.PositionChanged -= Caret_PositionChanged;
            TextBuffer.Changed -= TextBuffer_Changed;

            if (TextDocumentFactoryService.TryGetTextDocument(TextView.TextBuffer, out ITextDocument document))
            {
                document.FileActionOccurred -= Document_FileActionOccurred;
            }

            UnregisterFromOptionsChangeEvents();
        }

        private void UnregisterFromOptionsChangeEvents()
        {
            if (Package == null)
            {
                return;
            }

            Package.UnregisterFromOptionsChangeEvents(OptionsChanged);
        }

        private void FilterFunctions()
        {
            filteredFunctionLines.Clear();
            string[] words = SearchInput.Text.ToLowerInvariant().Split(' ');

            foreach (FunctionInfo functionInfo in functionLines)
            {
                bool allWordsMatch = true;
                string fullTextLower = (functionInfo.Scope + functionInfo.Tag + functionInfo.Signature).ToLowerInvariant();

                foreach (string word in words)
                {
                    if (!fullTextLower.Contains(word))
                    {
                        allWordsMatch = false;
                        break;
                    }
                }

                if (allWordsMatch)
                {
                    filteredFunctionLines.Add(functionInfo);
                }
            }

            if(filteredFunctionLines.Count > 0)
            {
                bool nothingSelected = true;
                foreach (FunctionInfo functionInfo in filteredFunctionLines)
                {
                    if (functionInfo.IsSelected)
                    {
                        nothingSelected = false;
                        break;
                    }
                }

                if (nothingSelected)
                {
                    SelectSearchResult(filteredFunctionLines[0]);
                }
            }
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterFunctions();
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                UpdateOverlayFromCurrentCaretPosition(e?.NewPosition.Point?.GetPoint(TextView.TextBuffer, e.NewPosition.Affinity));
            }
            catch { }
        }

        private void UpdateOverlayFromCurrentCaretPosition(SnapshotPoint? snapshotPoint)
        {
            if (snapshotPoint.HasValue)
            {
                ITextSnapshotLine line = snapshotPoint.Value.GetContainingLine();

                int lineNumber = line.LineNumber + 1;
                currentInfo = null;

                List<FunctionInfo> orderedFunctions = functionLines.OrderBy(x => int.Parse(x.LineNo)).ToList();
                if (orderedFunctions.Count == 1)
                {
                    currentInfo = orderedFunctions[0];
                }
                else
                {
                    for (int i = 1; i < orderedFunctions.Count; i++)
                    {
                        if (lineNumber < int.Parse(orderedFunctions[i].LineNo))
                        {
                            if (i > 0)
                            {
                                currentInfo = orderedFunctions[i - 1];
                            }
                            break;
                        }

                        if (i == orderedFunctions.Count - 1)
                        {
                            currentInfo = orderedFunctions[i];
                        }
                    }
                }

                if (currentInfo != null)
                {
                    Overlay.DataContext = currentInfo;
                }
            }
        }

        private void FunctionsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterFunctions();
            ItemsPopup.IsOpen = true;
        }

        private void ClosePopup()
        {
            SearchInput.Text = "";
            Overlay.Visibility = Visibility.Visible;
            ItemsPopup.IsOpen = false;
            SearchableNavbarControl.Visibility = functionLines.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        int selectedSearchResult = -1;

        private void GridContainer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FunctionInfo newSelectedSearchResult = null;
            FunctionInfo oldSelectedSearchResult = selectedSearchResult >= 0 && selectedSearchResult < filteredFunctionLines.Count ? filteredFunctionLines[selectedSearchResult] : null;

            if (e.Key == System.Windows.Input.Key.Up)
            {
                if (selectedSearchResult > 0 && selectedSearchResult < filteredFunctionLines.Count)
                {
                    newSelectedSearchResult = filteredFunctionLines[selectedSearchResult - 1];
                }
                else if (filteredFunctionLines.Count > 0)
                {
                    newSelectedSearchResult = filteredFunctionLines[0];
                }

                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                if (selectedSearchResult < filteredFunctionLines.Count - 1 && selectedSearchResult >= 0)
                {
                    newSelectedSearchResult = filteredFunctionLines[selectedSearchResult + 1];
                }
                else if (filteredFunctionLines.Count > 0 && selectedSearchResult < 0)
                {
                    newSelectedSearchResult = filteredFunctionLines[0];
                }

                e.Handled = true;

            }
            else if (e.Key == System.Windows.Input.Key.PageUp)
            {
                if (filteredFunctionLines.Count > 0)
                {
                    int newIndex = Math.Max(0, selectedSearchResult - (int)Math.Floor(FunctionsListBox.ActualHeight / 16.0f));
                    if (newIndex >= 0 && newIndex < filteredFunctionLines.Count)
                    {
                        newSelectedSearchResult = filteredFunctionLines[newIndex];
                    }
                }

                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.PageDown)
            {
                if (filteredFunctionLines.Count > 0)
                {
                    int newIndex = Math.Min(filteredFunctionLines.Count - 1, selectedSearchResult + (int)Math.Floor(FunctionsListBox.ActualHeight / 16.0f));
                    if (newIndex > 0 && newIndex < filteredFunctionLines.Count)
                    {
                        newSelectedSearchResult = filteredFunctionLines[newIndex];
                    }
                }

                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Home)
            {
                if (filteredFunctionLines.Count > 0)
                {
                    newSelectedSearchResult = filteredFunctionLines[0];
                }

                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.End)
            {
                if (filteredFunctionLines.Count > 0)
                {
                    newSelectedSearchResult = filteredFunctionLines[filteredFunctionLines.Count - 1];
                }

                e.Handled = true;
            }

            if (e.Handled)
            {
                if(oldSelectedSearchResult != null)
                {
                    oldSelectedSearchResult.IsSelected = false;
                }

                SelectSearchResult(newSelectedSearchResult);
            }

            if(e.Key == Key.Enter)
            {
                if(oldSelectedSearchResult != null)
                {
                    try
                    {
                        TextView.VisualElement.Focus();
                        (DTE?.ActiveDocument?.Selection as TextSelection)?.MoveToLineAndOffset(int.Parse(oldSelectedSearchResult.LineNo), 1);

                        e.Handled = true;
                    }
                    catch { }

                    ClosePopup();
                }
            }
            else if(e.Key == Key.Escape)
            {
                ClosePopup();

                TextView.VisualElement.Focus();
            }
        }

        private void SelectSearchResult(FunctionInfo newSelectedSearchResult)
        {
            if(newSelectedSearchResult != null)
            {
                newSelectedSearchResult.IsSelected = true;
                selectedSearchResult = filteredFunctionLines.IndexOf(newSelectedSearchResult);

                if (selectedSearchResult >= 0)
                {
                    Overlay.DataContext = newSelectedSearchResult;

                    VirtualizingStackPanel virtualizingStackPanel = GetChildOfType<VirtualizingStackPanel>(FunctionsListBox);
                    if (virtualizingStackPanel != null)
                    {
                        virtualizingStackPanel.BringIndexIntoViewPublic(selectedSearchResult);
                        ListBoxItem listBoxItem = FunctionsListBox.ItemContainerGenerator.ContainerFromItem(newSelectedSearchResult) as ListBoxItem;

                        listBoxItem?.BringIntoView();
                    }
                }
            }
        }

        public static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void SearchInput_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SearchInput.SelectAll();
            Overlay.Visibility = Visibility.Collapsed;
            ItemsPopup.IsOpen = true;
            IgnoreNextUnfocus = false;

            SelectSearchResult(currentInfo);
        }

        private void SearchInput_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(IgnoreNextUnfocus)
            {
                SearchInput.Focus();
                IgnoreNextUnfocus = false;
            }
            else
            {
                ClosePopup();
            }
        }

        private void FunctionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSearchResult = FunctionsListBox.SelectedIndex;
        }

        private bool IgnoreNextUnfocus = false;
        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IgnoreNextUnfocus = true;
        }

        private void SearchInput_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Right && !ItemsPopup.IsOpen)
            {
                e.Handled = true;
            }
            else
            {
                IgnoreNextUnfocus = false;
            }
        }

        private void SearchInput_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TextView.VisualElement.Focus();
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FunctionInfo functionInfo = (sender as DockPanel)?.DataContext as FunctionInfo;
            if (functionInfo != null)
            {
                try
                {
                    TextView.VisualElement.Focus();
                    (DTE?.ActiveDocument?.Selection as TextSelection)?.MoveToLineAndOffset(int.Parse(functionInfo.LineNo), 1);
                }
                catch { }

                ClosePopup();
            }
        }

        public void CommandReceived()
        {
            SearchableNavbarControl.Visibility = Visibility.Visible;

#pragma warning disable VSTHRD001
            _ = Dispatcher.BeginInvoke((Action)delegate
            {
                Keyboard.Focus(SearchInput);
            }, DispatcherPriority.Render);
#pragma warning restore VSTHRD001

            if (filteredFunctionLines.Count == 0)
            {
                QueueEvent.Set();
            }
        }

        private bool ExcludeFileExtension(string path, SearchableNavbarPackage package)
        {
            if(package == null || package.IgnoredFileExtensions.Length == 0)
            {
                return false;
            }

            string currentExtension = System.IO.Path.GetExtension(path).ToLower();
            return package.IgnoredFileExtensions.Split(',').Any((string extension) => { return currentExtension == extension; });
        }

        public class ContextMenuToggle
        {
            private Action<bool> Setter;
            private Func<bool> Getter;

            public ContextMenuToggle(Action<bool> setter, Func<bool> getter)
            {
                Setter = setter;
                Getter = getter;
            }

            private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    ContextMenuToggle contextMenuToggle = menuItem.Tag as ContextMenuToggle;
                    if(contextMenuToggle != null)
                    {
                        bool oldValue = contextMenuToggle.Getter();
                        contextMenuToggle.Setter(!oldValue);
                        menuItem.IsChecked = !oldValue;
                    }
                }
            }

            public static void AddMenuToggleOption(ContextMenu contextMenu, string header, Action<bool> setter, Func<bool> getter)
            {
                ContextMenuToggle contextMenuToggle = new ContextMenuToggle(setter, getter);

                MenuItem menuItem = new MenuItem { Header = header, IsCheckable = true };
                menuItem.Click += contextMenuToggle.ContextMenuItem_Click;
                menuItem.IsChecked = getter();
                menuItem.Tag = contextMenuToggle;

                contextMenu.Items.Add(menuItem);
            }
        }

        private void InitializeContextMenu()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            SearchInputContextMenu.Items.Clear();

            if (Package == null)
            {
                LoadPackage();
                if(Package == null)
                {
                    return;
                }
            }

            ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Sort Alphabetically", value => Package.SortAlphabetically = value, () => Package.SortAlphabetically);
            ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Fully Qualified Tags", value => Package.ShowFullyQualifiedTags = value, () => Package.ShowFullyQualifiedTags);
            ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Anonymous Tags", value => Package.ShowAnonymousTags = value, () => Package.ShowAnonymousTags);
            ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Tag Signature", value => Package.ShowTagSignature = value, () => Package.ShowTagSignature);

            if (FileType == "C++")
            {
                SearchInputContextMenu.Items.Add(new Separator());
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Macro Definitions", value => Package.CppShowMacroDefinitions = value, () => Package.CppShowMacroDefinitions);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Function Definitions", value => Package.CppShowFunctionDefinitions = value, () => Package.CppShowFunctionDefinitions);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Enumerators", value => Package.CppShowEnumerators = value, () => Package.CppShowEnumerators);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Enumeration Names", value => Package.CppShowEnumerationNames = value, () => Package.CppShowEnumerationNames);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Local Variables", value => Package.CppShowLocalVariables = value, () => Package.CppShowLocalVariables);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Class, Struct, and Union Members", value => Package.CppShowClassStructUnionMembers = value, () => Package.CppShowClassStructUnionMembers);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Function Prototypes", value => Package.CppShowFunctionPrototypes = value, () => Package.CppShowFunctionPrototypes);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Structure Names", value => Package.CppShowStructureNames = value, () => Package.CppShowStructureNames);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Typedefs", value => Package.CppShowTypedefs = value, () => Package.CppShowTypedefs);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Union Names", value => Package.CppShowUnionNames = value, () => Package.CppShowUnionNames);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Variable Definitions", value => Package.CppShowVariableDefinitions = value, () => Package.CppShowVariableDefinitions);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show External And Forward Variable Declarations", value => Package.CppShowExternalAndForwardVariableDeclarations = value, () => Package.CppShowExternalAndForwardVariableDeclarations);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Function Parameters", value => Package.CppShowFunctionParameters = value, () => Package.CppShowFunctionParameters);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Goto Labels", value => Package.CppShowGotoLabels = value, () => Package.CppShowGotoLabels);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Macro Parameters", value => Package.CppShowMacroParameters = value, () => Package.CppShowMacroParameters);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Classes", value => Package.CppShowClasses = value, () => Package.CppShowClasses);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Namespaces", value => Package.CppShowNamespaces = value, () => Package.CppShowNamespaces);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Using Namespace Statements", value => Package.CppShowUsingNamespaceStatements = value, () => Package.CppShowUsingNamespaceStatements);
                ContextMenuToggle.AddMenuToggleOption(SearchInputContextMenu, "Show Template Parameters", value => Package.CppShowTemplateParameters = value, () => Package.CppShowTemplateParameters);
            }
        }
    }
}
