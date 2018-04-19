namespace IAR.DispatcherAPI.Models
{
    public class AppSettings
    {
        public string Domain { get; set; }
        public  string EmlOnErrorEmailFrom { get; set; }
        public  string EmlOnErrorEmailTo { get; set; }
        public  string SaveUnprocessedMessage { get; set; }
        public  string NofityEmlErrors { get; set; }
        public  string AttachmentsPath { get; set; }
        public  string ValidAttachmentTypes { get; set; }
        public  string MaxMessageSpan { get; set; }
        public  string IarApiUrl { get; set; }
        public string EmlOnUnmmatchedDispatchEmailTo { get; set; }
        public string UsaDomain { get; set; }
        public string CanadaDomain { get; set; }
    }
}
