namespace football_management_system_cscb.Models.Match
{
    public class TeamMatchStrength
    {
        public double Attack { get; set; }
        public double Midfield { get; set; }
        public double Defense { get; set; }
        public double Goalkeeper { get; set; }

        public double Overall =>
            (Attack * 0.35) +
            (Midfield * 0.30) +
            (Defense * 0.25) +
            (Goalkeeper * 0.10);
    }
}
