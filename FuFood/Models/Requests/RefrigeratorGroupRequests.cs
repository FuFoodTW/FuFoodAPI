namespace FuFood.Models.Requests;

public class RefrigeratorJoinRequest
{
    public required string QrCode { get; set; }
}

public class RefrigeratorLeaveRequest
{
    public Guid? NewOwnerId { get; set; }
}
