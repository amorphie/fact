

using static System.Net.Mime.MediaTypeNames;

namespace amorphie.user.data;
public record GetSecurityImageResponse(
Guid Id,
string Image,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);
public record PostSecurityImageRequest(
string Image,
Guid UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);