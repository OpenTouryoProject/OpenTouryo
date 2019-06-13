openssl dsaparam -out dsaparam.out 2048
openssl gendsa -out private-key.pem dsaparam.out
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > _SHA256DSA.cer
openssl pkcs12 -export -inkey private-key.pem -in _SHA256DSA.cer > _SHA256DSA.pfx