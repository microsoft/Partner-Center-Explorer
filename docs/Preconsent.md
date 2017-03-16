# Pre-consent 
Azure Active Directory utilizes a consent framework that is based on a user or an administrator giving consent to an application 
that asks to be registered in their directory, which may involve accessing directory data. This framework presents two primary 
problems for Cloud Solution Provider (CSP) partners

1. Getting all customers to consent to an application can prove to be difficult task.
2. There might be a need to perform an operation against a newly provisioned tenant before access has been granted to the customer. 
Which means the customer will not be able to consent to the application.

In order to overcome these issues CSP partners can configure an for pre-consent. This configuration takes advantage of the 
delegated administrative permissions that are granted to CSP partner over a customer associated with their reseller. Perform 
the following steps to configure the application that will be used to access the Partner Center API for pre-consent 

1. Install Azure AD PowerShell Module (instruction available [here](https://docs.microsoft.com/en-us/powershell/azuread/)).
2. Update the _AppId_ and _DisplayName_ variables in the PowerShell script below
3. Execute the modified PowerShell script. When prompted for authentication specify credentials that belong to the tenant where the application was created and that have global 
admin privileges  

```powershell
Connect-AzureAD

$AppId = 'INSERT-APPLICATION-ID-HERE'
$DisplayName = 'INSERT-APPLICATION-DISPLAY-NAME-HERE'

$g = Get-AzureADGroup | ? {$_.DisplayName -eq 'AdminAgents'}
$s = Get-AzureADServicePrincipal | ? {$_.AppId -eq $AppId}

if ($s -eq $null) { $s = New-AzureADServicePrincipal -AppId $AppId -DisplayName $DisplayName }
Add-AzureADGroupMember -ObjectId $g.ObjectId -RefObjectId $s.ObjectId
```