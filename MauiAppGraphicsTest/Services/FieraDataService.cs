using MauiAppGraphicsTest.Models;
using System.Collections.ObjectModel;

namespace MauiAppGraphicsTest.Services
{
    public class FieraDataService
    {
        public static ObservableCollection<Fiera> GetFiereDiTest()
        {
            var fiere = new ObservableCollection<Fiera>();

            // FIERA 1: Tecnologia e Innovazione
            var fiera1 = new Fiera
            {
                Nome = "TechExpo Milano 2025",
                Cliente = "Expo Events S.r.l.",
                DataInizio = new DateTime(2025, 9, 15),
                DataFine = new DateTime(2025, 9, 18),
                Citta = "Milano",
                SettoreMerceologico = "Tecnologia e Innovazione",
                NumeroPartecipanti = 15000
            };

            // Padiglione 1A - AI e Machine Learning
            var padiglione1A = new Padiglione
            {
                Nome = "Padiglione AI & Future",
                Cliente = "TechCorp Solutions",
                Descrizione = "Soluzioni di Intelligenza Artificiale e Machine Learning",
                NumeroStand = 15,
                SuperficieTotale = 3200
            };

            // Stand AI-001
            var stand1A1 = new Stand
            {
                Codice = "AI-001",
                Nome = "Microsoft AI Zone",
                Cliente = "TechCorp Solutions",
                Superficie = 250,
                Tipologia = "Premium Plus",
                Costo = 18000
            };
            stand1A1.Espositori.Add(new Espositore
            {
                Nome = "Marco Rossi",
                Azienda = "Microsoft Italia",
                Settore = "AI Solutions",
                Telefono = "02-1234567",
                Email = "marco.rossi@microsoft.com"
            });
            stand1A1.Espositori.Add(new Espositore
            {
                Nome = "Sara Bianchi",
                Azienda = "Microsoft Italia",
                Settore = "Machine Learning",
                Telefono = "02-1234568",
                Email = "sara.bianchi@microsoft.com"
            });
            stand1A1.Espositori.Add(new Espositore
            {
                Nome = "Andrea Verdi",
                Azienda = "Microsoft Italia",
                Settore = "Computer Vision",
                Telefono = "02-1234569",
                Email = "andrea.verdi@microsoft.com"
            });

            // Stand AI-002
            var stand1A2 = new Stand
            {
                Codice = "AI-002",
                Nome = "Google AI Hub",
                Cliente = "TechCorp Solutions",
                Superficie = 200,
                Tipologia = "Premium",
                Costo = 15000
            };
            stand1A2.Espositori.Add(new Espositore
            {
                Nome = "Luca Verdi",
                Azienda = "Google Italy",
                Settore = "TensorFlow & Keras",
                Telefono = "02-2345678",
                Email = "luca.verdi@google.com"
            });
            stand1A2.Espositori.Add(new Espositore
            {
                Nome = "Chiara Neri",
                Azienda = "Google Italy",
                Settore = "Cloud AI Platform",
                Telefono = "02-2345679",
                Email = "chiara.neri@google.com"
            });

            // Stand AI-003
            var stand1A3 = new Stand
            {
                Codice = "AI-003",
                Nome = "OpenAI Innovation Corner",
                Cliente = "TechCorp Solutions",
                Superficie = 180,
                Tipologia = "Startup Premium",
                Costo = 12000
            };
            stand1A3.Espositori.Add(new Espositore
            {
                Nome = "Elena Neri",
                Azienda = "OpenAI Europe",
                Settore = "GPT & Language Models",
                Telefono = "02-3456789",
                Email = "elena.neri@openai.com"
            });

            padiglione1A.Stand.Add(stand1A1);
            padiglione1A.Stand.Add(stand1A2);
            padiglione1A.Stand.Add(stand1A3);

            // Padiglione 1B - Software Development
            var padiglione1B = new Padiglione
            {
                Nome = "Dev World & Frameworks",
                Cliente = "DevWorld Company",
                Descrizione = "Sviluppo Software, Framework e Strumenti di Sviluppo",
                NumeroStand = 12,
                SuperficieTotale = 2800
            };

            var stand1B1 = new Stand
            {
                Codice = "DEV-001",
                Nome = ".NET Ecosystem",
                Cliente = "DevWorld Company",
                Superficie = 280,
                Tipologia = "Flagship",
                Costo = 22000
            };
            stand1B1.Espositori.Add(new Espositore
            {
                Nome = "Antonio Gialli",
                Azienda = "Microsoft",
                Settore = ".NET MAUI & Blazor",
                Telefono = "02-4567890",
                Email = "antonio.gialli@microsoft.com"
            });
            stand1B1.Espositori.Add(new Espositore
            {
                Nome = "Francesca Blu",
                Azienda = "Microsoft",
                Settore = "ASP.NET Core",
                Telefono = "02-4567891",
                Email = "francesca.blu@microsoft.com"
            });

            var stand1B2 = new Stand
            {
                Codice = "DEV-002",
                Nome = "React & Modern Frontend",
                Cliente = "DevWorld Company",
                Superficie = 220,
                Tipologia = "Premium",
                Costo = 16000
            };
            stand1B2.Espositori.Add(new Espositore
            {
                Nome = "Roberto Viola",
                Azienda = "Meta",
                Settore = "React & React Native",
                Telefono = "02-5678901",
                Email = "roberto.viola@meta.com"
            });

            padiglione1B.Stand.Add(stand1B1);
            padiglione1B.Stand.Add(stand1B2);

            fiera1.Padiglioni.Add(padiglione1A);
            fiera1.Padiglioni.Add(padiglione1B);

            // FIERA 2: Food & Beverage
            var fiera2 = new Fiera
            {
                Nome = "Food Innovation Expo",
                Cliente = "Culinary Events Italia",
                DataInizio = new DateTime(2025, 10, 20),
                DataFine = new DateTime(2025, 10, 23),
                Citta = "Bologna",
                SettoreMerceologico = "Food & Beverage",
                NumeroPartecipanti = 32000
            };

            // Padiglione Bio & Organic
            var padiglione2A = new Padiglione
            {
                Nome = "Bio & Sustainable Food",
                Cliente = "Green Food Ltd",
                Descrizione = "Prodotti Biologici, Sostenibili e a Km Zero",
                NumeroStand = 18,
                SuperficieTotale = 4200
            };

            var stand2A1 = new Stand
            {
                Codice = "BIO-001",
                Nome = "Terra Verde Premium",
                Cliente = "Green Food Ltd",
                Superficie = 350,
                Tipologia = "Eco Premium",
                Costo = 25000
            };
            stand2A1.Espositori.Add(new Espositore
            {
                Nome = "Maria Verdi",
                Azienda = "Terra Verde Bio",
                Settore = "Ortaggi Biologici",
                Telefono = "051-1234567",
                Email = "maria.verdi@terraverde.it"
            });
            stand2A1.Espositori.Add(new Espositore
            {
                Nome = "Giuseppe Rossi",
                Azienda = "Terra Verde Bio",
                Settore = "Frutta di Stagione",
                Telefono = "051-1234568",
                Email = "giuseppe.rossi@terraverde.it"
            });
            stand2A1.Espositori.Add(new Espositore
            {
                Nome = "Carla Marroni",
                Azienda = "Terra Verde Bio",
                Settore = "Conserve Biologiche",
                Telefono = "051-1234569",
                Email = "carla.marroni@terraverde.it"
            });

            padiglione2A.Stand.Add(stand2A1);

            fiera2.Padiglioni.Add(padiglione2A);

            // FIERA 3: Automotive Electric Future
            var fiera3 = new Fiera
            {
                Nome = "Electric Mobility Show",
                Cliente = "Automotive Events Group",
                DataInizio = new DateTime(2025, 11, 10),
                DataFine = new DateTime(2025, 11, 15),
                Citta = "Torino",
                SettoreMerceologico = "Automotive & E-Mobility",
                NumeroPartecipanti = 45000
            };

            // Padiglione Electric Vehicles
            var padiglione3A = new Padiglione
            {
                Nome = "Electric Revolution",
                Cliente = "E-Mobility Solutions",
                Descrizione = "Veicoli Elettrici, Batterie e Infrastrutture di Ricarica",
                NumeroStand = 25,
                SuperficieTotale = 6800
            };

            var stand3A1 = new Stand
            {
                Codice = "ELE-001",
                Nome = "Tesla Superstation",
                Cliente = "E-Mobility Solutions",
                Superficie = 500,
                Tipologia = "Mega Premium",
                Costo = 45000
            };
            stand3A1.Espositori.Add(new Espositore
            {
                Nome = "Michael Green",
                Azienda = "Tesla Italy",
                Settore = "Model S & Model X",
                Telefono = "011-1234567",
                Email = "michael.green@tesla.com"
            });
            stand3A1.Espositori.Add(new Espositore
            {
                Nome = "Laura White",
                Azienda = "Tesla Italy",
                Settore = "Model 3 & Model Y",
                Telefono = "011-1234568",
                Email = "laura.white@tesla.com"
            });
            stand3A1.Espositori.Add(new Espositore
            {
                Nome = "Paolo Grigi",
                Azienda = "Tesla Italy",
                Settore = "Supercharger Network",
                Telefono = "011-1234569",
                Email = "paolo.grigi@tesla.com"
            });
            stand3A1.Espositori.Add(new Espositore
            {
                Nome = "Anna Tesla",
                Azienda = "Tesla Italy",
                Settore = "Energy Storage",
                Telefono = "011-1234570",
                Email = "anna.tesla@tesla.com"
            });

            padiglione3A.Stand.Add(stand3A1);
            fiera3.Padiglioni.Add(padiglione3A);

            fiere.Add(fiera1);
            fiere.Add(fiera2);
            fiere.Add(fiera3);

            return fiere;
        }

        // Metodo per aggiungere dinamicamente nuovi dati
        public static void AddSampleData(ObservableCollection<Fiera> fiere)
        {
            // Esempio di come aggiungere dati runtime mantenendo la reattività
            var nuovaFiera = new Fiera
            {
                Nome = "Design & Innovation Week",
                Cliente = "Creative Events",
                DataInizio = DateTime.Today.AddDays(30),
                DataFine = DateTime.Today.AddDays(33),
                Citta = "Firenze",
                SettoreMerceologico = "Design & Arte",
                NumeroPartecipanti = 12000
            };

            var padiglione = new Padiglione
            {
                Nome = "Digital Art Hub",
                Cliente = "Digital Creators",
                Descrizione = "Arte Digitale e NFT",
                NumeroStand = 8,
                SuperficieTotale = 1600
            };

            var stand = new Stand
            {
                Codice = "ART-001",
                Nome = "NFT Gallery",
                Cliente = "Digital Creators",
                Superficie = 200,
                Tipologia = "Digital Premium",
                Costo = 12000
            };

            stand.Espositori.Add(new Espositore
            {
                Nome = "Alice Digital",
                Azienda = "NFT Studios",
                Settore = "Crypto Art",
                Telefono = "055-1234567",
                Email = "alice@nftstudios.com"
            });

            padiglione.Stand.Add(stand);
            nuovaFiera.Padiglioni.Add(padiglione);
            fiere.Add(nuovaFiera);
        }
    }
}
