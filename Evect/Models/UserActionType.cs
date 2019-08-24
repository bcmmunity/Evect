namespace Evect.Models
{
    public enum Actions
    {
        None,
        
        WaitingForEventCode,
        
        DeleteOrNot,
        
        Profile,
        
        
        #region private info
        WaitingForName,
        WainingForEmail,
        #endregion
        
        #region AdminActions
        AdminMode,
        GetInformationAboutTheEvent,


        #endregion

    }
}