# Terminology
The following terms are essential to understand when working with this project 

* **Admin on Behalf of (AOBO)** - Where an application created by the partner and registered in the partner’s Azure AD tenant can perform administrative tasks on behalf of 
the customer. This concept leverages the app + user authentication flow.​
* **App only authentication** - An OAuth2 authentication flow where the client identifier and client secret are used to obtain a token from Azure AD for the application itself.
This is possible because the application in Azure AD has a secret, also known as a password, that is used to verify the application’s identity. ​
* **App + user authentication** - An OAuth2 authentication flow where the client identifier and user credentials are used to obtain a token from Azure AD. This type of 
authentication requires user credentials or a user assertion token. ​
* **Application permissions** - A set of permissions that are available to be assigned to an Azure AD application. Once configured these permissions will be the privileges 
that the application has been granted. In order to utilize these permissions the app only authentication flow must be used.
* **Delegated permissions** - A set permissions that are available to be delegated to a user. Once configured these permissions will be the privileges delegated to a user. In
order to utilizes these permissions the app + user authentication flow must be used.
* **Pre-consent** - A configuration for an application in Azure AD that enables the application to take advantage of the existing delegated administrative rights. This 
configuration enables an application to bypass the consent framework when accessing resources that belong to customers that have an existing relationship with the partner. 
* **Service Principal** - An object that defines the policy and permissions for an application, providing the basis for a security principal to represent the application when 
accessing resources at run-time. A service principal object is required in each tenant for which an instance of the application's usage must be represented, enabling secure 
access to resources owned by user accounts from that tenant.