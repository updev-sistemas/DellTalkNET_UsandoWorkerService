# Instalação Windows

sc.exe create NOME_APLICATIVO binpath= CAMINHO_DO_APLICATIVO.exe start= auto

# Remoção Windows 
sc.exe delete NOME_APLICATIVO


# Instalação Linux

Instalar o Package: dotnet add package Microsoft.Extensions.Hosting.Systemd

Alterar em Program.cs de .UseWindowsService() para .UseSystemd()

## Criar o arquivo de configuração
```
touch NOME_SERVICE.service
```

## Mover o arquivo NOME_SERVICE.service para /etc/systemd/system
```
sudo mv NOME_SERVICE.service /etc/systemd/system
```
 
## Executar comando de Reload do SystemMD 
```
systemctl daemon-reloadc
```

## Executar comando Start para Iniciar o Serviço.
```
systemctl start NOME_SERVICE.service
```

## Template a ser colocada no  SERVICE_NOME.service
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
