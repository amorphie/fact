#!/bin/sh

# Let's wait for the Vault server to start.
sleep 5

# Check your secret endpoint
SECRET_CHECK=$(curl -s -o /dev/null -w "%{http_code}" -X GET 'http://vault:8200/v1/secret/data/user-secretstore' -H "X-Vault-Token: admin")

# If there is no secret, create it and set the relevant keys.
if [ "$SECRET_CHECK" -ne 200 ]; then
  curl -X POST 'http://vault:8200/v1/secret/data/user-secretstore' \
  -H "Content-Type: application/json" \
  -H "X-Vault-Token: admin" \
  -d '{
    "data": {
              "PostgreSql": "Host=localhost:5432;Database=users;Username=postgres;Password=postgres",
              "TempClient:ClientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              "TempClient:ClientCode": "TestApp",
              "TempClient:Secret": "testsecret"
            }
  }'
else
  echo "Secret 'myprojectname-secret' already exists."
fi
