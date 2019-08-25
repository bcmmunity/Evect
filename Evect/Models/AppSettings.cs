namespace Evect.Models
{
    public static class AppSettings
    {
        /*#region Liza
        public static string Url { get; set; } = "https://bot.diffind.com:443/{0}";
        public static string Name { get; set; } = "evect_bot";
        //  public static string Name { get; set; } = "evect_bot";
        public static string Key { get; set; } = "913092744:AAGfI54AotPc_7P4l7abF-ZAuwji0b1Nfu0";
        public static string DatabaseConnectionString { get; set; } = "Server=localhost;Database=u0641156_bot;User Id=u0641156_bot;Password=ReportBot123!";//Лиза
        #endregion */
        
        
        #region Kim
          public static string Url { get; set; } = "https://d6f90e23.ngrok.io:443/{0}"; // Перед ":" указать URL бота
  //        public static string Url { get; set; } = "https://evect.vrrk.ru:443/{0}"; // Перед ":" указать URL бота

          public static string Name { get; set; } = "evect_bot"; // Здесь имя бота (без @)

          public static string Key { get; set; } = "956625902:AAHF9mJj8SGo3_5g51kQZPUe3OX_cdAMM2o";

          
          public static string DatabaseConnectionString { get; set; } =
             "Server=localhost\\SQLEXPRESS;Database=evect19;Trusted_Connection=True;";
        #endregion

    }
}