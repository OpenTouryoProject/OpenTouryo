openssl genrsa -out private-key.pem 2048
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > _SHA256RSA.cer
openssl pkcs12 -export -out _SHA256RSA.pfx -inkey private-key.pem -in _SHA256RSA.cer
:openssl pkcs12 -in _SHA256RSA.pfx -out _SHA256RSA.cer -nokeys -clcerts
