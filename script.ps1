function Add-FileToBaseline{
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory)]$baselineFilePath,
		[Parameter(Mandatory)]$targetFilePath
	)

	try{
		if((Test-Path -Path $baselineFilePath) -eq $false){
			Write-Error -Message "$baselineFilePath does not exist" -ErrorAction Stop
		}
		if((Test-Path -Path $targetFilePath) -eq $false){
			Write-Error -Message "$targetFilePath does not exist" -ErrorAction Stop
		}

		$targetItem = Get-Item -Path $targetFilePath -ErrorAction Stop
		$resolvedTargetPath = $targetItem.FullName

		$imported = Import-Csv -Path $baselineFilePath -Delimiter ","
		$currentBaseline = if ($null -eq $imported) { @() } else { @($imported) }

		if($resolvedTargetPath -in $currentBaseline.path){
			Write-Error -Message "$resolvedTargetPath already exists in the baseline"
			do{
				$overwrite=Read-Host "Do you want to overwrite the existing entry? (Y/N)"
				if($overwrite -in @('y','yes')){
					Write-Host "Entry will be overwritten." -ForegroundColor Cyan

					$currentBaseline = @($currentBaseline | Where-Object { $_.path -ne $resolvedTargetPath })
				}elseif($overwrite -in @('n','no')){
					Write-Host "File path will not be overwritten." -ForegroundColor Gray
					return
				}else{
					Write-Host "Invalid input. Please enter Y or N." -ForegroundColor Red
				}
			}while($overwrite -notin @('y','yes','n','no'))
		}
		$hash = Get-FileHash -Path $resolvedTargetPath -Algorithm SHA256
		$newEntry = [PSCustomObject]@{
			path = $resolvedTargetPath
			hash = $hash.Hash
		}

		$currentBaseline += $newEntry
		$currentBaseline | Export-Csv -Path $baselineFilePath -Delimiter "," -NoTypeInformation -Encoding utf8
		Write-Host "Entry successfully added into the baseline." -ForegroundColor Green

	}catch{
		return $_.Exception.Message
	}
}

function Verify-Baseline{
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory)]$baselineFilePath
	)

	try{
		if((Test-Path -Path $baselineFilePath) -eq $false){
			Write-Error -Message "$baselineFilePath does not exist" -ErrorAction Stop
		}
		$baselineFiles= Import-Csv -Path $baselineFilePath -Delimiter ","

		foreach($file in $baselineFiles) {
			if(Test-Path -Path $file.path) {
				$currentHash= Get-FileHash -Path $file.path -Algorithm SHA256
				if($currentHash.Hash -eq $file.hash) {
					Write-Host "$($file.path) is unchanged." -ForegroundColor Green
				}
				else {
					Write-Host "$($file.path) has been modified!" -ForegroundColor Red
				}
			} else {
				Write-Host "$($file.path) is not found!" -ForegroundColor Yellow
			}
		}
	}catch{
		return $_.Exception.Message
	}
}

function Create-Baseline{
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory)]$baselineFilePath
	)

	try{
		if((Test-Path -Path $baselineFilePath)){
			Write-Error -Message "$baselineFilePath with this name already exist" -ErrorAction Stop
		}

		"path,hash" | Out-File -FilePath $baselineFilePath -Force -Encoding utf8
	}catch{
		return $_.Exception.Message
	}
}
$scriptPath = $PSScriptRoot
$baselineFilePath = Join-Path -Path $scriptPath -ChildPath "baselines.csv"

Create-Baseline -baselineFilePath $baselineFilePath

Add-FileToBaseline -baselineFilePath $baselineFilePath -targetFilePath "Files\test1.txt"

Verify-Baseline -baselineFilePath $baselineFilePath