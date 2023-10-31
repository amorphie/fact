using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class ChangePasswordDto
{
    public string? oldPassword { get; set; }
    public string? newPassword { get; set; }
    public string? newPasswordConfirm { get; set; }
}
