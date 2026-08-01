import test from "node:test";
import assert from "node:assert/strict";
import { readFile } from "node:fs/promises";
import { migratePostUseMarkers, parseEffect, serializeRecord, validateCard } from "../assets/card-maker.js";

const schema = JSON.parse(await readFile(new URL("../schemas/cardml-schema.json", import.meta.url)));

function makeRecord(values, effects = []) {
  return { id: "Prote:2:테스트", sheetName: "Prote", sourceRow: 2, original: { ...values }, values: { ...values }, effects: [...effects], notes: [] };
}

test("removeThis is a valid effect-only exhaustion marker", () => {
  const parsed = parseEffect("removeThis", schema);
  assert.equal(parsed.error, undefined);
  assert.equal(parsed.action, "removethis");
});

test("legacy description and keyword markers migrate into removeThis", () => {
  const record = makeRecord({ Tier: "1", cardName: "소모 카드", cost: "1", description: "보호 8 [소멸]", CType: "S", TType: "Ally", Keyword: "신속, 소멸" }, ["shield:User:8"]);
  assert.equal(migratePostUseMarkers(record), true);
  assert.equal(record.values.description, "보호 8");
  assert.equal(record.values.Keyword, "신속");
  assert.deepEqual(record.effects, ["shield:User:8", "removeThis"]);
  assert.equal(serializeRecord(record).effects2, "removeThis");
});

test("description [소멸] is rejected until it is moved to effects", () => {
  const record = makeRecord({ Tier: "0", cardName: "오류 카드", cost: "1", description: "피해 5 [소멸]", CType: "A", TType: "Enemy", Keyword: "" }, ["damage:Target:5:HP"]);
  const validation = validateCard(record, [], schema);
  assert.ok(validation.errors.some((message) => message.includes("description")));
});

test("effect parameter count follows the published schema", () => {
  assert.match(parseEffect("damage:Target", schema).error, /값 4개/);
  assert.equal(parseEffect("damage:Target:5:HP", schema).error, undefined);
});
