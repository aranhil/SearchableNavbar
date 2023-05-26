using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FunctionsListing
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

        public string FullText { get; set; }
        public string LineNo { get; set; }
    }
    public partial class ListingsMargin : UserControl, IWpfTextViewMargin
    {
        public const string MarginName = "FunctionListingsMargin";

        private DTE2 DTE;
        ITextCaret Caret;
        CodeModelEvents CodeModelEvents;
        public FrameworkElement DocumentElement = null;

        private bool IgnoreNextMouseOver = false;
        private bool IgnoreTextChange = false;

        private bool isDisposed;
        private IWpfTextView textView;

        List<FunctionInfo> functionLines = new List<FunctionInfo>();
        ObservableCollection<FunctionInfo> filteredFunctionLines = new ObservableCollection<FunctionInfo>();

        public ListingsMargin(IWpfTextView textView, DTE2 DTE)
        {
            InitializeComponent();

            this.DTE = DTE;
            this.DocumentElement = textView.VisualElement;
            this.Caret = textView.Caret;

            Caret.PositionChanged += Caret_PositionChanged;
            FunctionsListBox.ItemsSource = filteredFunctionLines;
            this.textView = textView;
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
            return string.Equals(marginName, ListingsMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                GC.SuppressFinalize(this);
                this.isDisposed = true;

                CodeModelEvents.ElementChanged += CodeModelElementChanged;
                CodeModelEvents.ElementAdded += CodeModelElementAdded;
                CodeModelEvents.ElementDeleted += CodeModelElementDeleted;

                Caret.PositionChanged -= Caret_PositionChanged;
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
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                EnvDTE.Document doc = DTE.ActiveDocument;
                string path = doc?.FullName ?? "";

                System.Threading.Tasks.Task.Run(() =>
                {
                    string tags = CTagsWrapper.Test(path);

                    Dispatcher.Invoke(() =>
                    {
                        string[] lines = tags.Split('\n');
                        foreach (string line in lines)
                        {
                            // Split each line into fields
                            string[] fields = line.Split('\t');

                            if(fields.Length >= 4)
                            {
                                functionLines.Add(new FunctionInfo() 
                                { 
                                    FullText = (fields[3].Length > 0 ? fields[3] + "::" : "") + fields[0] + fields[2],
                                    LineNo = fields[1]
                                });
                            }
                        }

                        FilterFunctions();
                    });
                });
            }
            catch(Exception)
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Events2 events = (Events2)DTE.Events;
            CodeModelEvents = events.get_CodeModelEvents(null);

            CodeModelEvents.ElementChanged += CodeModelElementChanged;
            CodeModelEvents.ElementAdded += CodeModelElementAdded;
            CodeModelEvents.ElementDeleted += CodeModelElementDeleted;

            ThreadHelper.ThrowIfNotOnUIThread();

            UpdateFunctions();
        }

        private void FilterFunctions()
        {
            filteredFunctionLines.Clear();
            string[] words = SearchInput.Text.ToLowerInvariant().Split(' ');

            foreach (FunctionInfo functionInfo in functionLines)
            {
                bool allWordsMatch = true;
                string fullTextLower = functionInfo.FullText.ToLowerInvariant();

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

        private void CodeModelElementDeleted(object Parent, CodeElement Element)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                UpdateFunctions();
            }));
        }

        private void CodeModelElementAdded(CodeElement Element)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                UpdateFunctions();
            }));
        }

        private void CodeModelElementChanged(CodeElement Element, vsCMChangeKind Change)
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                UpdateFunctions();
            }));
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
        }


        private void FunctionsList_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterFunctions();
            ItemsPopup.IsOpen = true;
        }

        int selectedSearchResult = -1;

        private void GridContainer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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
                    ItemsPopup.IsOpen = false;

                    try
                    {
                        ThreadHelper.ThrowIfNotOnUIThread();
                        textView.VisualElement.Focus();
                        (DTE?.ActiveDocument?.Selection as TextSelection)?.MoveToLineAndOffset(int.Parse(oldSelectedSearchResult.LineNo), 1);

                        e.Handled = true;
                    }
                    catch { }

                    SearchInput.Text = "";
                }
            }
        }

        private void SelectSearchResult(FunctionInfo newSelectedSearchResult)
        {
            newSelectedSearchResult.IsSelected = true;
            selectedSearchResult = filteredFunctionLines.IndexOf(newSelectedSearchResult);

            VirtualizingStackPanel virtualizingStackPanel = GetChildOfType<VirtualizingStackPanel>(FunctionsListBox);
            if(virtualizingStackPanel != null)
            {
                virtualizingStackPanel.BringIndexIntoViewPublic(selectedSearchResult);
                ListBoxItem listBoxItem = FunctionsListBox.ItemContainerGenerator.ContainerFromItem(newSelectedSearchResult) as ListBoxItem;

                listBoxItem?.BringIntoView();
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
            ItemsPopup.IsOpen = true;
        }

        private void SearchInput_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ItemsPopup.IsOpen = false;
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

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FunctionInfo functionInfo = (sender as DockPanel)?.DataContext as FunctionInfo;
            if (functionInfo != null)
            {
                ItemsPopup.IsOpen = false;

                try
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    textView.VisualElement.Focus();
                    (DTE?.ActiveDocument?.Selection as TextSelection)?.MoveToLineAndOffset(int.Parse(functionInfo.LineNo), 1);
                }
                catch { }

                SearchInput.Text = "";
            }
        }
    }
}
