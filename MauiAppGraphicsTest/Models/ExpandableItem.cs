using CommunityToolkit.Mvvm.ComponentModel;
using MauiAppGraphicsTest.Interfaces;
using System.Collections;

namespace MauiAppGraphicsTest.Models
{
    public abstract partial class ExpandableItem : ObservableObject, IHierarchicalItem
    {
        [ObservableProperty]
        private bool isExpanded = false;  // TUTTE CHIUSE ALL'INIZIO

        public abstract string DisplayName { get; }
        public abstract string DisplaySubtitle { get; }
        public abstract string DisplayIcon { get; }
        public abstract Color BackgroundColor { get; }
        public abstract IEnumerable GetChildren();
        public abstract bool HasChildren { get; }
        public abstract Dictionary<string, object> GetDisplayProperties();
    }
}