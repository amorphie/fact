using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

[Index(nameof(DeviceId),nameof(InstallationId))]
[Index(nameof(DeviceId))]
public class UserDevice : EntityBase
{
    public Guid DeviceId { get; set; }
    public Guid InstallationId{get;set;}
    public string? DeviceToken{get;set;}
    public string? DeviceModel{get;set;}
    public string? DevicePlatform{get;set;}
    public Guid? TokenId { get; set; }
    public Guid? ClientId { get; set; }
    [ForeignKey("User")]
    public Guid? UserId { get; set; }
    public User? Users { get; set; }
    public int Status { get; set; }
}