namespace football_management_system_cscb.Models.Formation
{
    public class Formation
    {
        public string Name { get; set; } = "";

        public int Goalkeepers { get; set; } = 1;
        public int Defenders { get; set; }
        public int Midfielders { get; set; }
        public int Attackers { get; set; }

        // tactical identity
        public double AttackBias { get; set; } = 0;
        public double MidfieldBias { get; set; } = 0;
        public double DefenseBias { get; set; } = 0;

        // ✅ REQUIRED FOR DB-DRIVEN CREATION
        public Formation() { }

        public Formation(string name)
        {
            var f = FormationLibrary.Get(name);

            Name = f.Name;
            Goalkeepers = f.Goalkeepers;
            Defenders = f.Defenders;
            Midfielders = f.Midfielders;
            Attackers = f.Attackers;

            AttackBias = f.AttackBias;
            MidfieldBias = f.MidfieldBias;
            DefenseBias = f.DefenseBias;
        }
    }
}