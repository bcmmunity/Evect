namespace Evect.Models
{
    public enum Actions
    {
        None,
        
        WaitingForEventCode,
        WaitingForValidationCode,
        
        DeleteOrNot,
        
        Profile, 
        
        FirstQuestion,
        
        SecondQuestion,
        
        ThirdQuestion,
        
        AddingParentTag,
        
        ChoosingTags,
        
        Networking,

        #region private info
        WaitingForName,
        WainingForEmail,
        #endregion
        
        #region AdminActions
        AdminMode,
        GetInformationAboutTheEvent,
        CreateNotification,
        #endregion

    }
}