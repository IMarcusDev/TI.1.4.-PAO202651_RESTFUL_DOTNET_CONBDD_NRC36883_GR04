# Login
curl -X POST http://localhost:5088/Login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "msosa",
    "password": "msosa"
  }' -k

# Movimiento
curl -X POST http://localhost:5088/Movimiento/Deposito \
  -H "Content-Type: application/json" \
  -d '{
    "token": "123",
    "cuentaOrigen": "1",
    "cuentaDestino": "123123",
    "valor": 100
  }' -k

curl -X POST http://localhost:5088/Movimiento/Retiro \
  -H "Content-Type: application/json" \
  -d '{
    "token": "123",
    "cuentaOrigen": "123123",
    "cuentaDestino": "123123",
    "valor": 50
  }' -k

curl -X POST http://localhost:5088/Movimiento/Transferencia \
  -H "Content-Type: application/json" \
  -d '{
    "token": "123",
    "cuentaOrigen": "2",
    "cuentaDestino": "123123",
    "valor": 25
  }' -k