namespace Dr.meow.Models
{
    // Models/EmbedReportData.cs

    /// <summary>
    /// 承載 Power BI 報表嵌入所需的關鍵資訊。
    /// </summary>

    public class EmbedReportData
    {
        public string ReportId { get; set; } = string.Empty;
        public string EmbedUrl { get; set; } = string.Empty;
        public string EmbedToken { get; set; } = string.Empty;
    }
}