namespace Acbs.Vsdc.TestHub.Modules.Msp.Catalog;

/// <summary>
/// Mã lỗi kỹ thuật có thể xuất hiện tại tag 405 của điện NAK F21.
/// </summary>
public static class MspNakErrorCodes
{
    public const string Unknown = "T02";
    public const string InvalidDataType = "T31";
    public const string InvalidBic = "T32";
    public const string DataTooLong = "T33";
    public const string MemberNotFound = "T44";
    public const string UnknownOperation = "T75";
    public const string UserNotRegistered = "T83";
    public const string UserNotAuthorized = "T84";
    public const string DuplicateMessage = "T98";
}
