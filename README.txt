## 實作內容

## 實作加分題


## docker image 生成
```
//先登入docker hub
docker login

//切換工作目錄
cd coin_api01\coin_api01
image.bat

//上傳 image.bat 產生出來的TAG與檔案
docker push [TAG]

```

## 容器環境建立
### MS SQL
```
docker run --name sql2019 -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
```