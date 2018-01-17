#### Expected output:

```bash
$ dotnet restore
...

$ dotnet run

Using OAuth 2.0 Device Flow. You need to sign in.
To sign in, use a web browser to open the page https://aka.ms/devicelogin
and enter the code XXXXXXXX to authenticate.
...

Azure AD returned this JWT:

Bearer eyJ0eXAiOiJ...7AlfAuW_Q

TOKEN IS VALID.
  Issuer: https://sts.windows.net/{tenant_guid}/
  ValidFrom: 1/17/2018 10:02:04 AM
  ValidTo: 1/17/2018 11:07:04 AM
  Public Signing Key: z44wMdHu8wKsumrbfaK98qxs5YI

Claims:
  aud: https://xxxxxxxxx.azurewebsites.net
  iss: https://sts.windows.net/{tenant_guid}/
  iat: 1516183324
  nbf: 1516183324
  exp: 1516187224
  http://schemas.microsoft.com/claims/authnclassreference: 1
  aio: ASQ...yTY=
  http://schemas.microsoft.com/claims/authnmethodsreferences: pwd
  appid: 202e7978-0008-4c07-8c54-440c9be7cb94
  appidacr: 0
  http://schemas.microsoft.com/2012/01/devicecontext/claims/identifier: xxxx-xxxx-xxxx-xxxx
  e_exp: 262800
  http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname: Loblaw
  http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname: Bob
  ipaddr: x.x.x.x
  name: Bob Loblaw
  http://schemas.microsoft.com/identity/claims/objectidentifier: xxxx-xxxx-xxxx-xxxx
  http://schemas.microsoft.com/identity/claims/scope: user_impersonation
  http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: ZXs...TYgU
  http://schemas.microsoft.com/identity/claims/tenantid: xxxx-xxxx-xxxx-xxxx
  http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name: bob@{tenant_name}.onmicrosoft.com
  http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn: bob@{tenant_name}.onmicrosoft.com
  uti: OP-...AA
  ver: 1.0
  ```