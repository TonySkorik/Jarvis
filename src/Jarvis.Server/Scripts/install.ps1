$binariesPath = "c:\Program Files\Jarvis\"
$exeFilePath = "c:\Program Files\Jarvis\Jarvis.Server.exe"
$userName = "TONY-PC-MARKII\justs"
$serviceName = "Jarvis.Server"
$description = "Jarvis Core Server Component"
$displayName = "Jarvis Server"

$acl = Get-Acl $binariesPath
$aclRuleArgs = $userName, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $binariesPath

New-Service -Name $serviceName -BinaryPathName $exeFilePath -Credential $userName -Description $description -DisplayName $displayName -StartupType Automatic