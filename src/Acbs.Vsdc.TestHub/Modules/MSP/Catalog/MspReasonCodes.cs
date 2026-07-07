namespace Acbs.Vsdc.TestHub.Modules.Msp.Catalog;

/// <summary>
/// Bộ mã lý do nghiệp vụ MSP theo Phụ lục 07.
/// </summary>
public static class MspReasonCodes
{
    public const string InsufficientFunds = "NSFF";
    public const string InsufficientSecurities = "NSEC";
    public const string InvalidOrClosedAccount = "IVAC";
    public const string AwaitingAuthorization = "AUTH";
    public const string ComplianceHold = "COMP";
    public const string BadReferenceOrMismatch = "BREF";

    // Mã được dùng trong mẫu phản hồi báo cáo/validation của dự án.
    public const string InvalidDate = "IVDT";
    public const string TechnicalError = "TECH";
}
