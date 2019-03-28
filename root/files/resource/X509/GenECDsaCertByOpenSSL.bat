openssl ecparam -list_curves
openssl ecparam -out private-key.pem -name prime256v1 -genkey
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > _SHA256ECDSA.cer
openssl pkcs12 -export -out _SHA256ECDSA.pfx -inkey private-key.pem -in _SHA256ECDSA.cer
:openssl pkcs12 -in _SHA256ECDSA.pfx -out _SHA256ECDSA.cer -nokeys -clcerts
