<#
.SYNOPSIS
  Win-ACME DNS-01 hook for Heracles/Hydra (creates/deletes TXT records).

.USAGE
  Called by win-acme as:
    create <Identifier> <RecordName> <Token>
    delete <Identifier> <RecordName> <Token>

.REQUIRES
  - heracles.exe on PATH (or adjust $HeraclesExe)
  - HYDRA_URI and HYDRA_TOKEN present in environment (e.g. via bws run)
#>

param(
  [Parameter(Mandatory=$true)][string]$Operation,   # create | delete
  [Parameter(Mandatory=$true)][string]$Identifier,  # e.g. imm-dc5.imm.ox.ac.uk
  [Parameter(Mandatory=$true)][string]$RecordName,  # e.g. _acme-challenge.imm-dc5.imm.ox.ac.uk
  [Parameter(Mandatory=$true)][string]$Token        # TXT value
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# Path to heracles (adjust if required)
$HeraclesExe = "heracles.exe"

# --- Helpers --------------------------------------------------------------

function Ensure-Env {
  if ([string]::IsNullOrWhiteSpace($env:HYDRA_URI) -or [string]::IsNullOrWhiteSpace($env:HYDRA_TOKEN)) {
    throw "HYDRA_URI / HYDRA_TOKEN not set in environment. Run under 'bws run --project-id <...> -- ...' or set them."
  }
  if ($env:HYDRA_URI -notmatch '/ipam/\/?$') {
    Write-Warning ("HYDRA_URI normally ends with '/ipam/' (current: '{0}')" -f $env:HYDRA_URI)
  }
}

function To-Fqdn([string]$name) {
  if ($name.EndsWith('.')) { return $name } else { return "$name." }
}

# Writes ASCII JSON to a temp file and invokes heracles via cmd stdin redirection.
function Invoke-HeraclesJson {
  param(
    [Parameter(Mandatory=$true)][ValidateSet('add','delete')] [string]$Subcommand,
    [Parameter(Mandatory=$true)][string]$Json
  )

  $tmp = [System.IO.Path]::GetTempFileName()
  try {
    [System.IO.File]::WriteAllText($tmp, $Json, [System.Text.Encoding]::ASCII)

    # Use cmd.exe < redirection (this matched the successful manual test)
    $inner = '"heracles.exe" {0} < "{1}"' -f $Subcommand, $tmp
    $output = & "$env:ComSpec" /d /s /c $inner 2>&1
    $code   = $LASTEXITCODE

    return @{ Output = $output; ExitCode = $code }
  }
  finally {
    Remove-Item $tmp -ErrorAction SilentlyContinue
  }
}

# Create TXT record (builds exact JSON Heracles expects)
function Add-Txt {
  param([string]$fqdn,[string]$value)

  # JSON shape Heracles expects (ASCII)
  $comment = "WinAcme"
  $json = "[{`"comment`"`:`"$comment`"`,`"content`"`:`"$value`"`,`"hostname`"`:`"$fqdn`"`,`"type`"`:`"TXT`"}]"

  # Save a copy for debugging
  try {
    $logDir = "C:\ProgramData\Heracles"
    New-Item -Path $logDir -ItemType Directory -Force | Out-Null
    $logFile = Join-Path $logDir "last_add.json"
    [System.IO.File]::WriteAllText($logFile, $json, [System.Text.Encoding]::ASCII)
  } catch {}

  # Show first bytes (debug)
  $bytes = [System.Text.Encoding]::ASCII.GetBytes($json)
  $preview = ($bytes[0..([Math]::Min(15,$bytes.Length-1))] -join ',')
  Write-Host ("JSON first bytes: {0}" -f $preview)  # should start with 91 ('[')

  Write-Host ("Heracles add -> hostname={0}" -f $fqdn)
  $res = Invoke-HeraclesJson -Subcommand add -Json $json
  if ($res.ExitCode -ne 0) {
    throw ("heracles add failed ({0}): {1}" -f $res.ExitCode, $res.Output)
  }

  # Parse and validate response JSON
  try {
    $added = $res.Output | ConvertFrom-Json
  } catch {
    Write-Warning ("Heracles output not JSON; raw output below:`n{0}" -f $res.Output)
    throw ("heracles add returned non-JSON")
  }

  if (-not $added -or ($added.Count -lt 1)) {
    throw ("heracles add returned empty result; record not created. Raw: {0}" -f $res.Output)
  }

  $ok = $added | Where-Object { $_.type -eq 'TXT' -and $_.hostname -eq $fqdn -and $_.content -eq $value }
  if (-not $ok) {
    throw ("heracles add response did not include expected TXT. Raw: {0}" -f $res.Output)
  }

  # Safely extract ids (works when $added is single object or array)
  $ids = @($added) | ForEach-Object { $_.id }
  Write-Host ("Add confirmed (id(s): {0})" -f ($ids -join ','))
}

# Delete TXT record(s) matching hostname+content
function Remove-Txt {
  param([string]$fqdn,[string]$value)

  $fqdnDot = if ($fqdn.EndsWith('.')) { $fqdn } else { "$fqdn." }

  Write-Host ("Heracles search -> {0}" -f $fqdnDot)
  $searchOut = & $HeraclesExe search $fqdnDot 2>&1
  if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($searchOut)) {
    Write-Warning ("heracles search non-zero exit ({0}): {1} ; assuming nothing to delete." -f $LASTEXITCODE, $searchOut)
    return
  }

  # Parse JSON and normalize to array
  $recordsRaw = $null
  try { $recordsRaw = $searchOut | ConvertFrom-Json } catch {
    Write-Warning ("Search output not JSON; assuming nothing to delete. Raw: {0}" -f $searchOut)
    return
  }
  $records = @()
  if ($recordsRaw -is [System.Array]) { $records = $recordsRaw } elseif ($recordsRaw) { $records = @($recordsRaw) }

  # Filter to the exact TXT we created
  $matches = $records | Where-Object {
    $_.type -eq 'TXT' -and $_.hostname -eq $fqdnDot -and $_.content -eq $value
  }
  $matchesArr = @()
  if ($matches -is [System.Array]) { $matchesArr = $matches } elseif ($matches) { $matchesArr = @($matches) }

  if ($matchesArr.Count -lt 1) {
    Write-Warning ("No exact TXT match to delete for {0} value '{1}'." -f $fqdnDot, $value)
    return
  }

  # Re-serialize the matched record(s) as an array (what `heracles delete` expects)
  $payload = ($matchesArr | ConvertTo-Json -Compress -Depth 8)
  if ($payload.Trim().StartsWith('{')) { $payload = '[' + $payload + ']' }  # ensure array even for single match

  # Save the exact JSON we'll send (useful for troubleshooting)
  try {
    $logDir = "C:\ProgramData\Heracles"
    New-Item -Path $logDir -ItemType Directory -Force | Out-Null
    [System.IO.File]::WriteAllText((Join-Path $logDir "last_delete.json"), $payload, [System.Text.Encoding]::ASCII)
  } catch {}

  Write-Host ("Heracles delete -> {0} ({1} record(s))" -f $fqdnDot, $matchesArr.Count)
  $res = Invoke-HeraclesJson -Subcommand delete -Json $payload
  if ($res.ExitCode -ne 0) {
    Write-Warning ("heracles delete reported ({0}): {1}" -f $res.ExitCode, $res.Output)
    return
  }

  Write-Host ("Delete confirmed (id(s): {0})" -f (($matchesArr | ForEach-Object { $_.id }) -join ','))
}



# Optional helper: wait until TXT visible on authoritative servers
function Wait-ForTxt {
  param(
    [string]$fqdn,
    [string]$value,
    [int]$timeoutSec = 600,
    [int]$intervalSec = 15,
    [string[]]$servers = @("129.67.1.190","129.67.1.191","163.1.2.190")
  )
  $deadline = (Get-Date).AddSeconds($timeoutSec)
  do {
    foreach ($s in $servers) {
      try {
        $ans = Resolve-DnsName -Type TXT -Server $s $fqdn -ErrorAction Stop
        if ($ans | Where-Object { $_.Strings -contains $value }) {
          Write-Host ("TXT visible at {0}" -f $s)
          return
        }
      } catch {}
    }
    Start-Sleep -Seconds $intervalSec
  } while ((Get-Date) -lt $deadline)
  throw ("TXT not visible on authoritative servers within {0}s" -f $timeoutSec)
}

# ----------------------- MAIN ----------------------------------------------
try {
  Ensure-Env
  $fqdn = To-Fqdn $RecordName

  switch ($Operation.ToLowerInvariant()) {
    'create' {
      Add-Txt -fqdn $fqdn -value $Token
      # Optionally wait for visibility:
      # Wait-ForTxt -fqdn $fqdn -value $Token
    }
    'delete' {
      Remove-Txt -fqdn $fqdn -value $Token
    }
    default  { throw ("Unknown operation '{0}' (expected 'create' or 'delete')." -f $Operation) }
  }
}
catch {
  Write-Error $_.Exception.Message
  exit 1
}
