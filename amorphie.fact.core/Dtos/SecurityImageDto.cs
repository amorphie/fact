
using amorphie.core.Base;
using static System.Net.Mime.MediaTypeNames;

public record GetSecurityImageResponse(
Guid Id,
string Image,
 DateTime CreatedAt ,
Guid CreatedBy,
Guid? CreatedByBehalfOf,
DateTime ModifiedAt,
Guid ModifiedBy,
Guid? ModifiedByBehalfOf);
public record PostSecurityImageRequest(
string Image,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);