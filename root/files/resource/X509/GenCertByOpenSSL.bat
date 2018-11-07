openssl genrsa -out private-key.pem 2048
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > crt.crt
openssl pkcs12 -export -out SHA256RSA.pfx -inkey private-key.pem -in crt.crt
openssl pkcs12 -in SHA256RSA.pfx -out SHA256RSA.cer -nokeys -clcerts

openssl dsaparam -out dsa.param 2048
openssl gendsa -out private-key.pem dsa.param
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > crt.crt
openssl pkcs12 -export -out SHA256DSA.pfx -inkey private-key.pem -in crt.crt
openssl pkcs12 -in SHA256DSA.pfx -out SHA256DSA.cer -nokeys -clcerts

openssl openssl ecparam -out private-key.pem -name prime256v1 -genkey
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > crt.crt
openssl pkcs12 -export -out SHA256ECDSA.pfx -inkey private-key.pem -in crt.crt
openssl pkcs12 -in SHA256ECDSA.pfx -out SHA256ECDSA.cer -nokeys -clcerts