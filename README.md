# Basic-Web-Login
## 1. Step one
Run the first migration with the command 'Add-Migration <migration name>' on dotnet cmd. Run Program.cs with the commented lines.
This will create the SQL Server DB. Then, comment again the previous lines for skipping this step when running the app. DB should be created now.

## 2. Step two
Build appsettings.json. Check appsettingsExample.json as a guide.
Here is where credentials and DB connections are setted, modify them with your credentials (email, password, smtp-server and host).
Also modify DB connection string.

##3. Front-end and wwwroot
Feel free to add assets, images and styles. This project frontend is pretty simple so you can personalize it as you want using css or directly
modifying the views. Be careful with the asp variables, controllers and models.