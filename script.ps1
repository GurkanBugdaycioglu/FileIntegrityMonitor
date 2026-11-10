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

		$targetPath = Get-Item -Path $targetFilePath -ErrorAction Stop
		$resolvedTargetPath = $targetPath.FullName

		$hash= Get-FileHash -Path $resolvedTargetPath -Algorithm SHA256

		"$($resolvedTargetPath),$($hash.Hash)" | Out-File -FilePath $baselineFilePath -Encoding utf8 -Append

		Write-Output "Entry succesfully added into the baseline"
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