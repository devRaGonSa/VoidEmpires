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
