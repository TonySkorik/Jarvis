$exePath = "./"
$exeFilePath = "Jarvis.Server.exe"
$userName = "justs"
$serviceName = "Jarvis.Server"
$description = "Jarvis Core Server Component"
$displayName = "Jarvis Server"

$acl = Get-Acl $exePath
$aclRuleArgs = $userName, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $exePath

New-Service -Name $serviceName -BinaryPathName $exeFilePath -Credential $userName -Description $description -DisplayName $displayName -StartupType Automatic