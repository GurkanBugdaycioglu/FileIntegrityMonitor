$scriptPath = $PSScriptRoot
$baselineFilePath = Join-Path -Path $scriptPath -ChildPath "baselines.csv"

##add a new file to the baseline csv
$fileToMonitorPath = Join-Path -Path $scriptPath -ChildPath "Files\test.txt"
$hash= Get-FileHash -Path $fileToMonitorPath -Algorithm SHA256

"$($fileToMonitorPath),$($hash.Hash)" | Out-File -FilePath $baselineFilePath -Encoding utf8 -Append

##monitor the file for changes
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