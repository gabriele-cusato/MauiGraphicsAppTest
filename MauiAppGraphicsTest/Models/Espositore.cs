using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using MauiAppGraphicsTest.Interfaces;

namespace MauiAppGraphicsTest.Models
{
    public partial class Espositore : ExpandableItem
    {
        [ObservableProperty]
        private string nome = string.Empty;

        [ObservableProperty]
        private string azienda = string.Empty;

        [ObservableProperty]
        private string settore = string.Empty;

        [ObservableProperty]
        private string telefono = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        public override string DisplayName => Nome;
        public override string DisplaySubtitle => Azienda;
        public override string DisplayIcon => "👤";
        public override Color BackgroundColor => Colors.MediumPurple;
        public override bool HasChildren => false;

        public override IEnumerable GetChildren()
        {
            return new List<object>(); // Espositori non hanno figli
        }

        public override Dictionary<string, object> GetDisplayProperties()
        {
            return new Dictionary<string, object>
            {
                { "Azienda", Azienda },
                { "Settore", Settore },
                { "Telefono", Telefono },
                { "Email", Email }
            };
        }
    }
}
