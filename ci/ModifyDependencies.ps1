# ModifyDependencies.ps1 script.
# This script file is responsible for modifying versions
# (and other attributes of dependencies defined in different
# files of the project).

[CmdletBinding()]
param (
    [Parameter(Mandatory = $false)]
    [string]$CsProjPath = ".\SeleniumTest1\SeleniumTest1.csproj"
)

# Make sure System.Xml.Linq is loaded before proceeding.
[System.Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq")

$csProjContent = Get-Content -Path $CsProjPath -Raw

$myDocX = [System.Xml.Linq.XDocument]::Parse($csProjContent)

$packageReferences = $myDocX.Element("Project").Elements("ItemGroup").Elements("PackageReference")
[System.Xml.Linq.XElement]$chromeDriver = $packageReferences | Where-Object {
    $_.Attribute("Include").Value -eq "Selenium.WebDriver.ChromeDriver" 
}

# $targetVersion = "110.0.5481.7700"
$targetVersion = "110.0.5481.3000"

# change version of chromeDriver package to the one that works.
if ($chromeDriver.Attribute("Version").Value -ne $targetVersion) {
    $chromeDriver.SetAttributeValue("Version", $targetVersion)
}

$finalValue = $myDocX.ToString()
Set-Content -Path $CsProjPath -Value $finalValue -Force
