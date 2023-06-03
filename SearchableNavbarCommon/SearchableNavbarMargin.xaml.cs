using EnvDTE;
using EnvDTE80;
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
using System.Diagnostics;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Microsoft.VisualStudio.Threading.AsyncReaderWriterLock;
using static System.Windows.Forms.AxHost;

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
        public string Scope { get; set; }
        public string LineNo { get; set; }

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
        public const string MarginName = "FunctionListingsMargin";

        private DTE2 DTE;
        ITextCaret Caret;
        public FrameworkElement DocumentElement = null;

        private bool isDisposed;
        private IWpfTextView textView;
        private ITextBuffer textBuffer;
        private IVsImageService2 ImageService;
        private ITextDocumentFactoryService TextDocumentFactoryService;

        FunctionInfo currentInfo = null;
        List<FunctionInfo> functionLines = new List<FunctionInfo>();
        ObservableCollection<FunctionInfo> filteredFunctionLines = new ObservableCollection<FunctionInfo>();

        private ManualResetEvent QueueEvent = new ManualResetEvent(false);
        CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        bool DocumentModified = false;

        public SearchableNavbarMargin(IWpfTextView textView, DTE2 DTE, IVsImageService2 ImageService, ITextDocumentFactoryService TextDocumentFactoryService)
        {
            InitializeComponent();

            this.DTE = DTE;
            this.textView = textView;
            this.ImageService = ImageService;
            this.TextDocumentFactoryService = TextDocumentFactoryService;

            Caret = textView.Caret;
            textBuffer = textView.TextBuffer;
            DocumentElement = textView.VisualElement;

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

        private void FilteredFunctionLines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SearchableNavbarControl.Visibility = filteredFunctionLines.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                this.ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                this.ThrowIfDisposed();

                // Since this is a horizontal margin, its width will be bound to the width of the text view.
                // Therefore, its size is its height.
                return this.ActualHeight;
            }
        }

        public bool Enabled
        {
            get
            {
                this.ThrowIfDisposed();

                // The margin should always be enabled
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, SearchableNavbarMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                GC.SuppressFinalize(this);
                this.isDisposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }

        private void UpdateFunctions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {

                EnvDTE.Document doc = DTE.ActiveDocument;
                string path = doc?.FullName ?? "";

                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    string tags = CTagsWrapper.Parse(path);
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    try
                    {
                        functionLines.Clear();

                        if (tags != null)
                        {
                            string[] lines = tags.Split('\n');
                            foreach (string line in lines)
                            {
                                // Split each line into fields
                                string[] fields = line.Split('\t');

                                if (fields.Length == 2)
                                {
                                    FunctionInfo newFunctionInfo = new FunctionInfo()
                                    {
                                        Tag = fields[0],
                                        LineNo = fields[1],
                                        Scope = ""
                                    };

                                    int index = functionLines.FindIndex(x => x.LineNo == newFunctionInfo.LineNo);
                                    if (index >= 0)
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

                    FilterFunctions();
                });
            }
            catch(Exception)
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Caret.PositionChanged += Caret_PositionChanged;
            textBuffer.Changed += TextBuffer_Changed;
            filteredFunctionLines.CollectionChanged += FilteredFunctionLines_CollectionChanged;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextBuffer, out ITextDocument document))
            {
                document.FileActionOccurred += Document_FileActionOccurred;
            }

            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await TaskScheduler.Default;
                await UpdateQueueAsync(CancellationTokenSource.Token);
            });

            QueueEvent.Set();
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            QueueEvent.Set();
            CancellationTokenSource.Cancel();

            Caret.PositionChanged -= Caret_PositionChanged;
            textBuffer.Changed -= TextBuffer_Changed;
            filteredFunctionLines.CollectionChanged -= FilteredFunctionLines_CollectionChanged;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextBuffer, out ITextDocument document))
            {
                document.FileActionOccurred -= Document_FileActionOccurred;
            }
        }

        private void FilterFunctions()
        {
            filteredFunctionLines.Clear();
            string[] words = SearchInput.Text.ToLowerInvariant().Split(' ');

            foreach (FunctionInfo functionInfo in functionLines)
            {
                bool allWordsMatch = true;
                string fullTextLower = (functionInfo.Scope + functionInfo.Tag).ToLowerInvariant();

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
                SnapshotPoint? point = e.NewPosition.Point.GetPoint(textView.TextBuffer, e.NewPosition.Affinity);

                if (point.HasValue)
                {
                    ITextSnapshotLine line = point.Value.GetContainingLine();

                    int lineNumber = line.LineNumber + 1;
                    currentInfo = null;

                    List<FunctionInfo> orderedFunctions = functionLines.OrderBy(x => int.Parse(x.LineNo)).ToList();
                    if(orderedFunctions.Count == 1)
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

                            if(i == orderedFunctions.Count - 1)
                            {
                                currentInfo = orderedFunctions[i];
                            }
                        }
                    }

                    if(currentInfo != null)
                    {
                        Overlay.DataContext = currentInfo;
                    }
                }
            }
            catch { }
        }


        private void FunctionsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterFunctions();
            ItemsPopup.IsOpen = true;
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

                if(newSelectedSearchResult != null)
                {
                    SelectSearchResult(newSelectedSearchResult);
                }
            }

            if(e.Key == Key.Enter)
            {
                if(oldSelectedSearchResult != null)
                {
                    try
                    {
                        textView.VisualElement.Focus();
                        (DTE?.ActiveDocument?.Selection as TextSelection)?.MoveToLineAndOffset(int.Parse(oldSelectedSearchResult.LineNo), 1);

                        e.Handled = true;
                    }
                    catch { }

                    SearchInput.Text = "";
                    Overlay.Visibility = Visibility.Visible;
                    ItemsPopup.IsOpen = false;
                }
            }
            else if(e.Key == Key.Escape)
            {
                SearchInput.Text = "";
                Overlay.Visibility = Visibility.Visible;
                ItemsPopup.IsOpen = false;

                textView.VisualElement.Focus();
            }
        }

        private void SelectSearchResult(FunctionInfo newSelectedSearchResult)
        {
            newSelectedSearchResult.IsSelected = true;
            selectedSearchResult = filteredFunctionLines.IndexOf(newSelectedSearchResult);

            if(selectedSearchResult >= 0)
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

            if (currentInfo != null)
            {
                SelectSearchResult(currentInfo);
            }
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
                Overlay.Visibility = Visibility.Visible;
                SearchInput.Text = "";
                ItemsPopup.IsOpen = false;
            }
        }

        private void GridContainer_PreviewKeyUp(object sender, KeyEventArgs e)
        {

        }

        private void FunctionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSearchResult = FunctionsListBox.SelectedIndex;
        }

        public void FunctionClicked(ListBoxItem listBoxItem)
        {
        }

        private bool IgnoreNextUnfocus = false;
        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IgnoreNextUnfocus = true;
        }

        private void SearchInput_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IgnoreNextUnfocus = false;
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FunctionInfo functionInfo = (sender as DockPanel)?.DataContext as FunctionInfo;
            if (functionInfo != null)
            {
                try
                {
                    textView.VisualElement.Focus();
                    (DTE?.ActiveDocument?.Selection as TextSelection)?.MoveToLineAndOffset(int.Parse(functionInfo.LineNo), 1);
                }
                catch { }

                SearchInput.Text = "";
                Overlay.Visibility = Visibility.Visible;
                ItemsPopup.IsOpen = false;
            }
        }
    }
}
