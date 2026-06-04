Set-StrictMode -Version Latest

function Get-DevQaPropertyValue {
    param(
        [Parameter(Mandatory = $true)]
        [object]$InputObject,
        [Parameter(Mandatory = $true)]
        [string[]]$PropertyNames
    )

    if ($null -eq $InputObject) {
        return $null
    }

    foreach ($propertyName in $PropertyNames) {
        $property = $InputObject.PSObject.Properties[$propertyName]
        if ($null -ne $property) {
            return $property.Value
        }
    }

    return $null
}

function Get-DevQaEnumerable {
    param([object]$Value)

    if ($null -eq $Value) {
        return @()
    }

    if ($Value -is [string]) {
        return @($Value)
    }

    if ($Value -is [System.Collections.IEnumerable]) {
        return @($Value)
    }

    return @($Value)
}

function Get-DevQaResourceMap {
    param([object]$InputObject)

    $result = [ordered]@{}
    $warnings = New-Object System.Collections.Generic.List[string]

    if ($null -eq $InputObject) {
        $warnings.Add("No resource data was returned.")
        return [pscustomobject]@{
            Map = $result
            Warnings = @($warnings)
            RecognizedShape = $false
        }
    }

    $flatResourceNames = @("credits", "metal", "crystal", "gas", "deuterium", "energy")

    $resourceCandidates = Get-DevQaPropertyValue -InputObject $InputObject -PropertyNames @("stockpile", "resourceStockpile", "resources", "reserves")
    if ($null -ne $resourceCandidates) {
        return Get-DevQaResourceMap -InputObject $resourceCandidates
    }

    $inputProperties = @($InputObject.PSObject.Properties.Name)
    $flatKeys = @($inputProperties | Where-Object { $flatResourceNames -contains $_.ToLowerInvariant() })
    if ($flatKeys.Count -gt 0) {
        foreach ($key in $flatKeys) {
            $value = Get-DevQaPropertyValue -InputObject $InputObject -PropertyNames @($key)
            if ($null -ne $value) {
                $result[$key] = [decimal]$value
            }
        }

        return [pscustomobject]@{
            Map = $result
            Warnings = @($warnings)
            RecognizedShape = $true
        }
    }

    $rows = Get-DevQaEnumerable $InputObject
    $recognizedRowCount = 0

    foreach ($row in $rows) {
        if ($null -eq $row) {
            continue
        }

        $resourceName = Get-DevQaPropertyValue -InputObject $row -PropertyNames @("resourceType", "name", "resource", "label", "key")
        $amountValue = Get-DevQaPropertyValue -InputObject $row -PropertyNames @("amount", "quantity", "value", "total")

        if ($null -ne $resourceName -and $null -ne $amountValue) {
            $result["$resourceName"] = [decimal]$amountValue
            $recognizedRowCount++
        }
    }

    if ($recognizedRowCount -gt 0) {
        return [pscustomobject]@{
            Map = $result
            Warnings = @($warnings)
            RecognizedShape = $true
        }
    }

    $warnings.Add("Resource data shape was not recognized by the QA helper.")
    return [pscustomobject]@{
        Map = $result
        Warnings = @($warnings)
        RecognizedShape = $false
    }
}

function Format-DevQaResourceSummary {
    param([object]$InputObject)

    $resourceState = if ($InputObject -is [hashtable] -or $InputObject -is [System.Collections.Specialized.OrderedDictionary]) {
        [pscustomobject]@{
            Map = $InputObject
            Warnings = @()
            RecognizedShape = ($InputObject.Count -gt 0)
        }
    }
    else {
        Get-DevQaResourceMap -InputObject $InputObject
    }

    $map = $resourceState.Map
    if ($map.Count -eq 0) {
        return [pscustomobject]@{
            Summary = "warning: no readable resource rows were returned"
            Warnings = @($resourceState.Warnings)
            RecognizedShape = $resourceState.RecognizedShape
            Map = $map
        }
    }

    $summary = ($map.Keys | Sort-Object | ForEach-Object { "{0}={1}" -f $_, $map[$_] }) -join ", "
    return [pscustomobject]@{
        Summary = $summary
        Warnings = @($resourceState.Warnings)
        RecognizedShape = $resourceState.RecognizedShape
        Map = $map
    }
}

function Get-DevQaOpenQueueCount {
    param(
        [object]$Rows,
        [string[]]$OpenStatusNames = @("Pending", "Active")
    )

    $openStatuses = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($statusName in $OpenStatusNames) {
        [void]$openStatuses.Add($statusName)
    }

    $count = 0
    foreach ($row in (Get-DevQaEnumerable $Rows)) {
        $statusValue = Get-DevQaPropertyValue -InputObject $row -PropertyNames @("status", "Status")
        if ($null -ne $statusValue -and $openStatuses.Contains("$statusValue")) {
            $count++
        }
    }

    return $count
}

function Get-DevQaShipyardSnapshot {
    param([object]$ShipyardUiState)

    $shipyard = Get-DevQaPropertyValue -InputObject $ShipyardUiState -PropertyNames @("shipyard", "Shipyard")
    if ($null -eq $shipyard) {
        $shipyard = $ShipyardUiState
    }

    if ($null -eq $shipyard) {
        return $null
    }

    $resources = Format-DevQaResourceSummary (Get-DevQaPropertyValue -InputObject $shipyard -PropertyNames @("resourceStockpile", "ResourceStockpile"))
    $catalog = Get-DevQaEnumerable (Get-DevQaPropertyValue -InputObject $shipyard -PropertyNames @("catalog", "Catalog"))
    $queue = Get-DevQaEnumerable (Get-DevQaPropertyValue -InputObject $shipyard -PropertyNames @("queue", "Queue"))
    $stock = Get-DevQaEnumerable (Get-DevQaPropertyValue -InputObject $shipyard -PropertyNames @("orbitalStock", "OrbitalStock"))

    $availableCount = @($catalog | Where-Object {
        (Get-DevQaPropertyValue -InputObject $_ -PropertyNames @("availabilityStatus", "AvailabilityStatus")) -eq "Available"
    }).Count

    return [pscustomobject]@{
        Planet = Get-DevQaPropertyValue -InputObject $shipyard -PropertyNames @("planetName", "PlanetName")
        Resources = $resources.Summary
        ResourceWarnings = @($resources.Warnings)
        AvailableOptions = $availableCount
        BlockedOptions = @($catalog).Count - $availableCount
        QueueCount = @($queue).Count
        StockCount = @($stock).Count
    }
}

function Get-DevQaFleetSnapshot {
    param(
        [object]$FleetUiState,
        [Guid]$PlanetId
    )

    $fleet = Get-DevQaPropertyValue -InputObject $FleetUiState -PropertyNames @("uiState", "UiState")
    if ($null -eq $fleet) {
        $fleet = $FleetUiState
    }

    if ($null -eq $fleet) {
        return $null
    }

    $groups = Get-DevQaEnumerable (Get-DevQaPropertyValue -InputObject $fleet -PropertyNames @("groups", "Groups"))
    $resourceContexts = Get-DevQaEnumerable (Get-DevQaPropertyValue -InputObject $fleet -PropertyNames @("resourceContexts", "ResourceContexts"))
    $activeTransfers = @($groups | Where-Object {
        [bool](Get-DevQaPropertyValue -InputObject $_ -PropertyNames @("hasActiveTransfer", "HasActiveTransfer"))
    }).Count
    $stationedCount = @($groups | Where-Object {
        (Get-DevQaPropertyValue -InputObject $_ -PropertyNames @("status", "Status")) -eq "Stationed"
    }).Count

    $selectedContext = $resourceContexts | Where-Object {
        (Get-DevQaPropertyValue -InputObject $_ -PropertyNames @("planetId", "PlanetId")) -eq $PlanetId
    } | Select-Object -First 1

    if ($null -eq $selectedContext) {
        $selectedContext = @($resourceContexts | Select-Object -First 1)
    }

    $resourceSummary = Format-DevQaResourceSummary (Get-DevQaPropertyValue -InputObject $selectedContext -PropertyNames @("balances", "Balances"))

    return [pscustomobject]@{
        GroupCount = @($groups).Count
        StationedCount = $stationedCount
        ActiveTransferCount = $activeTransfers
        ResourceContext = $resourceSummary.Summary
        ResourceWarnings = @($resourceSummary.Warnings)
        ResourceContextCount = @($resourceContexts).Count
    }
}

function Get-DevQaHttpResponseBody {
    param([System.Exception]$Exception)

    $response = $Exception.Response
    if ($null -eq $response) {
        return $null
    }

    $stream = $response.GetResponseStream()
    if ($null -eq $stream) {
        return $null
    }

    $reader = New-Object System.IO.StreamReader($stream)
    try {
        if ($reader.BaseStream.CanSeek) {
            $reader.BaseStream.Position = 0
        }
        $reader.DiscardBufferedData()
        return $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }
}

function ConvertFrom-DevQaJsonSafely {
    param([string]$JsonText)

    if ([string]::IsNullOrWhiteSpace($JsonText)) {
        return $null
    }

    try {
        $command = Get-Command ConvertFrom-Json -ErrorAction Stop
        if ($command.Parameters.ContainsKey("Depth")) {
            return $JsonText | ConvertFrom-Json -Depth 20
        }

        return $JsonText | ConvertFrom-Json
    }
    catch {
        return $null
    }
}

function Get-DevQaResponseErrorText {
    param(
        [object]$ResponseObject,
        [string]$FallbackText
    )

    if ($null -eq $ResponseObject) {
        if (-not [string]::IsNullOrWhiteSpace($FallbackText)) {
            return Get-DevQaResponseErrorText -ResponseObject $FallbackText
        }

        return $null
    }

    if ($ResponseObject -is [string]) {
        $parsedResponse = ConvertFrom-DevQaJsonSafely $ResponseObject
        if ($null -ne $parsedResponse) {
            return Get-DevQaResponseErrorText -ResponseObject $parsedResponse
        }

        return $ResponseObject
    }

    $errors = @(Get-DevQaEnumerable (Get-DevQaPropertyValue -InputObject $ResponseObject -PropertyNames @("errors", "Errors")))
    if ($errors.Count -eq 0) {
        if (-not [string]::IsNullOrWhiteSpace($FallbackText)) {
            return Get-DevQaResponseErrorText -ResponseObject $FallbackText
        }

        return $null
    }

    return ($errors | ForEach-Object { "$_" }) -join "; "
}

function Format-DevQaPayloadSummary {
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Payload
    )

    if ($Payload.Count -eq 0) {
        return "none"
    }

    return ($Payload.Keys | ForEach-Object { "{0}={1}" -f $_, $Payload[$_] }) -join ", "
}

function Test-DevQaResponseHasKnownError {
    param(
        [object]$ResponseObject,
        [string]$FallbackText,
        [Parameter(Mandatory = $true)]
        [string]$KnownErrorFragment
    )

    $errorText = Get-DevQaResponseErrorText -ResponseObject $ResponseObject -FallbackText $FallbackText
    if ([string]::IsNullOrWhiteSpace($errorText)) {
        return $false
    }

    return $errorText.IndexOf($KnownErrorFragment, [System.StringComparison]::OrdinalIgnoreCase) -ge 0
}
