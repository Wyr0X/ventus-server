const fs = require("fs");
const path = require("path");

const PROTOS_DIR = path.join(__dirname, "protos");
const GENERATED_DIR = path.join(__dirname, "generated");

const CONFIGS = [
    {
        suffix: ".server.proto",
        outputFile: path.join(GENERATED_DIR, "ServerPackets.cs"),
        enumName: "ServerPacket",
        className: "ServerPacketDecoder"
    },
    {
        suffix: ".client.proto",
        outputFile: path.join(GENERATED_DIR, "ClientPackets.cs"),
        enumName: "ClientPacket",
        className: "ClientPacketDecoder"
    }
];

function findProtoFiles(dir, suffix, found = []) {
    const entries = fs.readdirSync(dir, { withFileTypes: true });
    for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);
        if (entry.isDirectory()) {
            findProtoFiles(fullPath, suffix, found);
        } else if (entry.isFile() && entry.name.endsWith(suffix)) {
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

function generateCSharpFile(messages, enumName, className, outputPath) {
    const enumEntries = messages.map(({ message }, i) => `        ${message} = ${i + 1},`);
    const mapEntries = messages.map(({ message }) =>
        `            { ${enumName}.${message}, ${message}.Descriptor.Parser },`
    );

    const content = `// 🛑 Archivo generado automáticamente. No editar.
using Google.Protobuf;

namespace Ventus.Network.Packets
{
    public enum ${enumName}
    {
${enumEntries.join("\n")}
    }

    public static class ${className}
    {
        public static readonly Dictionary<${enumName}, MessageParser> Parsers = new()
        {
${mapEntries.join("\n")}
        };
    }
}
`;

    fs.writeFileSync(outputPath, content, "utf8");
    console.log(`✅ Archivo generado: ${path.basename(outputPath)}`);
}

function run() {
    if (!fs.existsSync(GENERATED_DIR)) {
        fs.mkdirSync(GENERATED_DIR, { recursive: true });
    }

    for (const config of CONFIGS) {
        const protoFiles = findProtoFiles(PROTOS_DIR, config.suffix);
        const messages = protoFiles.flatMap(extractMessages);
        generateCSharpFile(messages, config.enumName, config.className, config.outputFile);
    }
}

run();
