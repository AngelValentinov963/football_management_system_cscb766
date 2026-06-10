namespace football_management_system_cscb.Models.Formation
{
    public static class FormationLibrary
    {
        public static Formation Get(string name)
        {
            return name switch
            {
                "4-3-3" => new Formation
                {
                    Name = "4-3-3",
                    Goalkeepers = 1,
                    Defenders = 4,
                    Midfielders = 3,
                    Attackers = 3,
                    AttackBias = 0.10,
                    MidfieldBias = 0.05,
                    DefenseBias = 0.00
                },

                "4-4-2" => new Formation
                {
                    Name = "4-4-2",
                    Goalkeepers = 1,
                    Defenders = 4,
                    Midfielders = 4,
                    Attackers = 2,
                    AttackBias = 0.00,
                    MidfieldBias = 0.00,
                    DefenseBias = 0.05
                },

                "3-5-2" => new Formation
                {
                    Name = "3-5-2",
                    Goalkeepers = 1,
                    Defenders = 3,
                    Midfielders = 5,
                    Attackers = 2,
                    AttackBias = 0.05,
                    MidfieldBias = 0.10,
                    DefenseBias = -0.05
                },
                "4-2-3-1" => new Formation
                {
                    Name = "4-2-3-1",
                    Goalkeepers = 1,
                    Defenders = 4,
                    Midfielders = 5,
                    Attackers = 1,
                    AttackBias = 0.05,
                    MidfieldBias = 0.10,
                    DefenseBias = 0.05
                },

                "5-3-2" => new Formation
                {
                    Name = "5-3-2",
                    Goalkeepers = 1,
                    Defenders = 5,
                    Midfielders = 3,
                    Attackers = 2,
                    AttackBias = -0.05,
                    MidfieldBias = 0.00,
                    DefenseBias = 0.15
                },

                _ => new Formation
                {
                    Name = "4-3-3",
                    Goalkeepers = 1,
                    Defenders = 4,
                    Midfielders = 3,
                    Attackers = 3
                },

                
            };
        }
    }
}