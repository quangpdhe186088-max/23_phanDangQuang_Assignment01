using System.ComponentModel.DataAnnotations;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Models;

public sealed class NewsReportViewModel
{
    [Display(Name = "Ngày bắt đầu"), DataType(DataType.Date)]
    public DateOnly? StartDate { get; set; }
    [Display(Name = "Ngày kết thúc"), DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }
    public bool HasResults { get; set; }
    public PagedList<NewsManagementDto> Articles { get; set; } = new([], 1, 0);
}
