################################################
# Automated certificate requests using WinAcme #
################################################
#
# 1. Copy heracles.exe to BOTH C:\WINDOWS\System32\ AND C:\WINDOWS\SysWOW64\
#
#    Make sure that the environment variables are set and it's working properly. If
#    you don't want to set the environment variables permanently, note that you can
#    set temporary environment variables in PowerShell like so:
#
#    [System.Environment]::SetEnvironmentVariable('HYDRA_URI', 'https://the/production/hydra/uri')
#    [System.Environment]::SetEnvironmentVariable('HYDRA_TOKEN', 'unit/user:somevalidtoken')
#
# 2. Download Win-Acme from https://www.win-acme.com/ and unpack it
#
# 3. Copy WinAcme.ps1 into the win-acme folder
#
# 4. Open a PowerShell with Adminitrative privileges
#
# 5. "cd" into the win-acme folder
#
# 6. Unblock-File WinAcme.ps1
#
# 7. .\wacs.exe --validationmode dns-01 --validation script --dnscreatescript \full\path\to\WinAcme.ps1 --dnsdeletescript \full\path\to\WinAcme.ps1

param([string]$Mode=$(throw "Mode not specified"),
      [string]$HostRecord=$(throw "HostRecord not specified"),
      [string]$TxtRecord=$(throw "TxtRecord not specified"),
      [string]$TxtContent=$(throw "TxtContent not specified"))

if (! (Test-Path C:\WINDOWS\SysWOW64\heracles.exe))
{
    throw "heracles.exe is not installed in C:\WINDOWS\SysWOW64\"
}

$HeraclesStdin = "[{`"comment`"`:`"WinAcme`"`,`"content`"`:`"$TxtContent`"`,`"hostname`"`:`"$TxtRecord.`"`,`"type`"`:`"TXT`"}]"
$HeraclesStdin | Out-File ".\hydra_add.txt"

Function Check-Panic
{
    param ([bool]$Status=$(throw "Status not specified"),
           [string]$Message=$(throw "Message not specified"))
    if (! $Status)
    {
        throw $Message
    }
}

Function Acme-TxtRecordExists
{
    $_ = Resolve-DnsName $TxtRecord -Type TXT 2>$null
    return $?
}

Function Acme-TxtRecordContents
{
    if (! (Acme-TxtRecordExists))
    {
        throw "Cannot get the contents of a non-existant record"
    }
    $record = Resolve-DnsName $TxtRecord -Type TXT
    return $record.strings
}

Function Do-Check
{
    Start-Sleep -Seconds 60
    if (! (Acme-TxtRecordExists))
    {
        Write-Host "Record not yet in DNS"
        Do-Check
    }
    if (! ((Acme-TxtRecordContents) -eq $TxtContent) )
    {
        Write-Host "$(Acme-TxtRecordContents) -ne $TxtContent"
        Do-Check
    }
    return
}

Function Do-Create
{
    if ((Acme-TxtRecordExists))
    {
        throw "Stale ACME record for host found in DNS"
    }
    Get-Content "hydra_add.txt " | & "heracles.exe" "add"
    Check-Panic -Status $? -Message "Hydra DNS change failed"
    Do-Check
}

Function Do-Delete
{
    $TxtRecord | & "heracles.exe" "search" | Out-File "hydra_delete.txt"
    $SelectedChar = Get-Content "hydra_delete.txt" | Select-String -Pattern "{"
    if ($SelectedChar.Matches.Count -ne 1)
    {
        throw "Attempted to delete multiple Hydra records"
    }
    Get-Content "hydra_delete.txt " | & "heracles.exe" "delete"
}

$_ = Resolve-DnsName $HostRecord 2>$null
Check-Panic -Status $? -Message "$HostRecord is not a valid DNS entry"

switch ($Mode)
{
    "create" { Do-Create }
    "delete" { Do-Delete } 
    default  { throw "Unknown mode $Mode" }
}
