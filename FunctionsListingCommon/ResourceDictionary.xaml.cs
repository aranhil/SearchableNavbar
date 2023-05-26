using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SearchableNavbar
{
    /// <summary>
    /// Interaction logic for ResourceDictionary.xaml
    /// </summary>
    public partial class ResourceDictionary
    {
        public ResourceDictionary()
        {
        }
        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxItem listBoxItem = (sender as FrameworkElement)?.TemplatedParent as ListBoxItem;
            listBoxItem.IsSelected = true;
            listBoxItem.Focus();
        }
    }
}
