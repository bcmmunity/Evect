namespace Evect.Models
{
    public static class AppSettings
    {
        public static string Url { get; set; } = "https://6920a63c.ngrok.io:443/{0}"; // Перед ":" указать URL бота
//        public static string Url { get; set; } = "https://evect.vrrk.ru:443/{0}"; // Перед ":" указать URL бота

        public static string Name { get; set; } = "evect_bot"; // Здесь имя бота (без @)

//        public static string Key { get; set; } = "913092744:AAGfI54AotPc_7P4l7abF-ZAuwji0b1Nfu0";
        public static string Key { get; set; } = "956625902:AAHF9mJj8SGo3_5g51kQZPUe3OX_cdAMM2o";

//        public static string DatabaseConnectionString { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Database=eventbot;Trusted_Connection=True;MultipleActiveResultSets=true";//Лиза
       public static string DatabaseConnectionString { get; set; } =
           "Server=localhost\\SQLEXPRESS;Database=evect6;Trusted_Connection=True;";

        
//        public static string DatabaseConnectionString { get; set; } =
//            "Server=localhost;Database=u0707180_evect;User Id=u0707180_evect;Password=pVmbs2at;";
    }
}