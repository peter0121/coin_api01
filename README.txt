## ��@���e
* ���O DB ���@�\��C
* �I�s coindesk �� API�C
* �I�s coindesk �� API�A�öi�����ഫ�A�զ��s API�C
* �Ҧ��\�ৡ���]�t�椸���աC

## ��@�[���D
* Error handling �B�z API response
* ����B��b Docker

## docker image �ͦ�
```
//���n�Jdocker hub
docker login

//�����u�@�ؿ�
cd coin_api01\coin_api01
image.bat

//�W�� image.bat ���ͥX�Ӫ�TAG�P�ɮ�
docker push [TAG]

```

## �e�����ҫإ�
### MS SQL
```
docker run --name sql2019 -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
```