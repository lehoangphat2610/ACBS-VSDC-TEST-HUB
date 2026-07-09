document.querySelectorAll('input,select,textarea').forEach(x => x.addEventListener('focus', () => x.classList.add('focused')));

// Top-right user dropdown.
(function () {
    const menu = document.querySelector('[data-user-menu]');
    if (!menu) return;

    const button = menu.querySelector('[data-user-menu-button]');
    const panel = menu.querySelector('[data-user-menu-panel]');
    if (!button || !panel) return;

    function setOpen(open) {
        menu.classList.toggle('open', open);
        button.setAttribute('aria-expanded', open ? 'true' : 'false');
    }

    button.addEventListener('click', function (event) {
        event.preventDefault();
        event.stopPropagation();
        setOpen(!menu.classList.contains('open'));
    });

    document.addEventListener('click', function (event) {
        if (!menu.contains(event.target)) setOpen(false);
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') setOpen(false);
    });
})();

// Simulator ManualMode expand/collapse.
document.querySelectorAll('[data-toggle-manual]').forEach(button => {
    button.addEventListener('click', () => {
        const card = button.closest('.manual-card');
        if (!card) return;
        card.classList.toggle('open');
    });
});

// Swagger-like API execution buttons.
document.querySelectorAll('.api-action').forEach(button => {
    button.addEventListener('click', async event => {
        event.preventDefault();
        event.stopPropagation();
        const resultBox = document.querySelector('[data-api-result]');
        const method = button.getAttribute('data-api-method') || 'GET';
        const url = button.getAttribute('data-api-url');
        if (!url) return;

        let body = null;
        if (button.getAttribute('data-api-body-form') === 'manual') {
            const form = button.closest('form');
            if (form) {
                const fd = new FormData(form);
                body = JSON.stringify({
                    operation: fd.get('Operation'),
                    result: fd.get('Result'),
                    reference: fd.get('Reference'),
                    accountNo: fd.get('AccountNo'),
                    targetAccount: fd.get('TargetAccount'),
                    reason: fd.get('Reason')
                });
            }
        }

        if (resultBox) resultBox.textContent = `Executing ${method} ${url} ...`;
        button.disabled = true;
        try {
            const response = await fetch(url, {
                method,
                credentials: 'same-origin',
                headers: body ? { 'Content-Type': 'application/json' } : {},
                body
            });
            const text = await response.text();
            let formatted = text;
            try { formatted = JSON.stringify(JSON.parse(text), null, 2); } catch { /* keep text */ }
            if (resultBox) resultBox.textContent = `${response.status} ${response.statusText}\n${formatted}`;
            if (response.ok && (url.includes('/start') || url.includes('/stop') || url.includes('/manual/'))) {
                setTimeout(() => window.location.reload(), 750);
            }
        } catch (error) {
            if (resultBox) resultBox.textContent = `ERROR\n${error}`;
        } finally {
            button.disabled = false;
        }
    });
});

// MSP editor live review preview.
(function () {
    const editors = document.querySelectorAll('[data-msp-editor]');
    if (!editors.length) return;

    const value = (root, key) => {
        const el = root.querySelector(`[data-fin="${key}"]`);
        return el ? (el.value || '').trim() : '';
    };

    const ymd = (dateText) => {
        if (!dateText) return new Date().toISOString().slice(0, 10).replaceAll('-', '');
        return dateText.replaceAll('-', '').replaceAll('/', '');
    };

    const money = (text) => (text || '0').replaceAll(',', '').trim();

    const normalizeBic = (text, fallback) => {
        let bic = (text || fallback || 'VSDC404X').trim().toUpperCase();
        if (/^\d{3}$/.test(bic)) return `VSDC${bic}X`;
        if (bic.length >= 8) return bic.substring(0, 8);
        return bic.padEnd(8, 'X');
    };

    function buildPreview(root) {
        const mt = root.getAttribute('data-msp-type') || '599';
        const operation = root.getAttribute('data-msp-operation') || 'MSP_OPERATION';
        const ref = value(root, 'reference') || 'ACBSREF';
        const related = value(root, 'related');
        const account = value(root, 'account') || '006C123456';
        const isin = value(root, 'isin') || 'VN000000HPG4';
        const date = ymd(value(root, 'date'));
        const qty = money(value(root, 'quantity') || '10000');
        const narrative = value(root, 'narrative');
        const amount = money(value(root, 'amount')) || '1500000000';
        const currency = value(root, 'currency') || 'VND';
        const reason = value(root, 'reason') || narrative || 'STOCK';
        const price = money(value(root, 'price')) || '105500';
        const fee = money(value(root, 'fee')) || '527500';
        const tax = money(value(root, 'tax')) || '120000';
        const receiverBic = normalizeBic(value(root, 'receiverBic'), 'VSDC404X');
        const bic = normalizeBic(value(root, 'bic'), receiverBic);
        const originalType = value(root, 'originalType') || '199';
        const originalRef = value(root, 'originalRef') || related || 'ORIGINALREF';

        const inputHeader = (messageType) => `{1:F01VSDC002XAXXX0000000000}{2:I${messageType}${receiverBic}XXXXN}{4:\n`;

        if (mt === '524') {
            const isUnblock = operation.includes('UNBLOCK');
            const isCancel = operation.includes('CANCEL');
            const needsLink = isUnblock || isCancel;
            const functionCode = isCancel ? 'CANC' : 'NEWM';
            return inputHeader('524') +
                `:16R:GENL
` +
                `:20C::SEME//${ref}
` +
                `:23G:${functionCode}
` +
                `:98A::PREP//${date}
` +
                (needsLink ? `:16R:LINK
:20C::PREV//${related || 'ACBS2606052575'}
:16S:LINK
` : '') +
                `:16S:GENL
` +
                `:16R:INPOSDET
` +
                `:97A::SAFE//${account}
` +
                `:35B:ISIN ${isin}
` +
                `:36B::SETT//UNIT/${qty}
` +
                `:98A::SETT//${date}
` +
                (narrative ? `:70E::SPRO//${narrative}
` : '') +
                `:93A::FROM//${isUnblock ? 'BLOK' : 'AVAI'}
` +
                `:93A::TOBA//${isUnblock ? 'AVAI' : 'BLOK'}
` +
                `:16S:INPOSDET
-}{5:{CHK:123456789ABC}}`;
        }



        if (mt === '199' && operation.includes('CASH')) {
            const func = operation.includes('UNBLOCK') ? 'UNBLOCK' : 'BLOCK';
            return inputHeader('199') +
                `:20:${ref}\n` +
                (related ? `:21:${related}\n` : '') +
                `:79:/FUNC/${func}\n` +
                `/ACCOUNT/${account}\n` +
                `/AMOUNT/${amount}\n` +
                `/CURRENCY/${currency}\n` +
                `/REASON/${reason}\n` +
                ((related || value(root, 'previous')) ? `/REF/${value(root, 'previous') || related}\n` : '') +
                `-}{5:{CHK:123456789ABC}}`;
        }

        if (mt === '541' || mt === '543') {
            const buy = mt === '541';
            return inputHeader(mt) +
                `:16R:GENL\n` +
                `:20C::SEME//${ref}\n` +
                `:23G:NEWM\n` +
                `:16S:GENL\n` +
                `:16R:TRADDET\n` +
                `:98A::TRAD//${date}\n` +
                `:90B::DEAL//ACTU/${price}\n` +
                `:35B:ISIN ${isin}\n` +
                `:16S:TRADDET\n` +
                `:16R:FIAC\n` +
                `:36B::SETT//UNIT/${qty}\n` +
                (narrative ? `:70D::DENC//${narrative}\n` : '') +
                `:97A::SAFE//${account}\n` +
                `:16S:FIAC\n` +
                `:16R:SETDET\n` +
                `:22F::SETR//TRAD\n` +
                `:16R:SETPRTY\n` +
                `:95P::${buy ? 'REAG' : 'DEAG'}//${bic}\n` +
                `:16S:SETPRTY\n` +
                `:16R:AMT\n` +
                `:19A::SETT//VND${amount}\n` +
                `:19A::CHAR//VND${fee}\n` +
                `:19A::TRAX//VND${tax}\n` +
                `:16S:AMT\n` +
                `:16S:SETDET\n-}{5:{CHK:123456789ABC}}`;
        }

        if (operation.includes('STATUS')) {
            return inputHeader(originalType === '599' ? '599' : '199') +
                `:20:${ref}\n` +
                `:79:/FUNC/STATUS_INQUIRY\n` +
                `/PREV_MSG_TYPE/${originalType}\n` +
                `/PREV_MSG_REF/${originalRef}\n` +
                `/ACCT/${account}\n` +
                `/ISIN/${isin}\n` +
                `/TRD_DATE/${date}\n` +
                `-}{5:{CHK:123456789ABC}}`;
        }

        return inputHeader('599') +
            `:20:${ref}\n` +
            `:79:/FUNC/RECONCILE_INQUIRY\n` +
            `/TRD_DATE/${date}\n` +
            `-}{5:{CHK:123456789ABC}}`;
    }

    function refresh(root) {
        const box = root.querySelector('[data-fin-preview]');
        if (box) box.textContent = buildPreview(root);
    }

    editors.forEach(root => {
        root.querySelectorAll('input,select,textarea').forEach(el => {
            el.addEventListener('input', () => refresh(root));
            el.addEventListener('change', () => refresh(root));
        });
        root.querySelectorAll('[data-refresh-preview]').forEach(btn => btn.addEventListener('click', () => refresh(root)));
        root.querySelectorAll('[data-copy-preview]').forEach(btn => btn.addEventListener('click', async () => {
            const box = root.querySelector('[data-fin-preview]');
            if (!box) return;
            try {
                await navigator.clipboard.writeText(box.textContent || '');
                btn.classList.add('copy-ok');
                const old = btn.textContent;
                btn.textContent = 'Copied';
                setTimeout(() => { btn.classList.remove('copy-ok'); btn.textContent = old; }, 1200);
            } catch { /* ignore */ }
        }));
        refresh(root);
    });
})();
