# SimpleAspWebApi

Simple asp.net core web application with efcore and mssql

## Before Build 

* Make sure to modify connectionString Server to yours in appsetting.json.
* For DB migration you can use Package Manager Console and thease commands :

```
add-migration <migration name>
Update-database
```
### New Item 

for adding new item it expects :

```json
{
  "title": "string",
  "query": "string"
}
```
in a POST request .
