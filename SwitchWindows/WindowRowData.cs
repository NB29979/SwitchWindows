using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchWindows
{
    public class WindowRowData
    {
        public string title;
        public string icon;
        public WindowRowData(string _title, string _icon)
        {
            this.title = _title;
            this.icon = _icon;
        }
    }
    public class WindowRowDataComparer : IEqualityComparer<WindowRowData>
    {
        public bool Equals(WindowRowData _x, WindowRowData _y)
        {
            if (Object.ReferenceEquals(_x, _y)) return true;
            return _x != null && _y != null && _x.title.Equals(_y.title);
        }
        public int GetHashCode(WindowRowData _obj)
        {
            return _obj.title.GetHashCode();
        }
    }
}
