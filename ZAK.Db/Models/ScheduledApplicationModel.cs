using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZAK.Db.Models;

[PrimaryKey(nameof(brigadeId), nameof(scheduledTime))]
public class ScheduledApplicationModel
{
    [ForeignKey("applicationId")]
    public int applicationId { get; set; }
    public ApplicationModel application { get; set; } = null!;
    public int scheduledTime { get; set; }
    [ForeignKey("brigadeId")]
    public int brigadeId { get; set; }
    public BrigadeModel brigade { get; set; } = null!;
}
