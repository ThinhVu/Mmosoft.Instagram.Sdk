namespace InstagramUser.Models
{
    public class ReportResult
    {        
        public string status { get; set; }
    }

    public enum ReportReasonId
    {
        SpamOrScram = 1,
        SelfHarm = 2,
        DrugUse = 3,
        NudityOrPornography = 4,
        GraphicViolence = 5,
        HateSpeechOrSymbol = 6,
        HarassmentOrBullying = 7
    }
}
