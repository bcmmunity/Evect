namespace Evect.Models
{
    public enum Actions
    {
        None,
        
        WaitingForEventCode,
        WaitingForValidationCode,
        AllEventsChangePage,
        
        DeleteOrNot,
        
        Profile,

        #region Networking mode
        
        FirstQuestion,
        SecondQuestion,
        ThirdQuestion,
        
        AddingParentTag,
        ChoosingTags,
        
        SearchingParentTag,
        SearchingTags,
        
        NetworkingMenu,
        
        MyProfile,
        ContactBook,
        Networking,
        
        #endregion

        #region private info
        WaitingForName,
        WainingForEmail,
        #endregion
        
        #region AdminActions
        AdminMode,
        GetInformationAboutTheEvent,
        AddNewInformationAboutEvent,
        EditInformationAboutEvent,
        CreateNotification,
        InformationAboutUsers,
        CreateSurvey,
        SurveyWithMessage,
        SurveyWithMarks,
        QuestionForSurveyWithMessage,
        QuestionForSurveyWithMarks,
        AnswerToSurvey

        #endregion

    }
}