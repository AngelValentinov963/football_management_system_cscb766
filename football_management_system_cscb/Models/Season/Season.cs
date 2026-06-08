namespace football_management_system_cscb.Models.Season
{
    public class Season
    {
        public int Id { get; set; }

        public int CurrentWeek { get; set; } = 1;


        public List<Fixture> Fixtures { get; set; } = new();
    }
}