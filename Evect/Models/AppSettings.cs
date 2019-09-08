#define LOCAL
namespace Evect.Models
{
    public static class AppSettings
    {
       /* #region Liza
        public static string Url { get; set; } = "https://bot.diffind.com:443/{0}";
        public static string Name { get; set; } = "evect_bot";
        //  public static string Name { get; set; } = "evect_bot";
        public static string Key { get; set; } = "913092744:AAGfI54AotPc_7P4l7abF-ZAuwji0b1Nfu0";
        public static string DatabaseConnectionString { get; set; } = "Server=localhost;Database=u0641156_bot;User Id=u0641156_bot;Password=ReportBot123!";//Лиза
        #endregion */
        
        
        #region Kim

#if  LOCAL
        public static string Url { get; set; } = "https://cb1ae38c.ngrok.io:443/{0}"; // Перед ":" указать URL бота

#else

        public static string Url { get; set; } = "https://evect.vrrk.ru:443/{0}"; // Перед ":" указать URL бота

#endif

          public static string Name { get; set; } = "evect_bot"; // Здесь имя бота (без @)

          public static string Key { get; set; } = "956625902:AAHF9mJj8SGo3_5g51kQZPUe3OX_cdAMM2o";

#if LOCAL  
          public static string DatabaseConnectionString { get; set; } =
             "Server=localhost\\SQLEXPRESS;Database=evect10;Trusted_Connection=True;";
#else
          public static string DatabaseConnectionString { get; set; } = 
              "Server=localhost;Database=u0707180_evect3;User Id=u0707180_evect3;Password=pVmbs2at";//Лиза

#endif
        #endregion

    }
}