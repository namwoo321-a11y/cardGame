import { CARDMAKER_DEFAULT_CONFIG } from "./card-maker.config.js";

const DB_NAME = "pe-cardmaker";
const DB_VERSION = 1;
const DRAFT_KEY = "active-draft";
const CONFIG_KEY = "card-maker-source-url";
const MAX_BACKUPS = 10;
const STANDARD_FIELDS = ["Tier", "cardName", "cost", "cost2", "description", "CType", "TType", "User", "Keyword"];

let schema = { effects: {} };
let state = createEmptyState();

function createEmptyState() {
  return { records: [], activeId: null, sourceUrl: CARDMAKER_DEFAULT_CONFIG.sourceUrl, sourceLabel: "초안", loadedAt: null };
}

const clone = (value) => JSON.parse(JSON.stringify(value));
const text = (value) => value === null || value === undefined ? "" : String(value);
const effectColumnOrder = (key) => Number((/^effects(\d+)$/i.exec(key) || [])[1] || Number.MAX_SAFE_INTEGER);
const isEffectColumn = (key) => /^effects\d+$/i.test(key);
const isRemoveThis = (effect) => ["removethis", "소멸", "[소멸]"].includes(text(effect).trim().toLowerCase());

export function parseEffect(raw, effectSchema = schema) {
  const source = text(raw).trim();
  if (!source) return { error: "효과가 비어 있습니다." };
  const parts = source.split(":").map((part) => part.trim());
  const action = parts[0].toLowerCase();
  const aliases = effectSchema.aliases || {};
  const canonical = text(aliases[action] || action).toLowerCase();
  const definition = (effectSchema.effects || {})[canonical];
  if (!definition) return { error: `알 수 없는 효과 '${parts[0]}'입니다.` };
  if (parts.length < definition.minParts) return { error: `'${parts[0]}' 효과는 값 ${definition.minParts}개 이상이 필요합니다.` };
  return { action: canonical, parts, definition };
}

export function migratePostUseMarkers(record) {
  const next = record;
  next.notes ??= [];
  let changed = false;
  const description = text(next.values.description);
  if (/\[소멸\]/i.test(description)) {
    next.values.description = description.replace(/\s*\[소멸\]\s*/gi, " ").replace(/\s{2,}/g, " ").trim();
    changed = true;
  }

  const keywordField = Object.prototype.hasOwnProperty.call(next.values, "Keyword") ? "Keyword" : "Keywords";
  const keywordValues = text(next.values[keywordField]).split(/[,|]/).map((value) => value.trim()).filter(Boolean);
  const keptKeywords = keywordValues.filter((value) => !/^\[?소멸\]?$/i.test(value));
  if (keptKeywords.length !== keywordValues.length) {
    next.values[keywordField] = keptKeywords.join(", ");
    changed = true;
  }

  const hadLegacyEffect = next.effects.some((effect) => text(effect).trim() !== "removeThis" && isRemoveThis(effect));
  next.effects = next.effects.map((effect) => isRemoveThis(effect) ? "removeThis" : text(effect).trim()).filter(Boolean);
  if (hadLegacyEffect) changed = true;
  if (changed && !next.effects.some((effect) => isRemoveThis(effect))) next.effects.push("removeThis");
  if (changed && !next.notes.includes("[소멸]을 description/Keyword에서 effects의 removeThis로 옮겼습니다.")) {
    next.notes.push("[소멸]을 description/Keyword에서 effects의 removeThis로 옮겼습니다.");
  }
  return changed;
}

export function serializeRecord(record) {
  const row = clone(record.values);
  Object.keys(row).filter(isEffectColumn).forEach((key) => delete row[key]);
  record.effects.map((effect) => text(effect).trim()).filter(Boolean).forEach((effect, index) => { row[`effects${index + 1}`] = effect; });
  return row;
}

export function validateCard(record, allRecords = [], effectSchema = schema) {
  const row = serializeRecord(record);
  const errors = [];
  const warnings = [];
  const cardName = text(row.cardName).trim();
  const tier = text(row.Tier).trim();
  const cost = text(row.cost).trim();

  if (!cardName) errors.push("cardName은 필수입니다.");
  if (tier && (!Number.isInteger(Number(tier)) || Number(tier) < 0)) errors.push("Tier는 0 이상의 정수여야 합니다.");
  if (cost && (!Number.isInteger(Number(cost)) || Number(cost) < 0)) errors.push("cost는 0 이상의 정수여야 합니다.");
  if (text(row.cost2).trim() && !/^.+\s+\d+$/.test(text(row.cost2).trim())) errors.push("cost2는 '에너지 2'처럼 자원 이름과 수치로 작성하세요.");
  if (/\[소멸\]/i.test(text(row.description))) errors.push("[소멸]은 description에 쓰지 말고 effects에 removeThis로 넣으세요.");
  if (text(row.Keyword || row.Keywords).split(/[,|]/).some((keyword) => /^\[?소멸\]?$/i.test(keyword.trim()))) errors.push("소멸은 Keyword가 아니라 effects의 removeThis입니다.");

  record.effects.forEach((effect, index) => {
    const parsed = parseEffect(effect, effectSchema);
    if (parsed.error) errors.push(`effects${index + 1}: ${parsed.error}`);
    if (isRemoveThis(effect) && text(effect).trim() !== "removeThis") warnings.push(`effects${index + 1}: removeThis 표기를 권장합니다.`);
  });

  const sameIdentity = allRecords.filter((candidate) => candidate.id !== record.id &&
    text(serializeRecord(candidate).User || candidate.sheetName) === text(row.User || record.sheetName) &&
    text(serializeRecord(candidate).cardName) === cardName);
  if (cardName && sameIdentity.length) errors.push("같은 User 안에 같은 cardName이 이미 있습니다.");
  return { errors, warnings };
}

function toRecord(sheetName, row, sourceRow) {
  const values = clone(row || {});
  STANDARD_FIELDS.forEach((field) => { if (!Object.prototype.hasOwnProperty.call(values, field)) values[field] = ""; });
  const effects = Object.keys(values).filter(isEffectColumn).sort((a, b) => effectColumnOrder(a) - effectColumnOrder(b)).map((key) => text(values[key]).trim()).filter(Boolean);
  const record = {
    id: `${sheetName}:${sourceRow}:${text(values.cardName)}`,
    sheetName,
    sourceRow,
    original: clone(values),
    values,
    effects,
    notes: []
  };
  migratePostUseMarkers(record);
  return record;
}

function normalizePayload(payload) {
  const source = payload?.result && typeof payload.result === "object" ? payload.result : payload;
  if (!source || typeof source !== "object" || Array.isArray(source)) throw new Error("시트 묶음 JSON 형식이 아닙니다.");
  const records = [];
  Object.entries(source).forEach(([sheetName, rows]) => {
    if (sheetName.startsWith("NOEX_") || !Array.isArray(rows)) return;
    rows.forEach((row, index) => records.push(toRecord(sheetName, row, index + 2)));
  });
  return records;
}

function hasChanged(record) { return JSON.stringify(serializeRecord(record)) !== JSON.stringify(record.original); }
function getActive() { return state.records.find((record) => record.id === state.activeId) || null; }
function activeValidation() { const active = getActive(); return active ? validateCard(active, state.records) : { errors: [], warnings: [] }; }
function setStatus(message, kind = "") { const target = document.querySelector("#status"); target.textContent = message; target.className = kind; }
function escapeHtml(value) { return text(value).replace(/[&<>"']/g, (character) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;", "'": "&#39;" })[character]); }

async function openDb() {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onupgradeneeded = () => {
      const db = request.result;
      if (!db.objectStoreNames.contains("drafts")) db.createObjectStore("drafts");
      if (!db.objectStoreNames.contains("backups")) db.createObjectStore("backups", { keyPath: "id" });
    };
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
  });
}

async function dbRequest(storeName, mode, action) {
  const db = await openDb();
  return new Promise((resolve, reject) => {
    const transaction = db.transaction(storeName, mode);
    const store = transaction.objectStore(storeName);
    const request = action(store);
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
  }).finally(() => db.close());
}

async function saveDraft(reason = "수동 저장") {
  const snapshot = { savedAt: new Date().toISOString(), reason, state: clone(state) };
  try {
    await dbRequest("drafts", "readwrite", (store) => store.put(snapshot, DRAFT_KEY));
    await dbRequest("backups", "readwrite", (store) => store.put({ id: snapshot.savedAt, ...snapshot }));
    const backups = await dbRequest("backups", "readonly", (store) => store.getAll());
    for (const backup of backups.sort((a, b) => a.id.localeCompare(b.id)).slice(0, Math.max(0, backups.length - MAX_BACKUPS))) {
      await dbRequest("backups", "readwrite", (store) => store.delete(backup.id));
    }
    setStatus(`초안을 저장했습니다 · ${new Date().toLocaleTimeString()}`, "success");
  } catch (error) {
    localStorage.setItem("pe-cardmaker-fallback", JSON.stringify(snapshot));
    setStatus("브라우저 저장소 제한으로 임시 저장으로 보관했습니다.", "error");
  }
}

async function loadDraft() {
  try {
    const draft = await dbRequest("drafts", "readonly", (store) => store.get(DRAFT_KEY));
    if (draft?.state) state = draft.state;
  } catch {
    const fallback = localStorage.getItem("pe-cardmaker-fallback");
    if (fallback) state = JSON.parse(fallback).state;
  }
}

async function loadSchema() {
  const response = await fetch("schemas/cardml-schema.json", { cache: "no-store" });
  if (!response.ok) throw new Error("카드 효과 명세를 불러오지 못했습니다.");
  schema = await response.json();
}

async function loadRemote() {
  const url = document.querySelector("#source-url").value.trim();
  if (!url) return setStatus("읽기 API 주소가 비어 있습니다.", "error");
  setStatus("Google Sheets 원본을 읽는 중입니다…");
  try {
    const response = await fetch(url, { cache: "no-store" });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    state = { ...createEmptyState(), records: normalizePayload(await response.json()), sourceUrl: url, sourceLabel: "Google Sheets 원본", loadedAt: new Date().toISOString() };
    state.activeId = state.records[0]?.id || null;
    await saveDraft("원본 불러오기 전 자동 백업");
    render();
    setStatus(`${state.records.length}장을 불러왔습니다. [소멸] 표기는 필요한 경우 effects로 초안 이전했습니다.`, "success");
  } catch (error) {
    setStatus(`원본을 읽지 못했습니다: ${error.message}`, "error");
  }
}

async function loadFixture() {
  try {
    const response = await fetch("fixtures/cardml-sample.json", { cache: "no-store" });
    state = { ...createEmptyState(), records: normalizePayload(await response.json()), sourceLabel: "오프라인 예시", loadedAt: new Date().toISOString() };
    state.activeId = state.records[0]?.id || null;
    render();
    setStatus("오프라인 예시를 열었습니다.", "success");
  } catch (error) { setStatus(`예시를 열지 못했습니다: ${error.message}`, "error"); }
}

function filterRecords() {
  const query = document.querySelector("#filter").value.trim().toLowerCase();
  const sheet = document.querySelector("#sheet-filter").value;
  const stateFilter = document.querySelector("#state-filter").value;
  return state.records.filter((record) => {
    const validation = validateCard(record, state.records);
    const haystack = `${record.sheetName} ${serializeRecord(record).User} ${serializeRecord(record).cardName}`.toLowerCase();
    return (!query || haystack.includes(query)) && (!sheet || record.sheetName === sheet) &&
      (!stateFilter || (stateFilter === "changed" && hasChanged(record)) || (stateFilter === "error" && validation.errors.length));
  });
}

function renderList() {
  const list = document.querySelector("#card-list");
  const filtered = filterRecords();
  document.querySelector("#card-count").textContent = `${filtered.length} / ${state.records.length}장`;
  list.innerHTML = filtered.map((record) => {
    const data = serializeRecord(record);
    const validation = validateCard(record, state.records);
    const classes = ["card-list-item", record.id === state.activeId ? "active" : "", hasChanged(record) ? "changed" : "", validation.errors.length ? "error" : ""].filter(Boolean).join(" ");
    return `<button class="${classes}" data-select-card="${escapeHtml(record.id)}"><span class="list-title">${escapeHtml(data.cardName || "이름 없는 카드")}</span><span class="list-meta">${escapeHtml(record.sheetName)} · ${escapeHtml(data.User || "사용자 미설정")} · T${escapeHtml(data.Tier || "0")}</span></button>`;
  }).join("") || `<p class="empty-state">조건에 맞는 카드가 없습니다.</p>`;
}

function renderSheetFilter() {
  const select = document.querySelector("#sheet-filter");
  const chosen = select.value;
  const sheets = [...new Set(state.records.map((record) => record.sheetName))].sort();
  select.innerHTML = `<option value="">모든 시트</option>${sheets.map((sheet) => `<option value="${escapeHtml(sheet)}">${escapeHtml(sheet)}</option>`).join("")}`;
  select.value = sheets.includes(chosen) ? chosen : "";
}

function renderEditor() {
  const active = getActive();
  document.querySelector("#empty-editor").classList.toggle("hidden", !!active);
  document.querySelector("#card-editor").classList.toggle("hidden", !active);
  if (!active) return;
  const row = serializeRecord(active);
  document.querySelector("#active-location").textContent = `${active.sheetName} · 원본 ${active.sourceRow}행`;
  document.querySelector("#active-title").textContent = row.cardName || "이름 없는 카드";
  document.querySelectorAll("[data-field]").forEach((input) => { input.value = text(active.values[input.dataset.field]); });

  const effects = document.querySelector("#effects");
  const template = document.querySelector("#effect-row-template");
  effects.innerHTML = "";
  active.effects.forEach((effect, index) => {
    const fragment = template.content.cloneNode(true);
    fragment.querySelector(".effect-number").textContent = `effects${index + 1}`;
    const input = fragment.querySelector(".effect-input");
    input.value = effect; input.dataset.effectIndex = String(index);
    fragment.querySelector(".move-effect-up").dataset.effectIndex = String(index);
    fragment.querySelector(".move-effect-down").dataset.effectIndex = String(index);
    fragment.querySelector(".remove-effect").dataset.effectIndex = String(index);
    effects.append(fragment);
  });

  const validation = activeValidation();
  const results = [...active.notes.map((note) => ({ type: "warning", message: note })), ...validation.errors.map((message) => ({ type: "error", message })), ...validation.warnings.map((message) => ({ type: "warning", message }))];
  document.querySelector("#validation-results").innerHTML = results.length ? results.map((item) => `<li class="${item.type}">${escapeHtml(item.message)}</li>`).join("") : `<li class="ok">현재 카드 데이터는 명세를 통과했습니다.</li>`;
}

function renderPreview() {
  const active = getActive();
  const preview = document.querySelector("#card-preview");
  const summary = document.querySelector("#change-summary");
  if (!active) { preview.textContent = "카드를 선택하면 실제 저장값을 미리 봅니다."; preview.classList.add("empty-preview"); summary.textContent = "변경 없음"; return; }
  const row = serializeRecord(active);
  preview.classList.remove("empty-preview");
  preview.innerHTML = `<div class="preview-header"><span class="preview-name">${escapeHtml(row.cardName || "이름 없는 카드")}</span><span class="preview-cost">의지 ${escapeHtml(row.cost || "0")}${row.cost2 ? ` · ${escapeHtml(row.cost2)}` : ""}</span></div><div>${escapeHtml(row.description || "설명 없음")}</div><div class="preview-keywords">${escapeHtml(row.Keyword || row.Keywords || "키워드 없음")}</div><div>${active.effects.length ? active.effects.map((effect, index) => `<div class="preview-effect">effects${index + 1}: ${escapeHtml(effect)}</div>`).join("") : `<div class="preview-effect">실행 효과 없음</div>`}</div>`;
  const changedCount = state.records.filter(hasChanged).length;
  const errors = state.records.reduce((count, record) => count + validateCard(record, state.records).errors.length, 0);
  summary.innerHTML = `<strong>${changedCount}장</strong> 변경됨 · <strong>${errors}</strong>개 오류<br><small>${active.notes.join(" ") || (hasChanged(active) ? "현재 카드에 저장 전 변경이 있습니다." : "현재 카드는 원본과 같습니다.")}</small>`;
}

function render() { renderSheetFilter(); renderList(); renderEditor(); renderPreview(); }

function selectCard(id) { state.activeId = id; render(); }
function activeOrWarn() { const active = getActive(); if (!active) setStatus("먼저 카드를 선택하세요.", "error"); return active; }

function changeField(field, value) { const active = activeOrWarn(); if (!active) return; active.values[field] = value; active.notes = []; renderList(); renderEditor(); renderPreview(); }
function changeEffect(index, value) { const active = activeOrWarn(); if (!active) return; active.effects[index] = value; active.notes = []; renderList(); renderEditor(); renderPreview(); }
function swapEffect(index, next) { const active = activeOrWarn(); if (!active || next < 0 || next >= active.effects.length) return; [active.effects[index], active.effects[next]] = [active.effects[next], active.effects[index]]; render(); }

function toggleExhaust() {
  const active = activeOrWarn(); if (!active) return;
  const index = active.effects.findIndex(isRemoveThis);
  if (index >= 0) active.effects.splice(index, 1); else active.effects.push("removeThis");
  active.notes = []; render();
}

function restoreActive() {
  const active = activeOrWarn(); if (!active) return;
  active.values = clone(active.original); active.effects = Object.keys(active.original).filter(isEffectColumn).sort((a, b) => effectColumnOrder(a) - effectColumnOrder(b)).map((key) => text(active.original[key]).trim()).filter(Boolean); active.notes = []; render();
}

function newCard() {
  const sheetName = document.querySelector("#sheet-filter").value || state.records[0]?.sheetName;
  if (!sheetName) return setStatus("원본을 먼저 불러오거나 시트를 선택하세요.", "error");
  const sourceRow = Math.max(1, ...state.records.filter((record) => record.sheetName === sheetName).map((record) => record.sourceRow || 1)) + 1;
  const record = toRecord(sheetName, { Tier: "0", cardName: "새 카드", cost: "0", cost2: "", description: "", CType: "S", TType: "Enemy", User: sheetName, Keyword: "" }, sourceRow);
  record.id = `${sheetName}:new:${crypto.randomUUID()}`; record.original = {};
  state.records.push(record); state.activeId = record.id; render();
}

function tsvEscape(value) { return text(value).replace(/\t/g, " ").replace(/\r?\n/g, " "); }
function download(filename, contents, mime) { const blob = new Blob([contents], { type: mime }); const url = URL.createObjectURL(blob); const anchor = document.createElement("a"); anchor.href = url; anchor.download = filename; document.body.append(anchor); anchor.click(); anchor.remove(); URL.revokeObjectURL(url); }
function exportTsv() {
  const sheetName = document.querySelector("#sheet-filter").value || getActive()?.sheetName;
  if (!sheetName) return setStatus("내보낼 시트를 선택하세요.", "error");
  const records = state.records.filter((record) => record.sheetName === sheetName);
  const columns = [...new Set(records.flatMap((record) => Object.keys(serializeRecord(record))))];
  const rows = records.map((record) => serializeRecord(record));
  download(`${sheetName}-CardML.tsv`, [columns.join("\t"), ...rows.map((row) => columns.map((column) => tsvEscape(row[column])).join("\t"))].join("\n"), "text/tab-separated-values;charset=utf-8");
  setStatus(`${sheetName} TSV를 내보냈습니다. Sheet에 붙여 넣기 전 변경 요약을 확인하세요.`, "success");
}
function exportJson() {
  const changed = state.records.filter(hasChanged).map((record) => ({ sheetName: record.sheetName, sourceRow: record.sourceRow, values: serializeRecord(record), notes: record.notes }));
  download("CardML-changes.json", JSON.stringify({ exportedAt: new Date().toISOString(), source: state.sourceLabel, changed }, null, 2), "application/json;charset=utf-8");
  setStatus(`${changed.length}개 변경 카드를 JSON으로 내보냈습니다.`, "success");
}

function bindEvents() {
  document.querySelector("#load-remote").addEventListener("click", loadRemote);
  document.querySelector("#load-fixture").addEventListener("click", loadFixture);
  document.querySelector("#save-draft").addEventListener("click", () => saveDraft());
  document.querySelector("#export-tsv").addEventListener("click", exportTsv);
  document.querySelector("#export-json").addEventListener("click", exportJson);
  document.querySelector("#save-source").addEventListener("click", () => { const value = document.querySelector("#source-url").value.trim(); localStorage.setItem(CONFIG_KEY, value); state.sourceUrl = value; setStatus("읽기 API 주소를 이 브라우저에 저장했습니다.", "success"); });
  document.querySelector("#new-card").addEventListener("click", newCard);
  document.querySelector("#restore-card").addEventListener("click", restoreActive);
  document.querySelector("#add-effect").addEventListener("click", () => { const active = activeOrWarn(); if (!active) return; active.effects.push("damage:Target:0:HP"); render(); });
  document.querySelector("#toggle-exhaust").addEventListener("click", toggleExhaust);
  document.querySelector("#filter").addEventListener("input", renderList);
  document.querySelector("#sheet-filter").addEventListener("change", renderList);
  document.querySelector("#state-filter").addEventListener("change", renderList);
  document.querySelector("#card-list").addEventListener("click", (event) => { const target = event.target.closest("[data-select-card]"); if (target) selectCard(target.dataset.selectCard); });
  document.querySelector("#card-editor").addEventListener("input", (event) => { if (event.target.dataset.field) changeField(event.target.dataset.field, event.target.value); if (event.target.dataset.effectIndex) changeEffect(Number(event.target.dataset.effectIndex), event.target.value); });
  document.querySelector("#effects").addEventListener("click", (event) => { const index = Number(event.target.dataset.effectIndex); if (!Number.isInteger(index)) return; if (event.target.classList.contains("remove-effect")) { getActive().effects.splice(index, 1); render(); } if (event.target.classList.contains("move-effect-up")) swapEffect(index, index - 1); if (event.target.classList.contains("move-effect-down")) swapEffect(index, index + 1); });
}

async function boot() {
  try { await loadSchema(); } catch (error) { setStatus(error.message, "error"); }
  await loadDraft();
  state.sourceUrl = localStorage.getItem(CONFIG_KEY) || state.sourceUrl || CARDMAKER_DEFAULT_CONFIG.sourceUrl;
  document.querySelector("#source-url").value = state.sourceUrl;
  bindEvents(); render();
}

if (typeof document !== "undefined") boot();
