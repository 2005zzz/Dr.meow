(() => {
    const tips = document.getElementById('tips');
    const modeTag = document.getElementById('modeTag');
    const menu = document.querySelectorAll('.menu-item');
    const form = document.getElementById('askForm');
    const input = document.getElementById('askInput');
    const msgs = document.getElementById('messages');
    const results = document.getElementById('results');
    const scrollEl = document.getElementById('scroll');
    const btnClear = document.getElementById('btnClear');

    const REPORTS_URL = '/Reports';

    menu.forEach(m => {
        m.addEventListener('click', () => {
            menu.forEach(x => x.classList.remove('active'));
            m.classList.add('active');
            const mode = m.dataset.mode;
            if (mode) {
                modeTag.textContent = mode === 'consult' ? 'RAG 顧問' : mode === 'forms' ? '新增表單' : '報表分析'; '表單管理';
                addTip(`已切換至「${modeTag.textContent}」。`, 'success');
                if (mode === 'reports') addTip(`開啟報表頁：<a href="${REPORTS_URL}" target="_blank">Reports</a>`, 'info');
                return;
            }
            const p = m.dataset.prompt;
            if (p) { input.value = `請提供「${p}」的作業重點與處置流程`; input.focus(); addTip(`已套用主題「${p}」。`, 'secondary'); }
        });
    });

    btnClear?.addEventListener('click', () => { msgs.innerHTML = ''; results.innerHTML = ''; addTip('已清空對話與結果。', 'secondary'); });

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const q = input.value.trim(); if (!q) return;
        pushUser(q); input.value = ''; results.innerHTML = ''; pushBot('查詢中…');

        try {
            const r = await fetch('/sample-data/search.json').catch(() => null);
            const data = r ? await r.json() : { answer: '（尚未接 API，顯示假資料）', items: [] };
            replaceLastBot(data.answer || '（沒有 AI 回覆）');
            (data.items || []).forEach(addCard);
            scrollBottom();
        } catch { replaceLastBot('查詢失敗，請稍後再試。'); }
    });

    function addTip(text, type = 'info') { tips?.insertAdjacentHTML('afterbegin', `<div class="alert alert-${type} py-2 small mb-2">${text}</div>`); }
    function pushUser(t) { msgs.insertAdjacentHTML('beforeend', `<div class="alert alert-primary small mb-2">${escape(t)}</div>`); scrollBottom(); }
    function pushBot(t) { msgs.insertAdjacentHTML('beforeend', `<div class="alert alert-secondary small mb-3" data-bot="1">${escape(t)}</div>`); scrollBottom(); }
    function replaceLastBot(t) { const els = [...document.querySelectorAll('[data-bot="1"]')]; const last = els.pop(); if (last) last.innerHTML = escape(t); }
    function addCard(x) {
        results.insertAdjacentHTML('beforeend', `
      <div class="col"><div class="card h-100"><div class="card-body">
        <div class="fw-bold mb-1"><a href="${x.url || '#'}" target="_blank">${escape(x.title || '（無標題）')}</a></div>
        <div class="text-muted small mb-2">${escape(x.source || '')}&emsp;${x.modifiedAt ? new Date(x.modifiedAt).toLocaleDateString() : ''}</div>
        <div class="mb-3">${escape(x.snippet || '')}</div>
        <div class="d-flex gap-2">
          <a class="btn btn-outline-primary btn-sm" target="_blank" href="${x.url || '#'}">開啟文件</a>
          <a class="btn btn-success btn-sm" href="/Incidents/New">新增為事件</a>
        </div>
      </div></div></div>`);
    }
    function escape(s) { return (s || '').replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m])) }
    function scrollBottom() { scrollEl.scrollTop = scrollEl.scrollHeight; }
})();
