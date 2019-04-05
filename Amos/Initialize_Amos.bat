%systemroot%\system32\inetsrv\appcmd add app /site.name:"Default Web Site" /path:/Amos /physicalPath:C:\inetpub\wwwroot\Amos
icacls C:\inetpub\wwwroot\Amos /grant "IIS APPPOOL"\DefaultAppPool:(w)
