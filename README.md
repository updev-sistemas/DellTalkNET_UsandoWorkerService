DellTalkNET_UsandoWorkerService



# Instalação Windows

sc.exe create NOME_APLICATIVO binpath= CAMINHO_DO_APLICATIVO.exe start= auto


# Instalação Linux

Instalar o Package: dotnet add package Microsoft.Extensions.Hosting.Systemd

Alterar em Program.cs de .UseWindowsService() para .UseSystemd()

Criar arquivo com extensão NOME_SERVICE.service e mover para /etc/systemd/system 
Executar comando: systemctl daemon-reloadc
Executar comando: systemctl start NOME_SERVICE.service

```
[Unit]
Description=Nome do Aplicativo

[Service]
User=root
WorkingDirectory=/path/do/aplicativo
ExecStart=/path/do/aplicativo/Nome_Do_Executavel
Restart=always

[Install]
WantedBy=multi-user.target 

```
