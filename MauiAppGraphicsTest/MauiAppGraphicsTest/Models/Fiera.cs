using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppGraphicsTest.Models
{
    public partial class Fiera : ExpandableItem
    {
        [ObservableProperty]
        private string nome = string.Empty;

        [ObservableProperty]
        private string cliente = string.Empty;

        [ObservableProperty]
        private DateTime dataInizio = DateTime.Today;

        [ObservableProperty]
        private DateTime dataFine = DateTime.Today.AddDays(3);

        [ObservableProperty]
        private string citta = string.Empty;

        [ObservableProperty]
        private string settoreMerceologico = string.Empty;

        [ObservableProperty]
        private int numeroPartecipanti = 0;

        public ObservableCollection<Padiglione> Padiglioni { get; set; } = new();

        public override string DisplayName => Nome;
        public override string DisplaySubtitle => $"{Citta} - {SettoreMerceologico}";
        public override string DisplayIcon => "🎪";
        public override Color BackgroundColor => Colors.DodgerBlue;
        public override bool HasChildren => Padiglioni.Any();

        public override IEnumerable GetChildren()
        {
            return Padiglioni;
        }

        public override Dictionary<string, object> GetDisplayProperties()
        {
            return new Dictionary<string, object>
            {
                { "Cliente", Cliente },
                { "Città", Citta },
                { "Settore", SettoreMerceologico },
                { "Data Inizio", DataInizio.ToString("dd/MM/yyyy") },
                { "Data Fine", DataFine.ToString("dd/MM/yyyy") },
                { "Partecipanti", $"{NumeroPartecipanti:N0}" },
                { "Padiglioni", $"{Padiglioni.Count} padiglioni" },
                { "Stand Totali", Padiglioni.SelectMany(p => p.Stand).Count().ToString() }
            };
        }
    }
}
