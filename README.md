## 實作內容
* 幣別 DB 維護功能。
* 呼叫 coindesk 的 API。
* 呼叫 coindesk 的 API，並進行資料轉換，組成新 API。
* 所有功能均須包含單元測試。

## 實作加分題
* Error handling 處理 API response
* 能夠運行在 Docker

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