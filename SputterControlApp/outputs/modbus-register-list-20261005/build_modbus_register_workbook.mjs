import fs from "node:fs/promises";
import path from "node:path";
import { SpreadsheetFile, Workbook } from "@oai/artifact-tool";

const rootDir = path.resolve(process.cwd());
const outputDir = path.join(rootDir, "outputs", "modbus-register-list-20261005");
const outputPath = path.join(outputDir, "SputterControlApp_Modbus_Adres_Listesi.xlsx");
const registerMapPath = path.join(rootDir, "Core", "Registers", "RegisterMap.cs");

const excludedFiles = new Set([
  "Forms/MainForm.AnalogTurbo.cs",
  "Forms/MainForm.StartControls.cs",
  "Forms/MaterialEditForm.cs",
  "Forms/MaterialEditForm.Designer.cs",
  "Forms/ProcessEditForm.cs",
  "Forms/ProcessEditForm.Designer.cs",
]);

async function listFiles(dir) {
  const entries = await fs.readdir(dir, { withFileTypes: true });
  const result = [];
  for (const entry of entries) {
    if (["bin", "obj", "artifacts", "outputs", ".git", "UI"].includes(entry.name)) continue;
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) result.push(...await listFiles(full));
    else if (entry.isFile() && entry.name.endsWith(".cs")) result.push(full);
  }
  return result;
}

function rel(file) {
  return path.relative(rootDir, file).replaceAll("\\", "/");
}

function stripCommentsKeepLines(text) {
  const withoutBlocks = text.replace(/\/\*[\s\S]*?\*\//g, match => match.replace(/[^\n]/g, " "));
  return withoutBlocks.split(/\r?\n/).map(line => {
    const index = line.indexOf("//");
    return index >= 0 ? line.slice(0, index) : line;
  }).join("\n");
}

function parseRegisterMap(text) {
  const rows = [];
  const lines = text.split(/\r?\n/);
  for (let index = 0; index < lines.length; index += 1) {
    const match = lines[index].match(/^\s*([A-Za-z_]\w*)\s*=\s*(\d+)\s*,?\s*(?:\/\/\s*(.*))?$/);
    if (!match) continue;
    const name = match[1];
    const address = Number(match[2]);
    const comment = (match[3] ?? "").trim();
    const lower = `${name} ${comment}`.toLowerCase();
    let dataType = "Int16";
    let registerCount = 1;
    if (lower.includes("int32")) {
      dataType = "Int32 (CDAB)";
      registerCount = 2;
    } else if (lower.includes("2 reg") || /actualflow|readsetpoint|writesetpoint|readfullscale|writefullscale|tordeger|analogvoltaj|thickness|rate\d|life\d|z_ratio|density|tooling|zerothickness/i.test(name)) {
      dataType = "Float32 (CDAB)";
      registerCount = 2;
    }
    rows.push({ name, address, comment, dataType, registerCount, definitionLine: index + 1 });
  }
  return rows.sort((a, b) => a.address - b.address || a.name.localeCompare(b.name));
}

function classifyOccurrence(fileRel, line, name) {
  const s = line.toLowerCase();
  if (/writeaddress|getwriteaddr|writesingleregisterasync|writemultipleregistersasync|writeu16andconfirmasync|writefloat32|senddevicecommand|startsource|stopsource/.test(s)) return "Yazma";
  if (/readaddress|getreadaddr|readholdingregistersasync|readf32|readi16|pollsingledevicestatus|waitregisterequals/.test(s)) return "Okuma";
  if (/Polling\.cs|SnapshotRead\.cs|GateValveState\.cs|EmergencyStop\.cs|RF1Sequence\.cs|DCSequence\.cs/.test(fileRel)) return "Okuma";
  if (/QcmSensors\.cs/.test(fileRel) && name.endsWith("_Control_Command")) return "Yazma";
  return "Referans";
}

const registerMapText = await fs.readFile(registerMapPath, "utf8");
const registers = parseRegisterMap(registerMapText);
const byName = new Map(registers.map(r => [r.name, r]));
const compiledFiles = (await listFiles(rootDir)).filter(file => !excludedFiles.has(rel(file)));
const usageRows = [];
const operationsByAddress = new Map();
const sourcesByAddress = new Map();

function addAddressUse(address, operation, source) {
  if (!operationsByAddress.has(address)) operationsByAddress.set(address, new Set());
  if (!sourcesByAddress.has(address)) sourcesByAddress.set(address, new Set());
  operationsByAddress.get(address).add(operation);
  sourcesByAddress.get(address).add(source);
}

function addLogicalUse(register, operation, source) {
  for (let offset = 0; offset < register.registerCount; offset += 1) {
    addAddressUse(register.address + offset, operation, source);
  }
}

for (const file of compiledFiles) {
  const fileRel = rel(file);
  const original = await fs.readFile(file, "utf8");
  const stripped = stripCommentsKeepLines(original);
  const originalLines = original.split(/\r?\n/);
  const strippedLines = stripped.split(/\r?\n/);

  for (let i = 0; i < strippedLines.length; i += 1) {
    const line = strippedLines[i];
    for (const match of line.matchAll(/RegisterId\.([A-Za-z_]\w*)/g)) {
      const register = byName.get(match[1]);
      if (!register) continue;
      let operation = classifyOccurrence(fileRel, line, register.name);
      const tupleMatch = line.match(/=>\s*\(\(ushort\)RegisterId\.([A-Za-z_]\w*),\s*\(ushort\)RegisterId\.([A-Za-z_]\w*)\)/);
      if (tupleMatch) operation = register.name === tupleMatch[1] ? "Yazma" : "Okuma";
      const source = `${fileRel}:${i + 1}`;
      addLogicalUse(register, operation, source);
      usageRows.push({
        address: register.address,
        registerName: register.name,
        operation,
        kind: "RegisterId referansı",
        file: fileRel,
        line: i + 1,
        code: originalLines[i].trim().replace(/\s+/g, " ").slice(0, 220),
      });
    }
  }

  for (const match of stripped.matchAll(/ReadHoldingRegistersAsync\s*\(\s*(?:\(ushort\))?RegisterId\.([A-Za-z_]\w*)\s*,\s*(\d+)/g)) {
    const register = byName.get(match[1]);
    if (!register) continue;
    const count = Number(match[2]);
    const line = stripped.slice(0, match.index).split("\n").length;
    for (let address = register.address; address < register.address + count; address += 1) {
      addAddressUse(address, "Okuma", `${fileRel}:${line} blok (${count})`);
    }
    usageRows.push({
      address: register.address,
      registerName: `${register.name} … D${register.address + count - 1}`,
      operation: "Okuma",
      kind: `Blok okuma (${count} register)`,
      file: fileRel,
      line,
      code: `D${register.address}–D${register.address + count - 1}`,
    });
  }

  for (const match of stripped.matchAll(/const\s+ushort\s+baseAddr\s*=\s*\(ushort\)RegisterId\.([A-Za-z_]\w*)\s*;[\s\S]{0,240}?const\s+ushort\s+count\s*=\s*(\d+)\s*;/g)) {
    const register = byName.get(match[1]);
    if (!register) continue;
    const count = Number(match[2]);
    const line = stripped.slice(0, match.index).split("\n").length;
    for (let address = register.address; address < register.address + count; address += 1) {
      addAddressUse(address, "Okuma", `${fileRel}:${line} blok (${count})`);
    }
    usageRows.push({
      address: register.address,
      registerName: `${register.name} … D${register.address + count - 1}`,
      operation: "Okuma",
      kind: `Değişkenle blok okuma (${count} register)`,
      file: fileRel,
      line,
      code: `D${register.address}–D${register.address + count - 1}`,
    });
  }

  for (const match of stripped.matchAll(/BindOnOffButton\s*\(\s*[^,]+,\s*(\d+)\s*,\s*(\d+)\s*,/g)) {
    const writeAddress = Number(match[1]);
    const readAddress = Number(match[2]);
    const line = stripped.slice(0, match.index).split("\n").length;
    addAddressUse(writeAddress, "Yazma", `${fileRel}:${line} ham adres`);
    addAddressUse(readAddress, "Okuma", `${fileRel}:${line} ham adres`);
    usageRows.push({
      address: writeAddress,
      registerName: byName.get([...byName.keys()].find(name => byName.get(name).address === writeAddress))?.name ?? "Harita dışı",
      operation: writeAddress === readAddress ? "Okuma + Yazma" : "Yazma",
      kind: "Ham adresli buton bağlantısı",
      file: fileRel,
      line,
      code: originalLines[line - 1].trim().replace(/\s+/g, " ").slice(0, 220),
    });
  }
}

const definedByPhysicalAddress = new Map();
for (const register of registers) {
  for (let offset = 0; offset < register.registerCount; offset += 1) {
    definedByPhysicalAddress.set(register.address + offset, { register, offset });
  }
}

const usedAddresses = [...operationsByAddress.keys()].sort((a, b) => a - b).map(address => {
  const mapped = definedByPhysicalAddress.get(address);
  const operations = [...operationsByAddress.get(address)].filter(x => x !== "Referans");
  const allOperations = operations.length ? operations : [...operationsByAddress.get(address)];
  const access = allOperations.includes("Okuma") && allOperations.includes("Yazma")
    ? "Okuma + Yazma"
    : allOperations.sort().join(" + ");
  return {
    address,
    dAddress: `D${address}`,
    holding: 40001 + address,
    registerName: mapped ? mapped.register.name : "Haritada tanımsız",
    wordRole: mapped ? (mapped.register.registerCount === 1 ? "Tek word" : `${mapped.offset + 1}/${mapped.register.registerCount}. word`) : "Blok içinde",
    dataType: mapped?.register.dataType ?? "Belirtilmemiş",
    access,
    sourceCount: sourcesByAddress.get(address).size,
    sources: [...sourcesByAddress.get(address)].sort().join("; "),
    note: mapped?.register.comment ?? "RegisterMap.cs içinde ayrı bir ad yok; blok erişimi kapsamında.",
  };
});

const referencesByName = new Map();
for (const usage of usageRows) {
  if (!byName.has(usage.registerName)) continue;
  if (!referencesByName.has(usage.registerName)) referencesByName.set(usage.registerName, []);
  referencesByName.get(usage.registerName).push(usage);
}

const mapRows = registers.map(register => {
  const refs = referencesByName.get(register.name) ?? [];
  const ops = new Set();
  const files = new Set();
  for (let address = register.address; address < register.address + register.registerCount; address += 1) {
    for (const op of operationsByAddress.get(address) ?? []) if (op !== "Referans") ops.add(op);
    for (const source of sourcesByAddress.get(address) ?? []) files.add(source.split(":")[0]);
  }
  const access = ops.has("Okuma") && ops.has("Yazma") ? "Okuma + Yazma" : [...ops].sort().join(" + ");
  return {
    start: register.address,
    range: register.registerCount === 1 ? `D${register.address}` : `D${register.address}–D${register.address + register.registerCount - 1}`,
    holding: register.registerCount === 1 ? `${40001 + register.address}` : `${40001 + register.address}–${40000 + register.address + register.registerCount}`,
    name: register.name,
    type: register.dataType,
    count: register.registerCount,
    used: [...Array(register.registerCount).keys()].some(offset => operationsByAddress.has(register.address + offset)) ? "Kullanılıyor" : "Tanımlı, kullanılmıyor",
    access: access || "—",
    files: [...files].sort().join("; ") || "—",
    comment: register.comment || "—",
    definition: `Core/Registers/RegisterMap.cs:${register.definitionLine}`,
  };
});

usageRows.sort((a, b) => a.address - b.address || a.file.localeCompare(b.file) || a.line - b.line);

const workbook = Workbook.create();
const usedSheet = workbook.worksheets.add("Kullanılan Adresler");
const mapSheet = workbook.worksheets.add("Register Haritası");
const usageSheet = workbook.worksheets.add("Kod Erişimleri");

const fontName = "Aptos";
const navy = "#17365D";
const blue = "#D9EAF7";
const light = "#F4F7FA";
const green = "#E2F0D9";
const amber = "#FFF2CC";
const red = "#FCE4D6";
const border = "#C8D2DC";

function styleTitle(sheet, rangeAddress, title, subtitleAddress, subtitle) {
  sheet.showGridLines = false;
  sheet.mergeCells(rangeAddress);
  const titleRange = sheet.getRange(rangeAddress);
  titleRange.values = [[title]];
  titleRange.format = {
    fill: navy,
    font: { name: fontName, size: 18, bold: true, color: "#FFFFFF" },
    verticalAlignment: "center",
  };
  titleRange.format.rowHeight = 34;
  sheet.getRange(subtitleAddress).values = [[subtitle]];
  sheet.getRange(subtitleAddress).format = {
    font: { name: fontName, size: 10, italic: true, color: "#44546A" },
    wrapText: true,
  };
}

function applyHeader(range) {
  range.format = {
    fill: navy,
    font: { name: fontName, bold: true, color: "#FFFFFF" },
    verticalAlignment: "center",
    wrapText: true,
    borders: { preset: "all", style: "thin", color: border },
  };
  range.format.rowHeight = 30;
}

function applyBody(range) {
  range.format = {
    font: { name: fontName, size: 10, color: "#1F2937" },
    verticalAlignment: "top",
    wrapText: true,
    borders: { preset: "all", style: "thin", color: border },
  };
}

styleTitle(
  usedSheet,
  "A1:J1",
  "SputterControlApp – Kullanılan Modbus Adresleri",
  "A2",
  "Kaynak: derlenen C# dosyaları ve Core/Registers/RegisterMap.cs. D adresleri sıfır tabanlıdır; 4xxxx gösterimi 40001 + adres olarak verilmiştir."
);
usedSheet.mergeCells("A2:J2");
usedSheet.getRange("A4:H5").values = [
  ["Kullanılan fiziksel adres", usedAddresses.length, "Tanımlı register", registers.length, "Kullanılan tanım", mapRows.filter(r => r.used === "Kullanılıyor").length, "Tanımsız blok adresi", usedAddresses.filter(r => r.registerName === "Haritada tanımsız").length],
  ["En düşük adres", Math.min(...usedAddresses.map(r => r.address)), "En yüksek adres", Math.max(...usedAddresses.map(r => r.address)), "Okunan", usedAddresses.filter(r => r.access.includes("Okuma")).length, "Yazılan", usedAddresses.filter(r => r.access.includes("Yazma")).length],
];
usedSheet.getRange("A4:H5").format = {
  fill: blue,
  font: { name: fontName, size: 11, bold: true, color: navy },
  borders: { preset: "all", style: "thin", color: border },
  verticalAlignment: "center",
};
usedSheet.getRange("A7:J7").values = [["D Adresi", "Decimal", "Holding (4xxxx)", "Register Adı", "Word Rolü", "Veri Tipi", "Erişim", "Kaynak Sayısı", "Kod Konumları", "RegisterMap Notu"]];
applyHeader(usedSheet.getRange("A7:J7"));
const usedValues = usedAddresses.map(row => [row.dAddress, row.address, row.holding, row.registerName, row.wordRole, row.dataType, row.access, row.sourceCount, row.sources, row.note]);
if (usedValues.length) {
  usedSheet.getRangeByIndexes(7, 0, usedValues.length, 10).values = usedValues;
  applyBody(usedSheet.getRangeByIndexes(7, 0, usedValues.length, 10));
  const usedTable = usedSheet.tables.add(`A7:J${7 + usedValues.length}`, true, "UsedModbusAddresses");
  usedTable.style = "TableStyleMedium2";
}
usedSheet.freezePanes.freezeRows(7);
usedSheet.getRange("A:A").format.columnWidth = 11;
usedSheet.getRange("B:C").format.columnWidth = 13;
usedSheet.getRange("D:D").format.columnWidth = 28;
usedSheet.getRange("E:G").format.columnWidth = 16;
usedSheet.getRange("H:H").format.columnWidth = 12;
usedSheet.getRange("I:I").format.columnWidth = 46;
usedSheet.getRange("J:J").format.columnWidth = 38;
usedSheet.getRange(`G8:G${7 + usedValues.length}`).conditionalFormats.add("containsText", { text: "Okuma + Yazma", format: { fill: green, font: { bold: true, color: "#375623" } } });
usedSheet.getRange(`G8:G${7 + usedValues.length}`).conditionalFormats.add("containsText", { text: "Referans", format: { fill: amber } });

styleTitle(
  mapSheet,
  "A1:J1",
  "Register Haritası ve Kullanım Durumu",
  "A2",
  "RegisterMap.cs içindeki tüm tanımlar. 'Tanımlı, kullanılmıyor' satırları derlenen kaynaklarda sabit erişim bulunmayan kayıtları gösterir; Manual Read ekranındaki kullanıcı seçimi dinamik olduğu için bu sınıflamaya dahil değildir."
);
mapSheet.mergeCells("A2:J2");
mapSheet.getRange("A4:J4").values = [["D Aralığı", "Başlangıç", "Holding (4xxxx)", "Register Adı", "Veri Tipi", "Word", "Durum", "Erişim", "Kullanılan Dosyalar", "Tanım / Not"]];
applyHeader(mapSheet.getRange("A4:J4"));
const mapValues = mapRows.map(row => [row.range, row.start, row.holding, row.name, row.type, row.count, row.used, row.access, row.files, `${row.comment} | ${row.definition}`]);
mapSheet.getRangeByIndexes(4, 0, mapValues.length, 10).values = mapValues;
applyBody(mapSheet.getRangeByIndexes(4, 0, mapValues.length, 10));
const mapTable = mapSheet.tables.add(`A4:J${4 + mapValues.length}`, true, "RegisterMapTable");
mapTable.style = "TableStyleMedium2";
mapSheet.freezePanes.freezeRows(4);
mapSheet.getRange("A:C").format.columnWidth = 15;
mapSheet.getRange("D:D").format.columnWidth = 30;
mapSheet.getRange("E:H").format.columnWidth = 17;
mapSheet.getRange("I:I").format.columnWidth = 36;
mapSheet.getRange("J:J").format.columnWidth = 48;
mapSheet.getRange(`G5:G${4 + mapValues.length}`).conditionalFormats.add("containsText", { text: "Kullanılıyor", format: { fill: green, font: { bold: true, color: "#375623" } } });
mapSheet.getRange(`G5:G${4 + mapValues.length}`).conditionalFormats.add("containsText", { text: "kullanılmıyor", format: { fill: red, font: { color: "#9C0006" } } });

styleTitle(
  usageSheet,
  "A1:G1",
  "Kod Erişimleri",
  "A2",
  "Her RegisterId referansı, sabit blok okuması ve ham adresli buton bağlantısı kaynak dosya/satır bilgisiyle listelenmiştir. 'Referans' sınıfı, yönü aynı satırdan kesin çıkarılamayan dolaylı kullanımdır."
);
usageSheet.mergeCells("A2:G2");
usageSheet.getRange("A4:G4").values = [["D Adresi", "Register / Aralık", "İşlem", "Kullanım Türü", "Dosya", "Satır", "Kod / Aralık"]];
applyHeader(usageSheet.getRange("A4:G4"));
const usageValues = usageRows.map(row => {
  const codeText = row.code.startsWith("=") ? `'${row.code}` : row.code;
  return [`D${row.address}`, row.registerName, row.operation, row.kind, row.file, row.line, codeText];
});
usageSheet.getRangeByIndexes(4, 0, usageValues.length, 7).values = usageValues;
applyBody(usageSheet.getRangeByIndexes(4, 0, usageValues.length, 7));
const usageTable = usageSheet.tables.add(`A4:G${4 + usageValues.length}`, true, "CodeAccessTable");
usageTable.style = "TableStyleMedium2";
usageSheet.freezePanes.freezeRows(4);
usageSheet.getRange("A:A").format.columnWidth = 11;
usageSheet.getRange("B:B").format.columnWidth = 34;
usageSheet.getRange("C:D").format.columnWidth = 22;
usageSheet.getRange("E:E").format.columnWidth = 38;
usageSheet.getRange("F:F").format.columnWidth = 10;
usageSheet.getRange("G:G").format.columnWidth = 66;
usageSheet.getRange(`C5:C${4 + usageValues.length}`).conditionalFormats.add("containsText", { text: "Okuma + Yazma", format: { fill: green } });
usageSheet.getRange(`C5:C${4 + usageValues.length}`).conditionalFormats.add("containsText", { text: "Referans", format: { fill: amber } });

for (const sheet of [usedSheet, mapSheet, usageSheet]) {
  const used = sheet.getUsedRange();
  used.format.font = { name: fontName, size: 10 };
  used.format.verticalAlignment = "top";
}

workbook.recalculate();

const inspections = [];
inspections.push((await workbook.inspect({ kind: "sheet", include: "id,name", maxChars: 3000 })).ndjson);
inspections.push((await workbook.inspect({ kind: "region", sheetId: "Kullanılan Adresler", range: "A1:J20", maxChars: 7000 })).ndjson);
inspections.push((await workbook.inspect({ kind: "region", sheetId: "Register Haritası", range: "A1:J18", maxChars: 7000 })).ndjson);
inspections.push((await workbook.inspect({ kind: "region", sheetId: "Kod Erişimleri", range: "A1:G18", maxChars: 7000 })).ndjson);
inspections.push((await workbook.inspect({ kind: "match", searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A", options: { useRegex: true, maxResults: 100 }, maxChars: 5000 })).ndjson);
await fs.writeFile(path.join(outputDir, "verification.txt"), inspections.join("\n\n"), "utf8");

for (const sheetName of ["Kullanılan Adresler", "Register Haritası", "Kod Erişimleri"]) {
  const preview = await workbook.render({ sheetName, autoCrop: "all", scale: 0.7, format: "png" });
  const safeName = sheetName.replaceAll(" ", "_").replaceAll("ı", "i").replaceAll("ş", "s");
  await fs.writeFile(path.join(outputDir, `preview_${safeName}.png`), new Uint8Array(await preview.arrayBuffer()));
}

const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(outputPath);

console.log(JSON.stringify({
  outputPath,
  compiledFileCount: compiledFiles.length,
  mappedRegisterCount: registers.length,
  usedPhysicalAddressCount: usedAddresses.length,
  usedMappedRegisterCount: mapRows.filter(r => r.used === "Kullanılıyor").length,
  usageRowCount: usageRows.length,
  sheetNames: ["Kullanılan Adresler", "Register Haritası", "Kod Erişimleri"],
}, null, 2));
