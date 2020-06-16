Welcome to the WebSecure wiki by **Saineshwar Bageri**!

Download Database :- https://payhip.com/b/9SoY

# Custom authentication with ASP.NET Core With SHA512 algorithm
Secure ASP.NET Core Application essential feature Using SHA512 algorithm. 

_**Everyone must store Salt in Different Database other than Main project Database For Security**_

Each time you create a new application you need essential feature such as
* Registration
* Login
* ChangePassword
* ResetPassword
* Email Verification of registered User 
* Sending Email for Reset Password

For Demo I have used gmail.
(Application is Configured with Gmail For Sending Email enter details in appsettings.json)

Making App Less Secure do at own Risk i am not responsible for it.

* Open your Google Admin console (admin.google.com).
* Click Security > Basic settings .
* Under Less secure apps, select Go to settings for less secure apps .
* In the subwindow, select the Enforce access to less secure apps for all users radio button. ...
* Click the Save button.

Enable less secure app on gmail for sending email from localhost :- https://hotter.io/docs/email-accounts/secure-app-gmail/

***
## Getting Started

### Registation Process

When User register clicks on submit button on Client-Side SHA512 Hash of the password is created and sent to the server on, on serverside salt is generated and combination **[ SHA512 Hash + Salt ]** is stored in **User** Table and salt is stored in **UserTokens** Table. 

A Verification Email is sent to entered email id. After the Verification, User will able to log into the application. 
While sending Email we generate **key** and **Unique Token**. the **key** is created using a combination of **(Ticks + UserId)** 
**_"More things can be added to make it more secure"_** which is Encrypted using AES algorithm. And **Unique Token** is generated using **RNGCryptoServiceProvider**. 

` var linktoverify = _appSettings.VerifyRegistrationUrl + "?key=" + HttpUtility.UrlEncode(encrypt) + "&hashtoken=" + HttpUtility.UrlEncode(token);`



***
### Login Process
   
When User Log into Application using Username and password, according to Username we get UserDetails of the User from **User** Table and on based of UserId we get User Salt which is stored in **UserTokens** Table. Next, we are going to combine Posted User **Password SHA512 Hash** with Stored **User Salt** and compare with Stored Hash in **User** Table.  


***


### Reset Password Process
Next in Forgot Password Process, we are going ask the user to enter Username and check Username exists in database then we are going to send an email with links to reset the password.
 
` var linktoverify = _appSettings.VerifyResetPasswordUrl + "?key=" + HttpUtility.UrlEncode(encrypt) + "&hashtoken=" + HttpUtility.UrlEncode(token);`

After clicking on Reset Password Link it will redirect to Reset Password Page. where you will be entering New Password to change your Password.
In the Reset Password process, New password hash and New Salt is created for more security.
All Password History is Maintained in **PasswordHistory** Table.
***

### Change Password Process
Next in Change Password Process, this process is done after login into the application there we are going to ask the user to enter Current password and New Password.  
All Password History is Maintained in **PasswordHistory** Table.

### About Platform Used 
Targeted Framework .Net Core 3.1

### Microsoft Visual Studio Community 2019<br>
Link to download Microsoft Visual Studio Community 2019: - https://visualstudio.microsoft.com/vs/ 

### Microsoft SQL Server 2019<br>
Link to download SQL Server Express: - https://www.microsoft.com/en-us/sql-server/sql-server-downloads

### External packages which are used in .Net Core Project
* Dapper ORM
* Microsoft.EntityFrameworkCore
* System.Data.SqlClient
* Microsoft.EntityFrameworkCore.SqlServer

### External packages javascript used
* js-sha256
* js-sha512

## Getting Started
1. Restore Database Script

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/4.png)

2. Make changes in appsettings.json Settings

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/13.png)

3. Run Application 

### Login Page

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/9.png)

### Registration Page

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/8.png)

### Email Verification of registered User

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/6.png)

### Forgot Password

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/10.png)

### Sending Email for Reset Password

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/11.png)

### Sending Email for Reset Password

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/7.png)

### Reset Password

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/12.png)

### Database

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/4.png)

### Tables 

![](https://github.com/saineshwar/WebSecure/blob/master/WebSecure/Images/5.png)
