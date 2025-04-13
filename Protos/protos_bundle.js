const fs = require('fs');
const path = require('path');

// 🐍 Convertir PascalCase o camelCase a snake_case
function toSnakeCase(str) {
  return str
    .replace(/([a-z0-9])([A-Z])/g, '$1_$2')         
    .replace(/([A-Z])([A-Z][a-z])/g, '$1_$2')       
    .toLowerCase();
}

// 🔍 Función recursiva para encontrar archivos .proto
function findProtoFiles(dir, filterFn, found = []) {
  const entries = fs.readdirSync(dir, { withFileTypes: true });
  for (const entry of entries) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      findProtoFiles(fullPath, filterFn, found);
    } else if (entry.isFile() && entry.name.endsWith('.proto') && filterFn(entry.name)) {
      found.push(fullPath);
    }
  }
  return found;
}

// 📄 Extraer package y mensajes de un archivo .proto
function extractProtoData(protoFile) {
  const content = fs.readFileSync(protoFile, 'utf8');
  const pkgRegex = /package\s+([\w\.]+)\s*;/;
  const pkgMatch = content.match(pkgRegex);
  const pkg = pkgMatch ? pkgMatch[1] : null;

  const messageRegex = /message\s+(\w+)\s*\{/g;
  const messages = [];
  let match;
  while ((match = messageRegex.exec(content)) !== null) {
    messages.push(match[1]);
  }
  return { pkg, messages };
}

// 🛠️ Generar el mensaje raíz con oneof usando el tipo totalmente calificado
function generateRootMessage(items, rootName) {
  let index = 1;
  const oneofBody = items.map(({ pkg, message }) => {
    const fieldName = toSnakeCase(message);
    const fullType = pkg ? `${pkg}.${message}` : message;
    return `    ${fullType} ${fieldName} = ${index++};`;
  }).join('\n');

  return `message ${rootName} {\n  oneof payload {\n${oneofBody}\n  }\n}`;
}

// 📄 Generar el contenido final del archivo .proto
function generateProtoFile(outputPath, imports, rootMessage, pkg) {
  const content = `
syntax = "proto3";
package ${pkg};

${imports.map((imp) => `import "${imp}";`).join('\n')}

${rootMessage}
  `.trim();

  fs.writeFileSync(outputPath, content, 'utf8');
  console.log(`✅ Generado: ${outputPath}`);
}

// 🧰 Main
const baseDir = __dirname;
const clientFiles = findProtoFiles(baseDir, (name) => name.endsWith('.client.proto'));
const serverFiles = findProtoFiles(baseDir, (name) => name.endsWith('.server.proto'));

function processGroup(files, outputFile, rootName, outPkg) {
  const allItems = [];
  const relativeImports = [];

  for (const file of files) {
    const relPath = path.relative(path.dirname(outputFile), file).replace(/\\/g, '/');
    relativeImports.push(relPath);

    const { pkg, messages } = extractProtoData(file);
    if (!pkg) {
      console.warn(`⚠️  No se encontró package en ${file}`);
    }
    messages.forEach(msg => allItems.push({ pkg, message: msg }));
  }

  const rootMessage = generateRootMessage(allItems, rootName);
  generateProtoFile(outputFile, relativeImports, rootMessage, outPkg);
}

// Generar archivos raíz
const clientOutput = path.join(baseDir, 'client_messages.generated.proto');
const serverOutput = path.join(baseDir, 'server_messages.generated.proto');

processGroup(clientFiles, clientOutput, 'ClientMessage', 'ventus.client');
processGroup(serverFiles, serverOutput, 'ServerMessage', 'ventus.server');
