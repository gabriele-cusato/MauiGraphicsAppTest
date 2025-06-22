using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppGraphicsTest.Interfaces
{
    public interface IHierarchicalItem
    {
        string DisplayName { get; }
        string DisplaySubtitle { get; }
        string DisplayIcon { get; }
        Color BackgroundColor { get; }
        bool IsExpanded { get; set; }
        IEnumerable GetChildren();
        bool HasChildren { get; }
        Dictionary<string, object> GetDisplayProperties();
    }
}
