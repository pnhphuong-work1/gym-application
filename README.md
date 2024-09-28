
# GymApplication

## Installation

```bash
  git clone https://github.com/lqviet45/PRN231-GymApp.git
```

## Pull Development
- You should pull developmet to get the latest code
- Should using Redis Insight to see redis data
- Check appsetting (I using cloud DB always if you want to run local you should change it)
- If you want to remove some property in swagger you should look at the UpdateUserRequest in Shared.BusinessObject
- Api versioning should have format yyyy-MM-dd (ex: 2024-09-28, ect...)

## Docker
- You should check docker-compose
- and run 
```bash
  Docker compose up -d
```
## Migration DB (Postgres)
- You should change the connection in the appsetting.json
- You don't need to add Migration (Very important if you Migration there are possible to be error when update database)
- Just run update database
