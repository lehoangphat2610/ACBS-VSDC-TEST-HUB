using Acbs.Vsdc.TestHub.Areas.MSP.Models;
using Acbs.Vsdc.TestHub.Modules.Msp.Catalog;
using Acbs.Vsdc.TestHub.Modules.Msp.Models;
using Acbs.Vsdc.TestHub.Services.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acbs.Vsdc.TestHub.Areas.MSP.Controllers;

[Area("MSP")]
[Authorize(Policy = "TesterOrAdmin")]
public sealed class MspTestController(OutgoingMspMessageService outgoing) : Controller
{
    public IActionResult Index() => View(new MspCreateMenuViewModel
    {
        Items =
        [
            new(MspOperationCodes.SecuritiesBlock, "MT524 - Phong tỏa chứng khoán", nameof(SecuritiesBlock), "524"),
            new(MspOperationCodes.SecuritiesBlockCancel, "MT524 - Hủy yêu cầu phong tỏa chứng khoán", nameof(SecuritiesBlockCancel), "524"),
            new(MspOperationCodes.SecuritiesUnblock, "MT524 - Giải tỏa chứng khoán", nameof(SecuritiesUnblock), "524"),
            new(MspOperationCodes.SecuritiesUnblockCancel, "MT524 - Hủy yêu cầu giải tỏa chứng khoán", nameof(SecuritiesUnblockCancel), "524"),
            new(MspOperationCodes.CashBlock, "MT199 - Phong tỏa tiền", nameof(CashBlock), "199"),
            new(MspOperationCodes.CashUnblock, "MT199 - Giải tỏa tiền", nameof(CashUnblock), "199"),
            new(MspOperationCodes.SettlementBuy, "MT541 - Chỉ thị thanh toán mua", nameof(BuySettlement), "541"),
            new(MspOperationCodes.SettlementSell, "MT543 - Chỉ thị thanh toán bán", nameof(SellSettlement), "543"),
            new(MspOperationCodes.StatusInquiry, "MT199 - Tra soát trạng thái", nameof(StatusInquiry), "199"),
            new(MspOperationCodes.ReconcileInquiry, "MT599 - Yêu cầu báo cáo đối chiếu", nameof(ReconcileInquiry), "599")
        ]
    });

    [HttpGet]
    public IActionResult SecuritiesBlock() => Mt524View(new Mt524Form(), unblock: false, cancel: false);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SecuritiesBlock(Mt524Form form, CancellationToken ct) => Send524(form, unblock: false, cancel: false, ct);

    [HttpGet]
    public IActionResult SecuritiesBlockCancel() => Mt524View(new Mt524Form(), unblock: false, cancel: true);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SecuritiesBlockCancel(Mt524Form form, CancellationToken ct) => Send524(form, unblock: false, cancel: true, ct);

    [HttpGet]
    public IActionResult SecuritiesUnblock() => Mt524View(new Mt524Form(), unblock: true, cancel: false);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SecuritiesUnblock(Mt524Form form, CancellationToken ct) => Send524(form, unblock: true, cancel: false, ct);

    [HttpGet]
    public IActionResult SecuritiesUnblockCancel() => Mt524View(new Mt524Form(), unblock: true, cancel: true);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SecuritiesUnblockCancel(Mt524Form form, CancellationToken ct) => Send524(form, unblock: true, cancel: true, ct);

    [HttpGet]
    public IActionResult CashBlock() => View("Mt199Cash", new Mt199CashForm());

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> CashBlock(Mt199CashForm form, CancellationToken ct) => SendCash(form, false, ct);

    [HttpGet]
    public IActionResult CashUnblock() => View("Mt199Cash", new Mt199CashForm());

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> CashUnblock(Mt199CashForm form, CancellationToken ct) => SendCash(form, true, ct);

    [HttpGet]
    public IActionResult BuySettlement() => View("Settlement", new SettlementForm());

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> BuySettlement(SettlementForm form, CancellationToken ct) => SendSettlement(form, true, ct);

    [HttpGet]
    public IActionResult SellSettlement() => View("Settlement", new SettlementForm());

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SellSettlement(SettlementForm form, CancellationToken ct) => SendSettlement(form, false, ct);

    [HttpGet]
    public IActionResult StatusInquiry() => View(new StatusInquiryForm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> StatusInquiry(StatusInquiryForm form, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(form);

        var request = new StatusInquiryRequest
        {
            Reference = form.Reference,
            ReceiverBic = form.ReceiverBic,
            OriginalMessageType = form.OriginalMessageType,
            OriginalReference = form.OriginalReference,
            AccountNo = form.AccountNo,
            Isin = form.Isin,
            TradeDate = form.TradeDate
        };

        var path = await outgoing.CreateAndSendAsync(MspOperationCodes.StatusInquiry, request, form.Reference, ct);
        TempData["Success"] = $"Đã tạo {path}";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ReconcileInquiry() => View(new ReconcileInquiryForm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReconcileInquiry(ReconcileInquiryForm form, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(form);

        var request = new ReconcileInquiryRequest
        {
            Reference = form.Reference,
            ReceiverBic = form.ReceiverBic,
            TradeDate = form.TradeDate
        };

        var path = await outgoing.CreateAndSendAsync(MspOperationCodes.ReconcileInquiry, request, form.Reference, ct);
        TempData["Success"] = $"Đã tạo {path}";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult Mt524View(Mt524Form form, bool unblock, bool cancel)
    {
        ViewData["Mt524IsUnblock"] = unblock;
        ViewData["Mt524IsCancel"] = cancel;
        return View("Mt524", form);
    }

    private async Task<IActionResult> Send524(Mt524Form form, bool unblock, bool cancel, CancellationToken ct)
    {
        if (!ModelState.IsValid) return Mt524View(form, unblock, cancel);

        if ((cancel || unblock) && string.IsNullOrWhiteSpace(form.RelatedReference))
        {
            ModelState.AddModelError(nameof(form.RelatedReference), "Phải nhập mã điện tham chiếu cho điện hủy hoặc giải tỏa chứng khoán.");
            return Mt524View(form, unblock, cancel);
        }

        var request = new Mt524InstructionRequest
        {
            Reference = form.Reference,
            RelatedReference = form.RelatedReference,
            ReceiverBic = form.ReceiverBic,
            AccountNo = form.AccountNo,
            Isin = form.Isin,
            Quantity = form.Quantity,
            EffectiveDate = form.EffectiveDate,
            Narrative = form.Narrative,
            IsUnblock = unblock,
            IsCancel = cancel
        };

        var operation = (unblock, cancel) switch
        {
            (false, false) => MspOperationCodes.SecuritiesBlock,
            (false, true) => MspOperationCodes.SecuritiesBlockCancel,
            (true, false) => MspOperationCodes.SecuritiesUnblock,
            (true, true) => MspOperationCodes.SecuritiesUnblockCancel
        };
        var path = await outgoing.CreateAndSendAsync(operation, request, form.Reference, ct);
        TempData["Success"] = $"Đã tạo {path}";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> SendCash(Mt199CashForm form, bool unblock, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Mt199Cash", form);

        var request = new Mt199CashInstructionRequest
        {
            Reference = form.Reference,
            RelatedReference = form.RelatedReference,
            ReceiverBic = form.ReceiverBic,
            AccountNo = form.AccountNo,
            Amount = form.Amount,
            Currency = form.Currency,
            Reason = form.Reason,
            PreviousReference = form.PreviousReference,
            IsUnblock = unblock
        };

        var operation = unblock ? MspOperationCodes.CashUnblock : MspOperationCodes.CashBlock;
        var path = await outgoing.CreateAndSendAsync(operation, request, form.Reference, ct);
        TempData["Success"] = $"Đã tạo {path}";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> SendSettlement(SettlementForm form, bool buy, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Settlement", form);

        var receiverBic = string.IsNullOrWhiteSpace(form.ReceiverBic) ? form.CounterpartyBic : form.ReceiverBic;
        var request = new SettlementInstructionRequest
        {
            Reference = form.Reference,
            ReceiverBic = receiverBic,
            AccountNo = form.AccountNo,
            Isin = form.Isin,
            TradeDate = form.TradeDate,
            Price = form.Price,
            Quantity = form.Quantity,
            SettlementAmount = form.SettlementAmount,
            FeeAmount = form.FeeAmount,
            TaxAmount = form.TaxAmount,
            CounterpartyBic = string.IsNullOrWhiteSpace(form.CounterpartyBic) ? receiverBic : form.CounterpartyBic,
            Narrative = form.Narrative,
            IsBuy = buy
        };

        var operation = buy ? MspOperationCodes.SettlementBuy : MspOperationCodes.SettlementSell;
        var path = await outgoing.CreateAndSendAsync(operation, request, form.Reference, ct);
        TempData["Success"] = $"Đã tạo {path}";
        return RedirectToAction(nameof(Index));
    }
}
