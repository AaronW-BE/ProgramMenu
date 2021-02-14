using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenu
{
    interface IListViewItem
    {
        object DataSource { get; set; }

        event EventHandler SelectedItemEvent;

        void SetSelected(bool selected);
    }
}
