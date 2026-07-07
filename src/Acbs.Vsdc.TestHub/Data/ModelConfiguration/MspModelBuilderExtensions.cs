using Acbs.Vsdc.TestHub.Domain;
using Microsoft.EntityFrameworkCore;
namespace Acbs.Vsdc.TestHub.Data;
public static class MspModelBuilderExtensions
{
    public static void ConfigureMspModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MspBusinessMessage>().HasIndex(x => new { x.MessageType, x.OperationCode });
        modelBuilder.Entity<MspBusinessMessage>().HasIndex(x => x.SenderReference);
        modelBuilder.Entity<MspNarrativeItem>().HasIndex(x => new { x.GatewayMessageId, x.Key });
        modelBuilder.Entity<MspSecuritiesPositionInstruction>().HasIndex(x => new { x.SafeAccount, x.Isin });
        modelBuilder.Entity<MspCashInstruction>().HasIndex(x => x.AccountNo);
        modelBuilder.Entity<MspSettlementInstruction>().HasIndex(x => new { x.TradeDate, x.SafeAccount, x.Isin });
        modelBuilder.Entity<MspStatusInquiry>().HasIndex(x => x.OriginalReference);
        modelBuilder.Entity<MspReportStatisticRow>().HasIndex(x => new { x.SendTime, x.MessageType });
        modelBuilder.Entity<MspParMetadata>().HasIndex(x => x.OriginalTransferReference);
        modelBuilder.Entity<MspOperationDefinition>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<MspFieldDefinition>().HasIndex(x => new { x.MspOperationDefinitionId, x.SequenceNo });
        modelBuilder.Entity<MspTemplateVersion>().HasIndex(x => new { x.OperationCode, x.Version }).IsUnique();
    }
}
