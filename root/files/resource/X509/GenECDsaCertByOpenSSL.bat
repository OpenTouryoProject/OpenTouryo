openssl ecparam -list_curves

openssl ecparam -out private-key.pem -name prime256v1 -genkey
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > _SHA256ECDSA.cer
openssl pkcs12 -export -inkey private-key.pem -in _SHA256ECDSA.cer > _SHA256ECDSA.pfx

openssl ecparam -out private-key.pem -name secp384r1 -genkey
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > _SHA384ECDSA.cer
openssl pkcs12 -export -inkey private-key.pem -in _SHA384ECDSA.cer > _SHA384ECDSA.pfx

openssl ecparam -out private-key.pem -name secp521r1 -genkey
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > _SHA512ECDSA.cer
openssl pkcs12 -export -inkey private-key.pem -in _SHA512ECDSA.cer > _SHA512ECDSA.pfx