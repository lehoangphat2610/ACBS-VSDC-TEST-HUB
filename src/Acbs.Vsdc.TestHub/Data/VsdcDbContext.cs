using Acbs.Vsdc.TestHub.Domain;
using Acbs.Vsdc.TestHub.Domain.Auth;
using Microsoft.EntityFrameworkCore;
namespace Acbs.Vsdc.TestHub.Data;
public sealed class VsdcDbContext(DbContextOptions<VsdcDbContext> options) : DbContext(options)
{
    public DbSet<GatewayFile> GatewayFiles => Set<GatewayFile>();
    public DbSet<GatewayMessage> GatewayMessages => Set<GatewayMessage>();
    public DbSet<MessageHeader> MessageHeaders => Set<MessageHeader>();
    public DbSet<MessageBlock> MessageBlocks => Set<MessageBlock>();
    public DbSet<MessageTag> MessageTags => Set<MessageTag>();
    public DbSet<MessageReference> MessageReferences => Set<MessageReference>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerIdentity> CustomerIdentities => Set<CustomerIdentity>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountMapping> AccountMappings => Set<AccountMapping>();
    public DbSet<AccountChange> AccountChanges => Set<AccountChange>();
    public DbSet<Security> Securities => Set<Security>();
    public DbSet<OrderRecord> Orders => Set<OrderRecord>();
    public DbSet<TradeRecord> Trades => Set<TradeRecord>();
    public DbSet<CashTransaction> CashTransactions => Set<CashTransaction>();
    public DbSet<SecuritiesTransfer> SecuritiesTransfers => Set<SecuritiesTransfer>();
    public DbSet<RightsRegistration> RightsRegistrations => Set<RightsRegistration>();
    public DbSet<CorporateAction> CorporateActions => Set<CorporateAction>();
    public DbSet<FeeRecord> Fees => Set<FeeRecord>();
    public DbSet<TaxRecord> Taxes => Set<TaxRecord>();
    public DbSet<NavRecord> NavRecords => Set<NavRecord>();
    public DbSet<ReportFile> ReportFiles => Set<ReportFile>();
    public DbSet<ReportRow> ReportRows => Set<ReportRow>();
    public DbSet<ProcessingHistory> ProcessingHistories => Set<ProcessingHistory>();
    public DbSet<ValidationError> ValidationErrors => Set<ValidationError>();
    public DbSet<OutboxJob> OutboxJobs => Set<OutboxJob>();
    public DbSet<InboxJob> InboxJobs => Set<InboxJob>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
    public DbSet<SimulatorRule> SimulatorRules => Set<SimulatorRule>();
    public DbSet<SimulatorRun> SimulatorRuns => Set<SimulatorRun>();
    public DbSet<ManualTemplate> ManualTemplates => Set<ManualTemplate>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<FileCheckpoint> FileCheckpoints => Set<FileCheckpoint>();
    public DbSet<MessageStatusHistory> MessageStatusHistories => Set<MessageStatusHistory>();
    public DbSet<MspBusinessMessage> MspBusinessMessages => Set<MspBusinessMessage>();
    public DbSet<MspAckNak> MspAckNaks => Set<MspAckNak>();
    public DbSet<MspNarrativeItem> MspNarrativeItems => Set<MspNarrativeItem>();
    public DbSet<MspSecuritiesPositionInstruction> MspSecuritiesPositionInstructions => Set<MspSecuritiesPositionInstruction>();
    public DbSet<MspSecuritiesPositionResponse> MspSecuritiesPositionResponses => Set<MspSecuritiesPositionResponse>();
    public DbSet<MspCashInstruction> MspCashInstructions => Set<MspCashInstruction>();
    public DbSet<MspCashResponse> MspCashResponses => Set<MspCashResponse>();
    public DbSet<MspSettlementInstruction> MspSettlementInstructions => Set<MspSettlementInstruction>();
    public DbSet<MspStatusInquiry> MspStatusInquiries => Set<MspStatusInquiry>();
    public DbSet<MspStatusResponse> MspStatusResponses => Set<MspStatusResponse>();
    public DbSet<MspReconcileInquiry> MspReconcileInquiries => Set<MspReconcileInquiry>();
    public DbSet<MspReconcileResponse> MspReconcileResponses => Set<MspReconcileResponse>();
    public DbSet<MspParty> MspParties => Set<MspParty>();
    public DbSet<MspAmount> MspAmounts => Set<MspAmount>();
    public DbSet<MspBalanceMovement> MspBalanceMovements => Set<MspBalanceMovement>();
    public DbSet<MspReportStatisticRow> MspReportStatisticRows => Set<MspReportStatisticRow>();
    public DbSet<MspParMetadata> MspParMetadata => Set<MspParMetadata>();
    public DbSet<MspOperationDefinition> MspOperationDefinitions => Set<MspOperationDefinition>();
    public DbSet<MspFieldDefinition> MspFieldDefinitions => Set<MspFieldDefinition>();
    public DbSet<MspTemplateVersion> MspTemplateVersions => Set<MspTemplateVersion>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();
    public DbSet<UserAuditLog> UserAuditLogs => Set<UserAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("vsdc");
        modelBuilder.ConfigureCoreModel();
        modelBuilder.ConfigureMspModel();
        modelBuilder.ConfigureAuthModel();
    }
}
