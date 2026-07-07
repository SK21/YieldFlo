# Minimal serial terminal for SensorSim.
# Usage:  powershell -ExecutionPolicy Bypass -File simterm.ps1 -Port COM9
# Type commands (d 50, f 4, s, g, b, c, ?) and press Enter. 'q' quits.
param([string]$Port = "COM9")

$sp = New-Object System.IO.Ports.SerialPort $Port, 115200, 'None', 8, 'One'
$sp.NewLine = "`n"
$sp.ReadTimeout = 200
# DTR/RTS left deasserted on purpose: asserting them can hold a D1 mini's
# auto-program circuit in reset. The sketch keeps running across open/close.
try { $sp.Open() }
catch {
    Write-Host "Cannot open $Port - close any serial monitor that is using it."
    Write-Host "Available ports: $([System.IO.Ports.SerialPort]::GetPortNames() -join ', ')"
    exit 1
}

Start-Sleep -Milliseconds 500
$banner = $sp.ReadExisting()
if ($banner) { Write-Host $banner -NoNewline }
Write-Host "Connected to $Port. Commands: d <pct>, f <hz>, p, s, g, b, c, ?   (q quits)"

# Ask for current status so we know the link works both ways
$sp.WriteLine("?")
Start-Sleep -Milliseconds 300
Write-Host $sp.ReadExisting() -NoNewline

while ($true) {
    $cmd = Read-Host ">"
    if ($cmd -eq 'q') { break }
    if ($cmd.Trim().Length -eq 0) { continue }
    $sp.WriteLine($cmd)
    Start-Sleep -Milliseconds 300
    Write-Host $sp.ReadExisting() -NoNewline
}
$sp.Close()
