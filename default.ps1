properties {
	$targetUrl = "http://localhost/"
	$wspFile = ".\Oxbow.Gwe.SharePoint\bin\debug\Oxbow.Gwe.SharePoint.wsp"
	$wspName = "Oxbow.Gwe.SharePoint.wsp"
	#$featureName = "MSI.ServiceEscalation.WSS_WebScopedFeatures"
	Set-Alias -Name stsadm -Value "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\14\BIN\STSADM.EXE"
}
task default -depends Deploy

task Deploy -depends Clean { 
	"stsadm -o addsolution -filename $wspFile"
	stsadm -o addsolution -filename "$wspFile"
	"stsadm -o deploysolution -name $wspName -allowgacdeployment -immediate"
	stsadm -o deploysolution -name $wspName -allowgacdeployment -immediate 
	"Start-Sleep -s 10"
	Start-Sleep -s 10
	"stsadm -o execadmsvcjobs"
	stsadm -o execadmsvcjobs
	#"net stop SharePoint 2010 Timer"
	#net stop "SharePoint 2010 Timer"
	#"net start SharePoint 2010 Timer"
	#net start "SharePoint 2010 Timer"
}

task Clean { 
	#"stsadm -o deactivatefeature -name $featureName -url $targetUrl"
	#stsadm -o deactivatefeature -name $featureName -url $targetUrl
	"stsadm -o retractsolution -name $wspName -immediate"
	stsadm -o retractsolution -name $wspName -immediate
	"stsadm -o execadmsvcjobs"
	stsadm -o execadmsvcjobs
	"Start-Sleep -s 10"
	Start-Sleep -s 10
	"stsadm -o deletesolution  -name $wspName"
	stsadm -o deletesolution  -name $wspName
	#iisreset
}