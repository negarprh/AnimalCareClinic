using System.Collections.Generic;


namespace AnimalCareClinic.ViewModels
{
    public class VetWorkloadItem
    {
        public int VeterinarianId { get; set; }
        public string VeterinarianName { get; set; } = string.Empty;
        public int VisitCount { get; set; }
    }

    public class MonthlyReportViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public int TotalAppointments { get; set; }
        public int BookedCount { get; set; }
        public int CancelledCount { get; set; }
        public int CompletedCount { get; set; }

        public List<VetWorkloadItem> VetWorkloads { get; set; } = new();
    }
}
