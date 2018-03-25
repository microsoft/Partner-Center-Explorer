<#
 * MIT License
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 #>

<#
    .SYNOPSIS
        This script will apply the supported service policy restriction for the specified Azure subscription. 
    .EXAMPLE
        .\Create-AzureADApplication.ps1 -DisplayName "Partner Center Explorer" 
        
        .\Create-AzureADApplication.ps1 -DisplayName "Partner Center Explorer" -TenantId eb210c1e-b697-4c06-b4e3-8b104c226b9a

        .\Create-AzureADApplication.ps1 -DisplayName "Partner Center Explorer" -TenantId tenant01.onmicrosoft.com
    .PARAMETER DisplayName
        Display name for the Azure AD application that will be created.
    .PARAMETER TenantId
        [OPTIONAL] The domain or tenant identifier for the Azure AD tenant that should be utilized to create the various resources. 
#>

Param
(
    [Parameter(Mandatory = $true)]
    [string]$DisplayName,
    [Parameter(Mandatory = $false)]
    [string]$TenantId
)

$ErrorActionPreference = "Stop"

# Check if the Azure AD PowerShell module has already been loaded. 
if ( ! ( Get-Module AzureAD ) ) {
    # Check if the Azure AD PowerShell module is installed.
    if ( Get-Module -ListAvailable -Name AzureAD ) {
        # The Azure AD PowerShell module is not load and it is installed. This module 
        # must be loaded for other operations performed by this script.
        Write-Host -ForegroundColor Green "Loading the AzureAD PowerShell module..."
        Import-Module AzureAD
    } else {
        Install-Module AzureAD
    }
}

$credential = Get-Credential -Message "Please specify credentials that have Global Admin privileges..."

try {
    Write-Host -ForegroundColor Green "When prompted please enter the appropriate credentials..."
   
    if([string]::IsNullOrEmpty($TenantId)) {
        Connect-AzureAD -Credential $credential

        $TenantId = $(Get-AzureADTenantDetail).ObjectId
    } else {
        Connect-AzureAD -Credential $credential -TenantId $TenantId
    }
} catch [Microsoft.Azure.Common.Authentication.AadAuthenticationCanceledException] {
    # The authentication attempt was canceled by the end-user. Execution of the script should be halted.
    Write-Host -ForegroundColor Yellow "The authentication attempt was canceled. Execution of the script will be halted..."
    Exit 
} catch {
    # An unexpected error has occurred. The end-user should be notified so that the appropriate action can be taken. 
    Write-Error "An unexpected error has occurred. Please review the following error message and try again." `
        "$($Error[0].Exception)"
}

$sessionInfo = Get-AzureADCurrentSessionInfo

$adAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "00000002-0000-0000-c000-000000000000";
    ResourceAccess = 
    [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
        Id = "311a71cc-e848-46a1-bdf8-97ff7156d8e6";
        Type = "Scope"}
}

$azureAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "797f4846-ba00-4fd7-ba43-dac1f8f63013";
    ResourceAccess = 
        [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
            Id = "41094075-9dad-400e-a0bd-54e686782033";
            Type = "Scope"}
}

$graphAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "00000003-0000-0000-c000-000000000000";
    ResourceAccess = 
        [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
            Id = "7e05723c-0bb0-42da-be95-ae9f08a6e53c";
            Type = "Role"},
    [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
        Id = "7ab1d382-f21e-4acd-a863-ba3e13f7da61";
        Type = "Role"}
}

$officeAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "c5393580-f805-4401-95e8-94b7a6ef2fc2";
    ResourceAccess = 
        [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
            Id = "e2cea78f-e743-4d8f-a16a-75b629a038ae";
            Type = "Role"}
}

$partnerCenterAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "fa3d9a0c-3fb0-42cc-9193-47c7ecd2edbd";
    ResourceAccess = 
        [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
            Id = "1cebfa2a-fb4d-419e-b5f9-839b4383e05a";
            Type = "Scope"}
}

Write-Host -ForegroundColor Green "Creating the Azure AD application and related resources..."

$app = New-AzureADApplication -AvailableToOtherTenants $true -DisplayName $DisplayName -IdentifierUris "https://$($sessionInfo.TenantDomain)/$((New-Guid).ToString())" -RequiredResourceAccess $adAppAccess, $azureAppAccess, $graphAppAccess, $officeAppAccess, $partnerCenterAppAccess

$spn = New-AzureADServicePrincipal -AppId $app.AppId -DisplayName $DisplayName

$password = New-AzureADApplicationPasswordCredential -ObjectId $app.ObjectId

$adminAgentsGroup = Get-AzureADGroup | Where-Object {$_.DisplayName -eq 'AdminAgents'}
Add-AzureADGroupMember -ObjectId $adminAgentsGroup.ObjectId -RefObjectId $spn.ObjectId

Write-Host "ApplicationId       = $($app.AppId)"
Write-Host "ApplicationSecret   = $($password.Value)"
Write-Host "ApplicationTenantId = $($sessionInfo.TenantId)"