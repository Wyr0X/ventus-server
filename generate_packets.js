// generate_server_packets.js
const fs = require("fs");
const path = require("path");

const PROTOS_DIR = path.join(__dirname, "protos");
const GENERATED_DIR = path.join(__dirname, "generated");
const OUTPUT_FILE = path.join(GENERATED_DIR, "ServerPackets.cs");

function findProtoFiles(dir, found = []) {
    const entries = fs.readdirSync(dir, { withFileTypes: true });
    for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);
        if (entry.isDirectory()) {
            findProtoFiles(fullPath, found);
        } else if (entry.isFile() && entry.name.endsWith(".server.proto")) {
            found.push(fullPath);
        }
    }
    return found;
}

function extractMessages(file) {
    const content = fs.readFileSync(file, "utf8");
    const messages = [];
    const regex = /message\s+(\w+)\s*\{/g;
    let match;
    while ((match = regex.exec(content)) !== null) {
        messages.push(match[1]);
    }
    return messages.map((msg) => ({ file, message: msg }));
}

function toNamespace(filePath) {
    // Suponiendo que el path en protos refleja el namespace C#
    const relative = path
        .relative(PROTOS_DIR, filePath)
        .replace(/\.proto$/, "");
    return relative.split(path.sep).map(capitalize).join(".");
}

function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function generateCSharpFile(messages) {
    const enumEntries = messages.map(
        ({ message }, i) => `        ${message} = ${i + 1},`
    );

    const mapEntries = messages.map(({ message, file }) => {
        const ns = toNamespace(file);
        return `            { ServerPacket.${message}, new ${ns}.${message}().Descriptor.Parser },`;
    });

    const usings = new Set(
        messages.map(({ file }) => `using ${toNamespace(file)};`)
    );

    const content = `// 🛑 Archivo generado automáticamente. No editar.
using System.Collections.Generic;
using Google.Protobuf;
${[...usings].join("\n")}

namespace Ventus.Network.Packets
{
    public enum ServerPacket
    {
${enumEntries.join("\n")}
    }

    public static class ServerPacketDecoder
    {
        public static readonly Dictionary<ServerPacket, MessageParser> Parsers = new()
        {
${mapEntries.join("\n")}
        };
    }
}
`;

    fs.writeFileSync(OUTPUT_FILE, content, "utf8");
    console.log(`✅ Archivo generado: ServerPackets.cs`);
}

function run() {
    const protoFiles = findProtoFiles(PROTOS_DIR);
    const messages = protoFiles.flatMap(extractMessages);

    if (!fs.existsSync(GENERATED_DIR)) {
        fs.mkdirSync(GENERATED_DIR, { recursive: true });
    }

    generateCSharpFile(messages);
}

run();
