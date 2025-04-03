# 1. Ir a la carpeta del submódulo y actualizarlo
Write-Host "🔄 Actualizando submódulo..."
cd protos
git pull origin main
cd ..

# 2. Crear la carpeta de salida si no existe
$OUTPUT_DIR = "GeneratedProtos"
if (!(Test-Path $OUTPUT_DIR)) {
    New-Item -ItemType Directory -Path $OUTPUT_DIR
}

# 3. Generar los archivos C#
Write-Host "⚙️ Generando archivos .cs desde .proto..."
protoc --proto_path=protos --csharp_out=$OUTPUT_DIR protos/**/*.proto

Write-Host "✅ Proceso completado!"
