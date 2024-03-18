using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

[Index(nameof(ClientId), nameof(UserId), nameof(DeviceId), nameof(InstallationId))]
[Index(nameof(ClientId), nameof(DeviceId), nameof(InstallationId))]
[Index(nameof(UserId), nameof(DeviceId), nameof(InstallationId))]
[Index(nameof(DeviceId), nameof(InstallationId))]
[Index(nameof(ClientId), nameof(UserId))]
[Index(nameof(ClientId), nameof(DeviceId))]
[Index(nameof(UserId))]
[Index(nameof(DeviceId))]
public class UserDevice : EntityBase
{
    public string DeviceId { get; set; }
    public Guid InstallationId { get; set; }
    public string? DeviceToken { get; set; }
    public string? DeviceModel { get; set; }
    public string? DevicePlatform { get; set; }
    public string? Manufacturer { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public string? RemovalReason { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? ActivationDate { get; set; }
    public DateTime? LastLogonDate { get; set; }
    public DateTime? ActivationRemovalDate { get; set; }
    public bool? IsGoogleServiceAvailable { get; set; }
    public bool? IsOnApp { get; set; }
    public Guid? TokenId { get; set; }
    public string? ClientId { get; set; }
    [ForeignKey("User")]
    public Guid? UserId { get; set; }
    public User? Users { get; set; }
    public int Status { get; set; }
    public bool IsRegistered
    {
        get
        {
            return Status == 1 && !string.IsNullOrWhiteSpace(DeviceToken);
        }
    }
}