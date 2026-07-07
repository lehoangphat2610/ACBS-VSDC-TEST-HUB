using Acbs.Vsdc.TestHub.Models;
using Acbs.Vsdc.TestHub.Services.Files;
using Acbs.Vsdc.TestHub.Services.Fin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Acbs.Vsdc.TestHub.Controllers;

[Authorize(Policy = "TesterOrAdmin")]
public sealed class TestModeController(OutgoingMessageService outgoing) : Controller
{
    [HttpGet]
    public IActionResult Create() => View(new OutgoingMessageForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OutgoingMessageForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(form);

        var path = await outgoing.CreateAndSendAsync(new OutgoingFinRequest(
            form.Operation,
            form.Reference,
            form.AccountNo,
            form.CustomerName,
            form.TargetAccount,
            form.IdentityNo,
            form.Note), cancellationToken);

        TempData["Success"] = $"Đã tạo điện và ghi vào Send: {path}";
        return RedirectToAction(nameof(Create));
    }
}
