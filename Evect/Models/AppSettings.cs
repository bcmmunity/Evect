#define LOCAL
namespace Evect.Models
{
    public static class AppSettings
    {
         /*#region Liza 
         public static string Url { get; set; } = "https://5007b67d.ngrok.io/{0}";
         public static string Name { get; set; } = "evect_bot";
         //  public static string Name { get; set; } = "evect_bot";
         public static string Key { get; set; } = "913092744:AAGfI54AotPc_7P4l7abF-ZAuwji0b1Nfu0";
         public static string DatabaseConnectionString { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Database=eventbot2;Trusted_Connection=True;"
         ;
         //public static string DatabaseConnectionString { get; set; } = "Server=localhost;Database=u0641156_bot;User Id=u0641156_bot;Password=ReportBot123!";//Лиза
         #endregion */
    
        
        #region Kim

#if LOCAL
        public static string Url { get; set; } = "https://3a6991ab.ngrok.io:443/{0}"; // Перед ":" указать URL бота

#else

        public static string Url { get; set; } = "https://bot.diffind.com:443/{0}"; // Перед ":" указать URL бота

#endif

          public static string Name { get; set; } = "evect_bot"; // Здесь имя бота (без @)


#if LOCAL 
          public static string DatabaseConnectionString { get; set; } =
             "Server=localhost\\SQLEXPRESS;Database=evect2;Trusted_Connection=True;";
                  public static string Key { get; set; } = "956625902:AAHF9mJj8SGo3_5g51kQZPUe3OX_cdAMM2o";
//                  public static string Key { get; set; } = "822563453:AAG6JbtEMw9fo38ZIUceHjd480SghEBKC3c";

#else
          public static string DatabaseConnectionString { get; set; } = 
              "Server=localhost;Database=u0641156_evect;User Id=u0641156_evect;Password=Evect123!@#";//Лиза
          public static string Key { get; set; } = "822563453:AAG6JbtEMw9fo38ZIUceHjd480SghEBKC3c";


#endif
        #endregion 

    }
}