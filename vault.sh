sleep 5 &&

curl -X POST 'http://vault:8200/v1/kv/data/user-secretstore' -H "Content-Type: application/json" -H "X-Vault-Token: admin" -d '{ "data": {"postgresql":"Host=localhost:5432;Database=users;Username=postgres;Password=postgres;Include Error Detail=true;"} }'